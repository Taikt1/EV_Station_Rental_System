# 🔧 BUG FIXES - WorkforceService

## Vấn Đề Đã Sửa

### ❌ **Lỗi 1: JSON Circular Reference**

**Error:**

```
System.Text.Json.JsonException: A possible object cycle was detected.
This can either be due to a cycle or if the object depth is larger than
the maximum allowed depth of 32.
Path: $.data.Assignments.Workday.Assignments.Workday.Assignments...
```

**Nguyên nhân:**

- `StaffAssignmentDTO` có property `WorkdayDTO`
- `WorkdayDTO` có list `StaffAssignmentDTO`
- Tạo ra vòng lặp vô hạn khi serialize JSON

**Giải pháp:**

1. **Xóa circular reference trong DTO:**

```csharp
// StaffAssignmentDTO.cs - TRƯỚC
public class StaffAssignmentDTO
{
    public ShiftDTO? Shift { get; set; }
    public WorkdayDTO? Workday { get; set; }  // ❌ Gây circular reference
}

// StaffAssignmentDTO.cs - SAU
public class StaffAssignmentDTO
{
    public ShiftDTO? Shift { get; set; }
    // ✅ Đã xóa WorkdayDTO để tránh circular reference
}
```

2. **Cấu hình JSON Serializer:**

```csharp
// Program.cs
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Tránh circular reference
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

        // Ignore null values
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
```

3. **Cập nhật AutoMapper:**

```csharp
// WorkforceMappingProfile.cs
CreateMap<StaffAssignment, StaffAssignmentDTO>()
    .ForMember(dest => dest.Shift, opt => opt.MapFrom(src => src.Shift));
    // ✅ CHỈ map Shift, KHÔNG map Workday
```

---

### ❌ **Lỗi 2: Timeout khi gọi UserService**

**Error:**

```
Error calling UserService: The delegate executed asynchronously through
TimeoutPolicy did not complete within the timeout.
```

**Nguyên nhân:**

- Timeout policy chỉ 3 giây quá ngắn
- UserService có thể chậm phản hồi
- Không có error handling tốt

**Giải pháp:**

1. **Tăng timeout trong Polly Policy:**

```csharp
// UsersMicroservicePolicies.cs - TRƯỚC
var timeoutPolicy = _pollyPolicies.GetTimeoutPolicy(TimeSpan.FromSeconds(3)); // ❌ Quá ngắn

// UsersMicroservicePolicies.cs - SAU
var timeoutPolicy = _pollyPolicies.GetTimeoutPolicy(TimeSpan.FromSeconds(30)); // ✅ 30 giây
```

2. **Cải thiện error handling trong UserMicroClient:**

```csharp
// UserMicroClient.cs
public async Task<UserProfileResponse?> GetUserProfileAsync(string userId, string? authToken = null)
{
    try
    {
        _logger?.LogInformation("Calling UserService for userId: {UserId}", userId);

        var response = await _httpClient.GetAsync($"/api/User/profile/{userId}");

        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogWarning("UserService returned {StatusCode}", response.StatusCode);
            return null;
        }

        return result?.Data;
    }
    catch (TaskCanceledException ex)
    {
        _logger?.LogError(ex, "Timeout calling UserService");
        return null;
    }
    catch (HttpRequestException ex)
    {
        _logger?.LogError(ex, "HTTP error calling UserService");
        return null;
    }
}
```

3. **Thêm logging với ILogger:**

```csharp
// UserMicroClient.cs
private readonly ILogger<UserMicroClient>? _logger;

public UserMicroClient(HttpClient httpClient, ILogger<UserMicroClient>? logger = null)
{
    _httpClient = httpClient;
    _logger = logger;
}
```

4. **Cải thiện performance với parallel loading:**

```csharp
// GetMultipleUserProfilesAsync - TRƯỚC (Sequential)
foreach (var userId in userIds)
{
    var profile = await GetUserProfileAsync(userId, authToken);
    if (profile != null) result[userId] = profile;
}

// GetMultipleUserProfilesAsync - SAU (Parallel)
var tasks = userIds.Select(async userId =>
{
    var profile = await GetUserProfileAsync(userId, authToken);
    return new { UserId = userId, Profile = profile };
});

var results = await Task.WhenAll(tasks);
```

---

## Các Files Đã Sửa

### 1. **StaffAssignmentDTO.cs**

- ✅ Xóa property `WorkdayDTO` để tránh circular reference
- ✅ Chỉ giữ lại `ShiftDTO`

### 2. **Program.cs**

- ✅ Thêm `ReferenceHandler.IgnoreCycles` cho JSON serializer
- ✅ Thêm `DefaultIgnoreCondition.WhenWritingNull`

### 3. **WorkforceMappingProfile.cs**

- ✅ Cập nhật mapping để không map circular references

### 4. **UsersMicroservicePolicies.cs**

- ✅ Tăng timeout từ 3s → 30s
- ✅ Giảm retry count từ 4 → 3
- ✅ Tăng circuit breaker threshold

### 5. **UserMicroClient.cs**

- ✅ Thêm ILogger dependency
- ✅ Cải thiện error handling với try-catch specific exceptions
- ✅ Thêm logging cho debug
- ✅ Parallel loading cho multiple users
- ✅ Better timeout handling

---

## Kết Quả Sau Khi Fix

### ✅ **Trước đây:**

```json
❌ Error: JSON cycle detected
❌ Error: Timeout after 3 seconds
❌ Console.WriteLine("Error...") không có stack trace
❌ Sequential loading chậm
```

### ✅ **Bây giờ:**

```json
✅ JSON serialize thành công với IgnoreCycles
✅ Timeout 30 giây, đủ thời gian cho UserService
✅ Logging chi tiết với ILogger
✅ Parallel loading nhanh hơn
✅ Error handling specific cho từng loại exception
```

---

## Testing

### Test 1: Lấy Workday (không còn circular reference)

```http
GET /api/Workday/{id}
Authorization: Bearer {token}

Response:
{
  "success": true,
  "data": {
    "workdayId": "...",
    "staffId": "...",
    "assignments": [
      {
        "assignmentId": "...",
        "shift": {
          "shiftId": "...",
          "shiftName": "Ca Sáng"
        }
        // ✅ Không có workday property nữa
      }
    ],
    "staffInfo": {
      "userId": "...",
      "fullName": "Nguyễn Văn A"
    }
  }
}
```

### Test 2: UserService timeout

```
Before:
❌ Error after 3 seconds
❌ No detailed logs

After:
✅ Wait up to 30 seconds
✅ Detailed logging:
   - "Calling UserService for userId: xxx"
   - "Successfully retrieved user profile"
   - "Timeout calling UserService" (nếu timeout)
```

---

## Best Practices Đã Áp Dụng

1. **DTO Design**

   - ✅ Tránh circular references
   - ✅ Chỉ include data cần thiết

2. **Error Handling**

   - ✅ Specific exception handling
   - ✅ Logging với ILogger
   - ✅ Graceful degradation (return null nếu UserService fail)

3. **Performance**

   - ✅ Parallel loading
   - ✅ Reasonable timeout values
   - ✅ Ignore null values trong JSON

4. **Resilience**
   - ✅ Polly retry policy
   - ✅ Circuit breaker
   - ✅ Timeout policy

---

## Lưu Ý Quan Trọng

### 🔴 **UserService phải chạy trước WorkforceService**

```powershell
# Terminal 1: UserService
cd d:\EV_Station_Rental_System\EV_StationRentalSystem_UserService
dotnet run --project EV_StationRentalSystem.API

# Terminal 2: WorkforceService
cd d:\EV_Station_Rental_System\EV_StationRentalSystem_WorkforceService
dotnet run --project EV_StationRentalSystem.API
```

### 🔴 **Kiểm tra URL UserService**

Trong `appsettings.json`:

```json
{
  "UserMicroName": "localhost",
  "UserMicroPort": "7001"
}
```

Đảm bảo UserService đang chạy tại `https://localhost:7001`

### 🔴 **JWT Key phải giống nhau**

Cả UserService và WorkforceService phải dùng cùng JWT Key:

```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyHere123456789012345678901234567890"
  }
}
```

---

## Monitoring & Debugging

### View Logs

```
info: EV_StationRentalSystem.Core.HttpClients.UserMicroClient[0]
      Calling UserService for userId: xxx

info: EV_StationRentalSystem.Core.HttpClients.UserMicroClient[0]
      Successfully retrieved user profile for userId: xxx

info: EV_StationRentalSystem.Core.HttpClients.UserMicroClient[0]
      Fetching 5 user profiles

info: EV_StationRentalSystem.Core.HttpClients.UserMicroClient[0]
      Successfully fetched 5/5 user profiles
```

### Nếu còn lỗi timeout

1. Kiểm tra UserService có đang chạy không
2. Kiểm tra port có đúng không
3. Kiểm tra firewall
4. Tăng timeout lên nếu cần (trong UsersMicroservicePolicies.cs)

---

**Status: ✅ All issues fixed and tested!**
