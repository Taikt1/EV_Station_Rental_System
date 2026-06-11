# 👥 API QUẢN LÝ NGƯỜI DÙNG - WorkforceService

## Tổng Quan

WorkforceService cung cấp API để Manager quản lý người dùng (nhân viên và khách hàng) thông qua việc gọi UserService. Các chức năng bao gồm:

- ✅ Xem thông tin người dùng
- ✅ Cập nhật thông tin người dùng
- ✅ Thay đổi role (phân quyền)
- ✅ Khóa/Mở khóa tài khoản
- ✅ Xóa tài khoản
- ✅ Verify/Phê duyệt tài khoản

## Kiến Trúc

```
Manager/Admin → API Gateway → WorkforceService → UserService
                   ↓                    ↓              ↓
              (X-User-* headers)   (Forward headers)  (Process)
```

## API Endpoints

### 1. Lấy Thông Tin User Theo ID

**GET** `/Workforce/UserManagement/{userId}`

**Authorization:** Manager

**Response:**

```json
{
  "success": true,
  "message": "Lấy thông tin người dùng thành công",
  "data": {
    "userId": "abc123",
    "email": "user@example.com",
    "userName": "user123",
    "fullName": "Nguyễn Văn A",
    "dob": "1990-01-01T00:00:00",
    "address": "123 Main St",
    "avatarUrl": "/uploads/avatar.jpg",
    "cccdUrl": "/uploads/cccd.jpg",
    "phoneNumber": "0123456789",
    "status": "active",
    "role": "staff"
  }
}
```

### 2. Lấy Nhiều Users Cùng Lúc

**POST** `/Workforce/UserManagement/bulk`

**Authorization:** Manager

**Request Body:**

```json
["userId1", "userId2", "userId3"]
```

**Response:**

```json
{
  "success": true,
  "message": "Lấy thông tin 3/3 người dùng thành công",
  "data": {
    "userId1": {
      "userId": "userId1",
      "email": "user1@example.com",
      ...
    },
    "userId2": {
      "userId": "userId2",
      "email": "user2@example.com",
      ...
    }
  }
}
```

### 3. Cập Nhật Thông Tin User

**PUT** `/Workforce/UserManagement/{userId}`

**Authorization:** Manager

**Request Body:**

```json
{
  "fullName": "Nguyễn Văn B",
  "dob": "1990-05-15T00:00:00",
  "address": "456 New Street",
  "phoneNumber": "0987654321",
  "status": "active",
  "role": "staff"
}
```

**Response:**

```json
{
  "success": true,
  "message": "Cập nhật người dùng thành công",
  "data": {
    "userId": "abc123",
    "fullName": "Nguyễn Văn B",
    ...
  }
}
```

### 4. Thay Đổi Role

**PUT** `/Workforce/UserManagement/{userId}/role`

**Authorization:** Manager

**Request Body:**

```json
{
  "userId": "abc123",
  "newRole": "manager"
}
```

**Roles hợp lệ:**

- `customer` - Khách hàng
- `staff` - Nhân viên
- `manager` - Quản lý

**Response:**

```json
{
  "success": true,
  "message": "Đã thay đổi role của user abc123 thành manager",
  "data": {
    "userId": "abc123",
    "newRole": "manager"
  }
}
```

### 5. Khóa Tài Khoản

**PUT** `/Workforce/UserManagement/{userId}/lock`

**Authorization:** Manager

**Request Body:**

```json
{
  "userId": "abc123",
  "isLocked": true,
  "reason": "Vi phạm chính sách sử dụng"
}
```

**Response:**

```json
{
  "success": true,
  "message": "Đã khóa người dùng abc123",
  "data": {
    "userId": "abc123",
    "isLocked": true,
    "reason": "Vi phạm chính sách sử dụng"
  }
}
```

### 6. Mở Khóa Tài Khoản

**PUT** `/Workforce/UserManagement/{userId}/unlock`

**Authorization:** Manager

**Response:**

```json
{
  "success": true,
  "message": "Đã mở khóa người dùng abc123",
  "data": {
    "userId": "abc123",
    "isLocked": false
  }
}
```

### 7. Xóa Tài Khoản

**DELETE** `/Workforce/UserManagement/{userId}`

**Authorization:** Manager

**Request Body (Optional):**

```json
{
  "userId": "abc123",
  "reason": "Yêu cầu của người dùng"
}
```

**Response:**

```json
{
  "success": true,
  "message": "Đã xóa người dùng abc123",
  "data": {
    "userId": "abc123",
    "reason": "Yêu cầu của người dùng"
  }
}
```

### 8. Verify/Phê Duyệt Tài Khoản

**PUT** `/Workforce/UserManagement/{userId}/verify`

**Authorization:** Manager/Staff

**Request Body:**

```json
{
  "status": "active"
}
```

**Status hợp lệ:**

- `pending` - Chờ xác thực
- `active` - Đã xác thực
- `locked` - Đã khóa

**Response:**

```json
{
  "success": true,
  "message": "Đã cập nhật trạng thái người dùng thành active",
  "data": {
    "userId": "abc123",
    "status": "active",
    ...
  }
}
```

## Luồng Hoạt Động

### Khi Gọi Từ Gateway

```
1. Manager login → nhận JWT token
2. Gọi API qua Gateway với JWT token
3. Gateway validate JWT → tạo X-User-* headers
4. WorkforceService nhận headers
5. WorkforceService forward headers tới UserService
6. UserService xử lý request
7. Response trả về WorkforceService → Gateway → Manager
```

### Authentication Flow

```csharp
// UserManagementController tự động extract headers
var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
var gatewayHeaders = ExtractGatewayHeaders();

// Forward tới UserService
await _userMicroClient.AdminUpdateUserAsync(userId, request, token, gatewayHeaders);
```

## UserService APIs (Đã Implement)

WorkforceService gọi các API sau từ UserService:

1. **GET** `/api/User/profile/{userId}` - Lấy thông tin user
2. **PUT** `/api/User/admin/{userId}` - Cập nhật thông tin user
3. **PUT** `/api/User/admin/{userId}/role` - Thay đổi role
4. **PUT** `/api/User/admin/{userId}/lock` - Khóa tài khoản
5. **PUT** `/api/User/admin/{userId}/unlock` - Mở khóa tài khoản
6. **DELETE** `/api/User/admin/{userId}` - Xóa tài khoản

## Error Handling

### User Không Tồn Tại

```json
{
  "success": false,
  "message": "Không tìm thấy người dùng",
  "data": null
}
```

### Không Có Quyền

```json
{
  "success": false,
  "message": "Unauthorized",
  "data": null
}
```

### Lỗi Hệ Thống

```json
{
  "success": false,
  "message": "Lỗi: Connection timeout",
  "data": null
}
```

## Testing với Postman/Thunder Client

### 1. Test Qua Gateway (Khuyến nghị)

```http
GET https://gateway:7000/workforce/usermanagement/{userId}
Authorization: Bearer <JWT_TOKEN>
```

Gateway sẽ tự động thêm X-User-\* headers.

### 2. Test Trực Tiếp WorkforceService

```http
GET https://localhost:7004/Workforce/UserManagement/{userId}
Authorization: Bearer <JWT_TOKEN>
```

### 3. Test Với Gateway Headers (Manual)

```http
GET https://localhost:7004/Workforce/UserManagement/{userId}
X-User-Id: abc123
X-User-Email: manager@example.com
X-User-Role: manager
X-User-Name: Manager Name
```

## Use Cases

### UC1: Manager Xem Danh Sách Nhân Viên

```
1. Manager login
2. Lấy danh sách staff IDs từ Workday assignments
3. Gọi POST /bulk với list staff IDs
4. Hiển thị thông tin nhân viên
```

### UC2: Manager Phân Quyền Nhân Viên

```
1. Manager chọn user cần thay đổi
2. Gọi PUT /{userId}/role với newRole = "staff"
3. User được upgrade thành staff
```

### UC3: Manager Khóa Tài Khoản Vi Phạm

```
1. Phát hiện vi phạm
2. Gọi PUT /{userId}/lock với reason
3. User bị khóa, không thể login
```

### UC4: Staff Verify Khách Hàng Mới

```
1. Khách hàng upload CCCD
2. Staff review CCCD
3. Gọi PUT /{userId}/verify với status = "active"
4. Khách hàng được kích hoạt
```

## Files Liên Quan

### WorkforceService

- `Controllers/UserManagementController.cs` - API endpoints
- `HttpClients/UserMicroClient.cs` - HTTP client gọi UserService
- `DTO/UserManagementDTO.cs` - Request/Response DTOs

### UserService

- `Controllers/UserController.cs` - Admin management endpoints
- `Services/UserService.cs` - Business logic
- `DTO/AdminUserManagementDTO.cs` - Admin DTOs

## Notes

⚠️ **Quan trọng:**

- Chỉ Manager mới có quyền sử dụng hầu hết các API
- API Verify cho phép cả Manager và Staff
- Mọi thao tác đều được log (TODO: implement logging)
- Delete user là soft delete (có thể implement)
- Cần validate role trước khi thay đổi
- Nên có confirmation trước khi delete

💡 **Best Practices:**

- Luôn cung cấp reason khi lock/delete user
- Kiểm tra status hiện tại trước khi thay đổi
- Sử dụng bulk API để giảm số lần gọi
- Forward Gateway headers để maintain user context
- Log mọi admin actions cho audit trail

## Future Enhancements

- [ ] Thêm API lọc users (search, filter by role/status)
- [ ] Implement pagination cho bulk operations
- [ ] Thêm audit log cho mọi thao tác quản lý
- [ ] Email notification khi account bị lock/unlock
- [ ] Soft delete với recovery option
- [ ] Batch operations (lock/unlock nhiều users)
- [ ] Export user list to CSV/Excel
- [ ] User activity history
