# Hướng Dẫn Chi Tiết: API Quản Lý Lịch Làm Việc Nhân Viên (Workforce Service)

## Tổng Quan Kiến Trúc

Hệ thống WorkforceService quản lý lịch làm việc của nhân viên với các thành phần chính:

1. **Shift (Ca làm việc)**: Định nghĩa các ca làm việc (sáng, chiều, tối)
2. **Workday (Ngày làm việc)**: Một ngày làm việc của một nhân viên tại một chi nhánh
3. **StaffAssignment (Phân công)**: Phân công nhân viên vào các ca làm việc cụ thể
4. **UserMicroClient**: HttpClient để gọi UserService lấy thông tin nhân viên

## Các Bước Đã Thực Hiện

### ✅ Bước 1: Tạo DTOs (Data Transfer Objects)

**Files đã tạo:**

- `ShiftDTO.cs`: DTO cho ca làm việc
- `WorkdayDTO.cs`: DTO cho ngày làm việc (có kèm thông tin user)
- `StaffAssignmentDTO.cs`: DTO cho phân công công việc
- `UserProfileResponse.cs`: DTO nhận từ UserService

**Đặc điểm:**

- Các Request/Response DTOs riêng biệt
- Filter request với pagination
- Bulk operation request

### ✅ Bước 2: Cập Nhật UserMicroClient

**File: `UserMicroClient.cs`**

**Chức năng:**

```csharp
// Lấy thông tin 1 user
GetUserProfileAsync(userId, authToken)

// Lấy thông tin nhiều users
GetMultipleUserProfilesAsync(userIds, authToken)
```

**Cách hoạt động:**

- Gọi API UserService: `GET /api/User/profile/{userId}`
- Truyền JWT token qua Authorization header
- Parse response và trả về UserProfileResponse

### ✅ Bước 3-4: Repository Pattern

**Interfaces:**

- `IShiftRepository`
- `IWorkdayRepository`
- `IStaffAssignmentRepository`

**Implementations:**

- `ShiftRepository`: CRUD cho shifts
- `WorkdayRepository`: CRUD + filter + includes
- `StaffAssignmentRepository`: CRUD + bulk operations

**Đặc điểm:**

- Include navigation properties khi cần
- Filter methods với nhiều điều kiện
- Validation methods (Exists, HasShift, etc.)

### ✅ Bước 5: Service Layer

**Interface: `IWorkforceService`**
**Implementation: `WorkforceService`**

**Chức năng chính:**

#### Shift Management

- GetShiftByIdAsync
- GetAllShiftsAsync
- CreateShiftAsync
- UpdateShiftAsync
- DeleteShiftAsync

#### Workday Management

- GetWorkdayByIdAsync (có call UserService)
- GetWorkdaysByFilterAsync (có call UserService cho nhiều users)
- CreateWorkdayAsync (validate duplicate)
- UpdateWorkdayAsync
- DeleteWorkdayAsync

#### Assignment Management

- GetAssignmentByIdAsync
- GetAssignmentsByWorkdayAsync
- CreateAssignmentAsync (validate workday, shift, duplicate)
- UpdateAssignmentAsync
- DeleteAssignmentAsync
- CreateBulkAssignmentsAsync (tạo nhiều assignments cho nhiều ngày)

#### Special Queries

- GetStaffScheduleAsync: Lịch làm việc của 1 nhân viên
- GetBranchScheduleAsync: Lịch làm việc toàn chi nhánh

**Integration với UserService:**

```csharp
// Lấy thông tin 1 user
var userProfile = await _userMicroClient.GetUserProfileAsync(
    staffId.ToString(),
    authToken
);
workdayDto.StaffInfo = userProfile;

// Lấy thông tin nhiều users
var staffIds = workdayDtos.Select(w => w.StaffId.ToString()).Distinct().ToList();
var userProfiles = await _userMicroClient.GetMultipleUserProfilesAsync(
    staffIds,
    authToken
);
```

### ✅ Bước 6: AutoMapper Configuration

**File: `WorkforceMappingProfile.cs`**

Mappings:

- Shift ↔ ShiftDTO
- Workday ↔ WorkdayDTO (with navigation)
- StaffAssignment ↔ StaffAssignmentDTO

### ✅ Bước 7: Dependency Injection

**Core DI:**

- IWorkforceService → WorkforceService
- IJwtService → JwtService

**Infrastructure DI:**

- IShiftRepository → ShiftRepository
- IWorkdayRepository → WorkdayRepository
- IStaffAssignmentRepository → StaffAssignmentRepository

### ✅ Bước 8: API Controllers

#### **ShiftController**

```
GET    /api/Shift              - Lấy tất cả shifts
GET    /api/Shift/{id}         - Lấy shift theo ID
POST   /api/Shift              - Tạo shift (Manager only)
PUT    /api/Shift/{id}         - Cập nhật shift (Manager only)
DELETE /api/Shift/{id}         - Xóa shift (Manager only)
```

#### **WorkdayController**

```
GET    /api/Workday                        - Lấy workdays (có filter)
GET    /api/Workday/{id}                   - Lấy workday theo ID
POST   /api/Workday                        - Tạo workday (Manager)
PUT    /api/Workday/{id}                   - Cập nhật workday (Manager)
DELETE /api/Workday/{id}                   - Xóa workday (Manager)
GET    /api/Workday/staff/{id}/schedule    - Lịch nhân viên
GET    /api/Workday/branch/{id}/schedule   - Lịch chi nhánh
```

#### **AssignmentController**

```
GET    /api/Assignment/{id}           - Lấy assignment theo ID
GET    /api/Assignment/workday/{id}   - Lấy assignments của workday
POST   /api/Assignment                - Tạo assignment (Manager)
POST   /api/Assignment/bulk           - Tạo nhiều assignments (Manager)
PUT    /api/Assignment/{id}           - Cập nhật assignment (Manager/Staff)
DELETE /api/Assignment/{id}           - Xóa assignment (Manager)
```

**Authorization:**

- Manager: Toàn quyền CRUD
- Staff: Xem lịch của mình, cập nhật trạng thái ca
- Customer: Không có quyền truy cập

### ✅ Bước 9: JWT Authentication

**Program.cs:**

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        // Validate token từ UserService
    });
```

### ✅ Bước 10: Configuration

**appsettings.json:**

```json
{
  "Jwt": {
    "Key": "...",
    "Issuer": "EVStationRentalSystem",
    "Audience": "EVStationRentalSystem"
  },
  "UserMicroName": "localhost",
  "UserMicroPort": "7001",
  ...
}
```

### ✅ Bước 11: API Testing File

**File: `WorkforceAPI.http`**

- Tất cả endpoint examples
- Workflow examples
- Sample requests

## Cách Chạy Migration

### Bước 1: Mở Terminal trong VS Code

```powershell
cd d:\EV_Station_Rental_System\EV_StationRentalSystem_WorkforceService
```

### Bước 2: Chạy Migration Commands

```powershell
# Tạo migration
dotnet ef migrations add InitialCreate --project EV_StationRentalSystem.Infrastructure --startup-project EV_StationRentalSystem.API

# Update database
dotnet ef database update --project EV_StationRentalSystem.Infrastructure --startup-project EV_StationRentalSystem.API
```

### Bước 3: Verify Database

Kiểm tra SQL Server xem các bảng đã được tạo:

- Shifts
- Workdays
- StaffAssignments
- StaffReassignments

## Workflows Thực Tế

### Workflow 1: Setup Shifts (Chỉ cần làm 1 lần)

```http
POST https://localhost:7004/api/Shift
Authorization: Bearer {manager_token}

{
  "shiftName": "Ca Sáng",
  "startTime": "08:00:00",
  "endTime": "12:00:00"
}

POST https://localhost:7004/api/Shift
{
  "shiftName": "Ca Chiều",
  "startTime": "13:00:00",
  "endTime": "17:00:00"
}

POST https://localhost:7004/api/Shift
{
  "shiftName": "Ca Tối",
  "startTime": "18:00:00",
  "endTime": "22:00:00"
}
```

### Workflow 2: Manager Phân Công Lịch Cho Nhân Viên

```http
# Bước 1: Tạo lịch làm việc hàng loạt (1 tuần)
POST https://localhost:7004/api/Assignment/bulk
Authorization: Bearer {manager_token}

{
  "staffId": "guid-cua-nhan-vien",
  "branchId": "guid-cua-chi-nhanh",
  "startDate": "2024-11-05",
  "endDate": "2024-11-11",
  "shiftIds": [
    "guid-ca-sang",
    "guid-ca-chieu"
  ],
  "task": "Phục vụ khách hàng, kiểm tra trạm sạc"
}
```

### Workflow 3: Nhân Viên Xem Lịch Của Mình

```http
GET https://localhost:7004/api/Workday/staff/{staffId}/schedule?startDate=2024-11-01&endDate=2024-11-30
Authorization: Bearer {staff_token}

Response sẽ bao gồm:
- Workday với thông tin chi nhánh
- Các ca làm việc được phân công
- Thông tin profile của nhân viên (từ UserService)
```

### Workflow 4: Manager Xem Lịch Toàn Chi Nhánh

```http
GET https://localhost:7004/api/Workday/branch/{branchId}/schedule?startDate=2024-11-01&endDate=2024-11-30
Authorization: Bearer {manager_token}

Response bao gồm:
- Tất cả workdays trong chi nhánh
- Thông tin tất cả nhân viên (từ UserService)
- Các ca đã phân công
```

### Workflow 5: Nhân Viên Cập Nhật Trạng Thái Ca

```http
PUT https://localhost:7004/api/Assignment/{assignmentId}
Authorization: Bearer {staff_token}

{
  "status": "Completed"
}
```

## Tích Hợp Với UserService

### Token Flow

1. User login qua UserService → nhận JWT token
2. Gửi request đến WorkforceService với token trong header
3. WorkforceService validate token
4. WorkforceService gọi UserService để lấy thông tin user
5. Response trả về kèm thông tin user

### Example Request

```http
GET /api/Workday/staff/{staffId}/schedule
Authorization: Bearer eyJhbGc...

Headers được forward đến UserService:
GET https://localhost:7001/api/User/profile/{staffId}
Authorization: Bearer eyJhbGc...
```

## Error Handling

Service xử lý các lỗi:

- Workday duplicate: "Nhân viên đã có lịch làm việc trong ngày này"
- Shift duplicate: "Ca làm việc này đã được phân công"
- Not found: "Không tìm thấy..."
- Validation errors từ business logic

## Security

- JWT token validation
- Role-based authorization:
  - Manager: Full CRUD
  - Staff: Read own schedule + Update status
- HttpClient sử dụng Polly policies (retry, circuit breaker)

## Performance Considerations

- Include navigation properties chỉ khi cần
- Batch loading user profiles (GetMultipleUserProfilesAsync)
- Pagination cho list endpoints
- Async/await throughout

## Testing Tips

1. **Test với Manager role trước**: Tạo shifts, workdays
2. **Test bulk assignment**: Tạo lịch cho nhiều ngày
3. **Test với Staff role**: Xem lịch, cập nhật status
4. **Test integration**: Verify thông tin user từ UserService
5. **Test edge cases**: Duplicate, not found, validation errors

## Các Command Hữu Ích

```powershell
# Build project
dotnet build

# Run project
dotnet run --project EV_StationRentalSystem.API

# Watch mode (auto-reload)
dotnet watch --project EV_StationRentalSystem.API

# Clean
dotnet clean

# Restore packages
dotnet restore
```

## Next Steps

Sau khi hoàn thành WorkforceService, bạn có thể:

1. **Thêm validation**: FluentValidation cho DTOs
2. **Thêm logging**: Serilog cho tracking
3. **Thêm caching**: Redis cho user profiles
4. **Thêm notifications**: SignalR khi có lịch mới
5. **Thêm reports**: Export lịch làm việc ra Excel/PDF
6. **Thêm dashboard**: Thống kê giờ làm việc, attendance rate

---

**Lưu ý quan trọng:**

- Đảm bảo UserService đang chạy trước khi test WorkforceService
- JWT Key phải giống nhau giữa các services
- Connection string SQL Server phải đúng
- Port numbers phải khớp với configuration
