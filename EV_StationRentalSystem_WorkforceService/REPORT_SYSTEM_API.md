# 📋 API QUẢN LÝ BÁO CÁO & KHIẾU NẠI - WorkforceService

## Tổng Quan

Hệ thống quản lý báo cáo và khiếu nại từ người dùng, cho phép:

- 👤 **User**: Tạo báo cáo, xem báo cáo của mình, đánh giá sau khi giải quyết
- 👨‍💼 **Staff**: Xử lý báo cáo được assign, giải quyết vấn đề
- 👔 **Manager**: Quản lý toàn bộ báo cáo, assign cho staff, xem thống kê

## Database Schema

### UserReport Entity

```csharp
public class UserReport
{
    Guid ReportId                   // Primary key
    string ReporterId               // User ID của người báo cáo
    string? ReporterEmail           // Email
    string? ReporterName            // Tên người báo cáo
    string ReportType               // Loại: complaint, feedback, technical_issue, safety_issue, other
    string Category                 // Danh mục: station, vehicle, payment, staff, app, other
    string Priority                 // Ưu tiên: low, medium, high, urgent
    string Title                    // Tiêu đề
    string Description              // Mô tả chi tiết
    Guid? RelatedStationId          // Station liên quan
    Guid? RelatedVehicleId          // Xe liên quan
    Guid? RelatedRentalId           // Rental liên quan
    string? AttachmentUrls          // URLs hình ảnh (JSON array)
    string Status                   // pending, in_progress, resolved, closed, rejected
    string? AssignedToStaffId       // Staff đang xử lý
    string? AssignedToStaffName     // Tên staff
    string? StaffNotes              // Ghi chú nội bộ
    string? Resolution              // Phản hồi cho user
    DateTime CreatedAt              // Thời gian tạo
    DateTime UpdatedAt              // Thời gian cập nhật
    DateTime? StartedAt             // Thời gian bắt đầu xử lý
    DateTime? ResolvedAt            // Thời gian hoàn thành
    int? UserRating                 // Đánh giá 1-5
    string? UserFeedback            // Nhận xét từ user
}
```

## API Endpoints

### 1. Tạo Báo Cáo (User)

**POST** `/Workforce/Report`

**Authorization:** User, Staff, Manager

**Request Body:**

```json
{
  "reportType": "complaint",
  "category": "vehicle",
  "priority": "high",
  "title": "Xe bị hư trong lúc thuê",
  "description": "Xe bị hỏng phanh sau 1 giờ sử dụng, rất nguy hiểm",
  "relatedStationId": "guid-station-id",
  "relatedVehicleId": "guid-vehicle-id",
  "relatedRentalId": "guid-rental-id",
  "attachmentUrls": ["/uploads/photo1.jpg", "/uploads/photo2.jpg"]
}
```

**Report Types:**

- `complaint` - Khiếu nại
- `feedback` - Góp ý
- `technical_issue` - Vấn đề kỹ thuật
- `safety_issue` - Vấn đề an toàn
- `other` - Khác

**Categories:**

- `station` - Trạm/Chi nhánh
- `vehicle` - Xe
- `payment` - Thanh toán
- `staff` - Nhân viên
- `app` - Ứng dụng
- `other` - Khác

**Priority Levels:**

- `low` - Thấp
- `medium` - Trung bình (mặc định)
- `high` - Cao
- `urgent` - Khẩn cấp

**Response:**

```json
{
  "success": true,
  "message": "Tạo báo cáo thành công",
  "data": {
    "reportId": "guid",
    "reporterId": "user-id",
    "reporterEmail": "user@example.com",
    "reporterName": "Nguyễn Văn A",
    "reportType": "complaint",
    "category": "vehicle",
    "priority": "high",
    "title": "Xe bị hư trong lúc thuê",
    "description": "...",
    "status": "pending",
    "createdAt": "2025-11-14T10:00:00Z",
    ...
  }
}
```

### 2. Xem Báo Cáo Của Tôi (User)

**GET** `/Workforce/Report/my-reports`

**Authorization:** User, Staff, Manager

**Response:**

```json
{
  "success": true,
  "message": "Lấy danh sách 5 báo cáo thành công",
  "data": [
    {
      "reportId": "guid",
      "title": "...",
      "status": "pending",
      "createdAt": "2025-11-14T10:00:00Z",
      ...
    }
  ]
}
```

### 3. Xem Chi Tiết Báo Cáo

**GET** `/Workforce/Report/{reportId}`

**Authorization:** User (chỉ báo cáo của mình), Staff, Manager

**Response:**

```json
{
  "success": true,
  "message": "Lấy thông tin báo cáo thành công",
  "data": {
    "reportId": "guid",
    "reporterId": "user-id",
    "title": "...",
    "description": "...",
    "status": "in_progress",
    "assignedToStaffId": "staff-id",
    "assignedToStaffName": "Staff Name",
    "staffNotes": "Đang kiểm tra xe",
    "resolution": null,
    "createdAt": "2025-11-14T10:00:00Z",
    "startedAt": "2025-11-14T11:00:00Z",
    ...
  }
}
```

### 4. Đánh Giá Báo Cáo Đã Giải Quyết (User)

**POST** `/Workforce/Report/{reportId}/rate`

**Authorization:** User (chỉ người tạo báo cáo)

**Request Body:**

```json
{
  "rating": 5,
  "feedback": "Xử lý rất nhanh và chuyên nghiệp"
}
```

**Rating:** 1-5 stars

**Response:**

```json
{
  "success": true,
  "message": "Đánh giá báo cáo thành công",
  "data": {
    "reportId": "guid",
    "status": "resolved",
    "userRating": 5,
    "userFeedback": "Xử lý rất nhanh và chuyên nghiệp",
    ...
  }
}
```

### 5. Lấy Danh Sách Báo Cáo Với Filter (Staff/Manager)

**GET** `/Workforce/Report?status=pending&priority=high&pageNumber=1&pageSize=20`

**Authorization:** Staff, Manager

**Query Parameters:**

- `reportType` - Filter theo loại
- `category` - Filter theo danh mục
- `status` - Filter theo trạng thái
- `priority` - Filter theo ưu tiên
- `assignedToStaffId` - Filter theo staff
- `reporterId` - Filter theo người báo cáo
- `fromDate` - Từ ngày
- `toDate` - Đến ngày
- `searchKeyword` - Tìm kiếm
- `pageNumber` - Trang (mặc định 1)
- `pageSize` - Số lượng (mặc định 20)

**Response:**

```json
{
  "success": true,
  "message": "Lấy danh sách báo cáo thành công",
  "data": [
    {
      "reportId": "guid",
      "title": "...",
      "status": "pending",
      ...
    }
  ],
  "pagination": {
    "pageNumber": 1,
    "pageSize": 20,
    "totalRecords": 150,
    "totalPages": 8
  }
}
```

### 6. Lấy Báo Cáo Được Assign Cho Tôi (Staff)

**GET** `/Workforce/Report/my-assigned`

**Authorization:** Staff, Manager

**Response:**

```json
{
  "success": true,
  "message": "Lấy danh sách 3 báo cáo được assign thành công",
  "data": [
    {
      "reportId": "guid",
      "title": "...",
      "status": "in_progress",
      "assignedToStaffId": "my-id",
      ...
    }
  ]
}
```

### 7. Assign Báo Cáo Cho Staff (Manager/Staff)

**PUT** `/Workforce/Report/{reportId}/assign`

**Authorization:** Manager, Staff

**Request Body:**

```json
{
  "staffId": "staff-user-id",
  "notes": "Assign cho bạn xử lý case này"
}
```

**Response:**

```json
{
  "success": true,
  "message": "Assign báo cáo thành công",
  "data": {
    "reportId": "guid",
    "status": "in_progress",
    "assignedToStaffId": "staff-user-id",
    "assignedToStaffName": "Staff Name",
    "startedAt": "2025-11-14T11:00:00Z",
    ...
  }
}
```

### 8. Cập Nhật Báo Cáo (Manager/Staff)

**PUT** `/Workforce/Report/{reportId}`

**Authorization:** Manager, Staff

**Request Body:**

```json
{
  "priority": "urgent",
  "status": "in_progress",
  "staffNotes": "Đã liên hệ khách hàng, đang kiểm tra xe"
}
```

### 9. Giải Quyết Báo Cáo (Staff)

**PUT** `/Workforce/Report/{reportId}/resolve`

**Authorization:** Manager, Staff

**Request Body:**

```json
{
  "resolution": "Đã thay phanh mới cho xe, hoàn tiền 50% chi phí thuê cho khách hàng",
  "staffNotes": "Đã kiểm tra xe, phanh thực sự có vấn đề"
}
```

**Response:**

```json
{
  "success": true,
  "message": "Giải quyết báo cáo thành công",
  "data": {
    "reportId": "guid",
    "status": "resolved",
    "resolution": "...",
    "resolvedAt": "2025-11-14T15:00:00Z",
    ...
  }
}
```

### 10. Đóng Báo Cáo (Manager/Staff)

**PUT** `/Workforce/Report/{reportId}/close`

**Authorization:** Manager, Staff

**Response:**

```json
{
  "success": true,
  "message": "Đóng báo cáo thành công",
  "data": {
    "reportId": "guid",
    "status": "closed",
    ...
  }
}
```

### 11. Từ Chối Báo Cáo (Manager/Staff)

**PUT** `/Workforce/Report/{reportId}/reject`

**Authorization:** Manager, Staff

**Request Body:**

```json
{
  "reason": "Báo cáo không đúng sự thật, đã kiểm tra camera"
}
```

**Response:**

```json
{
  "success": true,
  "message": "Từ chối báo cáo thành công",
  "data": {
    "reportId": "guid",
    "status": "rejected",
    "resolution": "Rejected: Báo cáo không đúng sự thật...",
    ...
  }
}
```

### 12. Xóa Báo Cáo (Manager)

**DELETE** `/Workforce/Report/{reportId}`

**Authorization:** Manager

**Response:**

```json
{
  "success": true,
  "message": "Xóa báo cáo thành công",
  "data": null
}
```

### 13. Thống Kê Báo Cáo (Manager)

**GET** `/Workforce/Report/statistics?fromDate=2025-01-01&toDate=2025-12-31`

**Authorization:** Manager

**Query Parameters:**

- `fromDate` - Từ ngày (optional)
- `toDate` - Đến ngày (optional)

**Response:**

```json
{
  "success": true,
  "message": "Lấy thống kê báo cáo thành công",
  "data": {
    "totalReports": 500,
    "pendingReports": 50,
    "inProgressReports": 100,
    "resolvedReports": 300,
    "closedReports": 40,
    "rejectedReports": 10,
    "reportsByType": {
      "complaint": 200,
      "feedback": 150,
      "technical_issue": 100,
      "safety_issue": 30,
      "other": 20
    },
    "reportsByCategory": {
      "station": 100,
      "vehicle": 250,
      "payment": 80,
      "staff": 40,
      "app": 30
    },
    "reportsByPriority": {
      "low": 100,
      "medium": 250,
      "high": 120,
      "urgent": 30
    },
    "averageResolutionTimeHours": 24.5,
    "averageRating": 4.3
  }
}
```

## Workflow

### 1. User Tạo Báo Cáo

```
User → Tạo báo cáo → Status: "pending"
                    ↓
            Email notification → Manager/Staff
```

### 2. Staff Xử Lý

```
Manager → Assign cho Staff → Status: "in_progress"
                           ↓
                    Staff xử lý
                           ↓
                 Resolve → Status: "resolved"
                           ↓
            Email notification → User
```

### 3. User Đánh Giá

```
User nhận thông báo → Xem resolution → Rate (1-5 stars)
                                              ↓
                                    Status: "resolved" (có rating)
```

### 4. Đóng Báo Cáo

```
Resolved report → Manager/Staff close → Status: "closed"
```

## Status Flow

```
pending → in_progress → resolved → closed
   ↓
rejected
```

## Use Cases

### UC1: Khách Hàng Khiếu Nại Xe Hỏng

```
1. User tạo report: type=complaint, category=vehicle, priority=high
2. Manager nhìn thấy trong dashboard
3. Manager assign cho Staff A
4. Staff A kiểm tra xe, thêm staff notes
5. Staff A resolve với solution
6. User nhận thông báo, đánh giá 5 sao
7. Staff đóng báo cáo
```

### UC2: Khách Hàng Góp Ý Cải Thiện App

```
1. User tạo report: type=feedback, category=app, priority=low
2. Staff xem và ghi nhận
3. Staff forward cho IT team (staff notes)
4. Staff resolve với thanks message
5. Manager đóng báo cáo
```

### UC3: Báo Cáo Vấn Đề An Toàn

```
1. User tạo report: type=safety_issue, category=station, priority=urgent
2. System auto-assign cho on-duty manager
3. Manager xử lý ngay lập tức
4. Manager resolve và close
```

## Features

✅ **Multi-level Priority** - Ưu tiên từ low đến urgent
✅ **Status Tracking** - Theo dõi tiến độ xử lý
✅ **Assignment System** - Assign cho staff phù hợp
✅ **Rich Context** - Liên kết với station, vehicle, rental
✅ **Attachments** - Hỗ trợ upload hình ảnh
✅ **Staff Notes** - Ghi chú nội bộ
✅ **User Rating** - Đánh giá chất lượng xử lý
✅ **Statistics** - Thống kê đa chiều
✅ **Search & Filter** - Tìm kiếm mạnh mẽ
✅ **Gateway Auth** - Hỗ trợ X-User-\* headers

## Best Practices

### Cho User:

- Mô tả chi tiết vấn đề
- Upload hình ảnh minh chứng
- Chọn đúng category và type
- Đánh giá sau khi được giải quyết

### Cho Staff:

- Cập nhật staff notes thường xuyên
- Resolve với resolution rõ ràng
- Xử lý báo cáo urgent ưu tiên
- Close báo cáo sau khi hoàn tất

### Cho Manager:

- Assign hợp lý theo kỹ năng staff
- Monitor pending reports
- Review statistics định kỳ
- Escalate urgent cases

## Database Migration

Chạy migration để tạo bảng UserReports:

```bash
cd EV_StationRentalSystem.Infrastructure
dotnet ef migrations add AddUserReportTable --project ../EV_StationRentalSystem.API
dotnet ef database update --project ../EV_StationRentalSystem.API
```

## Testing Scenarios

### Test 1: Create Report

```http
POST /Workforce/Report
Content-Type: application/json
X-User-Id: user123
X-User-Email: user@example.com
X-User-Name: User Name

{
  "reportType": "complaint",
  "category": "vehicle",
  "priority": "high",
  "title": "Test Report",
  "description": "Test description"
}
```

### Test 2: Assign Report

```http
PUT /Workforce/Report/{reportId}/assign
Content-Type: application/json
X-User-Id: manager123
X-User-Role: manager

{
  "staffId": "staff456",
  "notes": "Please handle this"
}
```

### Test 3: Resolve Report

```http
PUT /Workforce/Report/{reportId}/resolve
Content-Type: application/json
X-User-Id: staff456

{
  "resolution": "Problem fixed",
  "staffNotes": "Replaced brake pads"
}
```

## Future Enhancements

- [ ] Email notifications
- [ ] Real-time updates via SignalR
- [ ] File upload API for attachments
- [ ] SLA tracking (resolution time)
- [ ] Auto-assignment based on category
- [ ] Escalation rules
- [ ] Report templates
- [ ] Bulk operations
- [ ] Export reports to Excel
- [ ] Analytics dashboard
