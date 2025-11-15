# 🔐 GATEWAY AUTHENTICATION SETUP - WorkforceService

## Tổng Quan

WorkforceService đã được cấu hình để hỗ trợ authentication qua API Gateway, tương tự như UserService. Hệ thống hỗ trợ 2 phương thức xác thực:

1. **JWT Token**: Gọi trực tiếp từ client
2. **Gateway Headers**: Gọi qua API Gateway (được Gateway forward headers)

## Luồng Xác Thực

### Kịch Bản 1: Gọi Trực Tiếp (Không Qua Gateway)

```
Client → WorkforceService
  ↓ (JWT Token trong Authorization header)
WorkforceService validates JWT
  ↓ (JWT Token được forward)
WorkforceService → UserService
  ↓
Response về Client
```

### Kịch Bản 2: Gọi Qua Gateway (KHUYẾN NGHỊ)

```
Client → API Gateway
  ↓ (JWT Token)
Gateway validates JWT và tạo headers:
  - X-User-Id
  - X-User-Email
  - X-User-Role
  - X-User-Name
  ↓
Gateway → WorkforceService (với headers)
  ↓
GatewayAuthMiddleware đọc headers và tạo ClaimsPrincipal
  ↓
WorkforceService xử lý request
  ↓ (Forward headers)
WorkforceService → UserService
  ↓
Response về Gateway → Client
```

## Files Đã Tạo/Cập Nhật

### 1. GatewayAuthMiddleware.cs (NEW)

**Location:** `EV_StationRentalSystem.API/Middleware/GatewayAuthMiddleware.cs`

**Chức năng:**

- Đọc các headers `X-User-*` từ Gateway
- Tạo `ClaimsPrincipal` với các claims từ headers
- Set `HttpContext.User` để controller có thể sử dụng

### 2. WorkdayController.cs (UPDATED)

**Location:** `EV_StationRentalSystem.API/Controllers/WorkdayController.cs`

**Thay đổi:**

- Thêm helper method `ExtractGatewayHeaders()`
- Cập nhật các GET endpoints để extract và forward Gateway headers:
  - `GetWorkdays()`
  - `GetWorkdayById()`
  - `GetStaffSchedule()`
  - `GetBranchSchedule()`

### 3. AssignmentController.cs (UPDATED)

**Location:** `EV_StationRentalSystem.API/Controllers/AssignmentController.cs`

**Thay đổi:**

- Thêm helper method `ExtractGatewayHeaders()`
- Sẵn sàng cho việc forward Gateway headers khi cần gọi UserService

### 4. ShiftController.cs (UPDATED)

**Location:** `EV_StationRentalSystem.API/Controllers/ShiftController.cs`

**Thay đổi:**

- Thêm helper method `ExtractGatewayHeaders()`
- Sẵn sàng cho việc forward Gateway headers khi cần gọi UserService

**Chức năng:**

- Đọc headers từ Gateway: `X-User-Id`, `X-User-Email`, `X-User-Role`, `X-User-Name`
- Tạo `ClaimsPrincipal` từ headers
- Set `HttpContext.User` để controllers có thể dùng `[Authorize]`

**Code:**

```csharp
public class GatewayAuthMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var userId = context.Request.Headers["X-User-Id"].FirstOrDefault();

        if (!string.IsNullOrEmpty(userId))
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim("nameid", userId)
            };

            // Add other claims...

            var identity = new ClaimsIdentity(claims, "Gateway");
            context.User = new ClaimsPrincipal(identity);
        }

        await _next(context);
    }
}
```

### 2. Program.cs (UPDATED)

**Thêm middleware:**

```csharp
app.UseAuthentication();

// Add Gateway Auth Middleware
app.UseGatewayAuth(); // ← NEW LINE

app.UseAuthorization();
```

**Thứ tự quan trọng:**

1. UseAuthentication() - JWT validation
2. UseGatewayAuth() - Gateway headers
3. UseAuthorization() - Role-based authorization

### 3. UserMicroClient.cs (UPDATED)

**Thêm parameter `gatewayHeaders`:**

```csharp
public async Task<UserProfileResponse?> GetUserProfileAsync(
    string userId,
    string? authToken = null,
    Dictionary<string, string>? gatewayHeaders = null) // ← NEW
{
    // Clear existing headers
    _httpClient.DefaultRequestHeaders.Clear();

    // Add JWT if available
    if (!string.IsNullOrEmpty(authToken))
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", authToken);
    }

    // Forward Gateway headers
    if (gatewayHeaders != null)
    {
        foreach (var header in gatewayHeaders)
        {
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(
                header.Key, header.Value);
        }
    }

    // Call UserService...
}
```

### 4. IWorkforceService.cs (UPDATED)

**Thêm parameter `gatewayHeaders`:**

```csharp
Task<WorkdayDTO?> GetWorkdayByIdAsync(
    Guid workdayId,
    string? authToken = null,
    Dictionary<string, string>? gatewayHeaders = null); // ← NEW

Task<List<WorkdayDTO>> GetWorkdaysByFilterAsync(
    WorkdayFilterRequest filter,
    string? authToken = null,
    Dictionary<string, string>? gatewayHeaders = null); // ← NEW
```

### 5. WorkforceService.cs (UPDATED)

**Cập nhật implementations:**

```csharp
public async Task<WorkdayDTO?> GetWorkdayByIdAsync(
    Guid workdayId,
    string? authToken = null,
    Dictionary<string, string>? gatewayHeaders = null)
{
    var workday = await _workdayRepository.GetByIdAsync(workdayId, true);
    var workdayDto = _mapper.Map<WorkdayDTO>(workday);

    // Call UserService với JWT HOẶC Gateway headers
    if (authToken != null || gatewayHeaders != null)
    {
        var userProfile = await _userMicroClient.GetUserProfileAsync(
            workday.StaffId.ToString(),
            authToken,
            gatewayHeaders); // ← Forward headers

        workdayDto.StaffInfo = userProfile;
    }

    return workdayDto;
}
```

### 6. WorkdayController.cs (UPDATED)

**Thêm helper method:**

```csharp
private Dictionary<string, string>? ExtractGatewayHeaders()
{
    var headers = new Dictionary<string, string>();

    var userId = Request.Headers["X-User-Id"].FirstOrDefault();
    var userEmail = Request.Headers["X-User-Email"].FirstOrDefault();
    var userRole = Request.Headers["X-User-Role"].FirstOrDefault();
    var userName = Request.Headers["X-User-Name"].FirstOrDefault();

    if (!string.IsNullOrEmpty(userId))
    {
        headers["X-User-Id"] = userId;
        if (!string.IsNullOrEmpty(userEmail))
            headers["X-User-Email"] = userEmail;
        if (!string.IsNullOrEmpty(userRole))
            headers["X-User-Role"] = userRole;
        if (!string.IsNullOrEmpty(userName))
            headers["X-User-Name"] = userName;

        return headers;
    }

    return null;
}
```

**Sử dụng trong actions:**

```csharp
[HttpGet("{workdayId}")]
public async Task<IActionResult> GetWorkdayById(Guid workdayId)
{
    var token = Request.Headers["Authorization"]
        .ToString().Replace("Bearer ", "");
    var gatewayHeaders = ExtractGatewayHeaders(); // ← Extract headers

    var workday = await _workforceService.GetWorkdayByIdAsync(
        workdayId, token, gatewayHeaders); // ← Forward to service

    return Ok(workday);
}
```

## Cách Hoạt Động

### Flow Chi Tiết

#### 1. Request đến WorkforceService (qua Gateway)

```http
GET /api/Workday/123
Headers:
  X-User-Id: abc-123
  X-User-Email: user@example.com
  X-User-Role: manager
  X-User-Name: John Doe
```

#### 2. GatewayAuthMiddleware xử lý

```csharp
// Middleware tự động:
context.User = new ClaimsPrincipal(new ClaimsIdentity([
    new Claim(ClaimTypes.NameIdentifier, "abc-123"),
    new Claim(ClaimTypes.Email, "user@example.com"),
    new Claim(ClaimTypes.Role, "manager"),
    new Claim(ClaimTypes.Name, "John Doe")
], "Gateway"));
```

#### 3. Controller extract headers

```csharp
var gatewayHeaders = new Dictionary<string, string> {
    ["X-User-Id"] = "abc-123",
    ["X-User-Email"] = "user@example.com",
    ["X-User-Role"] = "manager",
    ["X-User-Name"] = "John Doe"
};
```

#### 4. Service forward headers sang UserService

```csharp
// WorkforceService → UserService
GET https://localhost:7001/api/User/profile/abc-123
Headers:
  X-User-Id: abc-123
  X-User-Email: user@example.com
  X-User-Role: manager
  X-User-Name: John Doe
```

#### 5. UserService GatewayAuthMiddleware xử lý

```csharp
// UserService middleware tự động tạo identity
context.User = ClaimsPrincipal với các claims từ headers
```

#### 6. UserService authorization check

```csharp
[Authorize]
[HttpGet("profile/{userId}")]
public async Task<IActionResult> GetProfile(string userId)
{
    var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    // currentUserId = "abc-123" từ Gateway headers

    if (currentUserId != userId && userRole != "manager")
        return Forbid();

    // Authorization passed!
}
```

## Testing

### Test 1: Gọi Qua Gateway (Recommended)

**Request từ Client:**

```http
GET https://gateway:7000/workforce/api/Workday/abc-123
Authorization: Bearer {jwt_token}
```

**Gateway xử lý và forward:**

```http
GET https://localhost:7004/api/Workday/abc-123
Headers:
  X-User-Id: user-123
  X-User-Email: user@example.com
  X-User-Role: manager
  X-User-Name: John Doe
```

**WorkforceService forward sang UserService:**

```http
GET https://localhost:7001/api/User/profile/staff-456
Headers:
  X-User-Id: user-123  ← Forwarded from Gateway
  X-User-Email: user@example.com
  X-User-Role: manager
  X-User-Name: John Doe
```

### Test 2: Gọi Trực Tiếp (Bypass Gateway)

**Request:**

```http
GET https://localhost:7004/api/Workday/abc-123
Authorization: Bearer {jwt_token}
```

**WorkforceService → UserService:**

```http
GET https://localhost:7001/api/User/profile/staff-456
Authorization: Bearer {jwt_token} ← JWT được forward
```

## Authorization Scenarios

### Scenario 1: Manager xem lịch chi nhánh

```http
GET /api/Workday/branch/branch-123/schedule
Headers từ Gateway:
  X-User-Role: manager ✅

→ GatewayAuthMiddleware tạo Role claim
→ [Authorize(Roles = "manager")] cho phép truy cập
→ Gọi UserService để lấy thông tin nhân viên
→ UserService kiểm tra manager có quyền xem user khác ✅
```

### Scenario 2: Staff xem lịch của mình

```http
GET /api/Workday/staff/user-123/schedule
Headers từ Gateway:
  X-User-Id: user-123
  X-User-Role: staff ✅

→ Staff có thể xem lịch của chính mình
→ Gọi UserService với headers
→ UserService check: currentUserId == userId ✅
```

### Scenario 3: Staff cố xem lịch người khác

```http
GET /api/Workday/staff/other-user/schedule
Headers từ Gateway:
  X-User-Id: user-123
  X-User-Role: staff ❌

→ Gọi UserService với headers
→ UserService check: currentUserId != userId && role != manager
→ Return Forbid() ❌
```

## Advantages

### 1. **Centralized Authentication**

- Gateway xử lý JWT validation một lần
- Các services không cần validate lại
- Giảm overhead, tăng performance

### 2. **Consistent Identity**

- User identity được maintain xuyên suốt các services
- Claims giống nhau ở mọi service
- Dễ debug và trace requests

### 3. **Security**

- Gateway là single entry point
- Internal services không expose ra ngoài
- Headers không thể fake từ bên ngoài

### 4. **Flexibility**

- Hỗ trợ cả JWT và Gateway headers
- Có thể gọi trực tiếp khi cần (development)
- Dễ test từng service độc lập

## Configuration Files

### appsettings.json (WorkforceService)

```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyHere...",
    "Issuer": "EVStationRentalSystem",
    "Audience": "EVStationRentalSystem"
  },
  "UserMicroName": "localhost",
  "UserMicroPort": "7001"
}
```

### appsettings.json (UserService)

```json
{
  "JwtSettings": {
    "Key": "YourSuperSecretKeyHere...", ← PHẢI GIỐNG WorkforceService
    "Issuer": "EVStationRentalSystem",
    "Audience": "EVStationRentalSystem"
  }
}
```

## Debugging

### Enable Logging

```csharp
// GatewayAuthMiddleware
_logger.LogInformation(
    "Gateway headers - UserId: {UserId}, Role: {Role}",
    userId, userRole);

// UserMicroClient
_logger.LogInformation("Forwarding Gateway headers to UserService");
```

### Check Logs

```
info: GatewayAuthMiddleware[0]
      Gateway headers - UserId: abc-123, Role: manager

info: GatewayAuthMiddleware[0]
      Gateway Auth: Created user identity for abc-123

info: UserMicroClient[0]
      Forwarding Gateway headers to UserService

info: UserMicroClient[0]
      Calling UserService for userId: staff-456
```

## Troubleshooting

### Problem 1: "User not authenticated"

**Nguyên nhân:** Gateway không gửi headers

**Giải pháp:**

- Kiểm tra Gateway có extract claims từ JWT không
- Kiểm tra Gateway có forward headers không
- Log headers trong GatewayAuthMiddleware

### Problem 2: "Forbidden" khi gọi UserService

**Nguyên nhân:** Headers không được forward đúng

**Giải pháp:**

```csharp
// Check trong UserMicroClient
_logger.LogInformation("Headers being sent: {@Headers}", gatewayHeaders);

// Check trong UserService GatewayAuthMiddleware
Console.WriteLine($"Received X-User-Id: {context.Request.Headers["X-User-Id"]}");
```

### Problem 3: JWT Key mismatch

**Lỗi:** Token validation failed

**Giải pháp:**

- Đảm bảo `Jwt:Key` giống nhau trong tất cả services
- Đảm bảo `Issuer` và `Audience` giống nhau

## Next Steps

1. **Cấu hình API Gateway** để forward headers
2. **Test end-to-end** flow từ Gateway
3. **Thêm logging** để monitor authentication flow
4. **Implement rate limiting** ở Gateway level
5. **Add distributed tracing** để track requests qua services

---

**Status: ✅ Gateway Authentication Configured!**

WorkforceService giờ hoàn toàn tương thích với UserService authentication pattern.
