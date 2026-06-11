# API GetUsersByRole - UserService

## Mô tả

API mới được thêm vào UserService để hỗ trợ Business Analytics trong WorkforceService. API này cho phép lấy danh sách users theo role.

## Endpoint

### Get Users By Role

```http
GET /api/User?role={role}
No Authorization Required (Internal microservice API)
```

**Query Parameters:**

- `role` (required): Role cần filter (customer, staff, manager)

**Response:**

```json
{
  "success": true,
  "message": "Retrieved 150 users with role 'customer'",
  "data": [
    {
      "userId": "user-guid-here",
      "email": "customer@example.com",
      "userName": "customer@example.com",
      "fullName": "Nguyễn Văn A",
      "dob": "1990-01-15T00:00:00",
      "address": "123 Đường ABC, Quận 1, TP.HCM",
      "avatarUrl": "/uploads/user-id/avatar.jpg",
      "cccdUrl": "/uploads/user-id/cccd.jpg",
      "phoneNumber": "0901234567",
      "status": "Active",
      "role": "customer"
    }
  ]
}
```

**Error Response:**

```json
{
  "success": false,
  "message": "Role parameter is required",
  "data": null
}
```

## Implementation Details

### 1. IUserService Interface

**File:** `EV_StationRentalSystem.Core/ServiceContracts/IUserService.cs`

```csharp
// Analytics support methods
Task<List<UserProfileResponse>> GetUsersByRoleAsync(string role);
```

### 2. UserService Implementation

**File:** `EV_StationRentalSystem.Core/Services/UserService.cs`

```csharp
public async Task<List<UserProfileResponse>> GetUsersByRoleAsync(string role)
{
    // Get all users in the specified role
    var usersInRole = await _userManager.GetUsersInRoleAsync(role);

    var userProfileResponses = new List<UserProfileResponse>();

    foreach (var user in usersInRole)
    {
        var userProfile = await _userProfileRepository.GetByUserIdAsync(user.Id);

        userProfileResponses.Add(new UserProfileResponse
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            FullName = userProfile?.FullName ?? string.Empty,
            Dob = userProfile?.Dob,
            Address = userProfile?.Address ?? string.Empty,
            AvatarUrl = userProfile?.AvatarUrl ?? string.Empty,
            CCCDUrl = userProfile?.CCCDUrl ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Status = user.Status ?? "Active",
            Role = role
        });
    }

    return userProfileResponses;
}
```

### 3. UserController Endpoint

**File:** `EV_StationRentalSystem.API/Controllers/UserController.cs`

````csharp
### 3. UserController Endpoint
**File:** `EV_StationRentalSystem.API/Controllers/UserController.cs`

```csharp
/// <summary>
/// Get users by role - For analytics and reporting
/// Internal API for microservice-to-microservice communication
/// No authentication required (similar to other microservices)
/// </summary>
[HttpGet]
public async Task<IActionResult> GetUsersByRole([FromQuery] string role)
{
    try
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return BadRequest(new
            {
                success = false,
                message = "Role parameter is required",
                data = (object?)null
            });
        }

        var users = await _userService.GetUsersByRoleAsync(role.ToLower());

        return Ok(new
        {
            success = true,
            message = $"Retrieved {users.Count} users with role '{role}'",
            data = users
        });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new
        {
            success = false,
            message = $"Error retrieving users by role: {ex.Message}",
            data = (object?)null
        });
    }
}
````

## Usage in WorkforceService

### UserMicroClient

**File:** `EV_StationRentalSystem_WorkforceService/EV_StationRentalSystem.Core/HttpClients/UserMicroClient.cs`

```csharp
/// <summary>
/// Get users by role for analytics
/// </summary>
public async Task<List<UserDataDTO>> GetUsersByRoleAsync(
    string role,
    string? authToken = null,
    Dictionary<string, string>? gatewayHeaders = null)
{
    try
    {
        _httpClient.DefaultRequestHeaders.Clear();

        if (!string.IsNullOrEmpty(authToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
        }

        if (gatewayHeaders != null)
        {
            foreach (var header in gatewayHeaders)
            {
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        _logger?.LogInformation("Fetching users with role: {Role}", role);

        var response = await _httpClient.GetAsync($"/api/User?role={role}");

        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogWarning("Failed to fetch users by role {Role}: {StatusCode}", role, response.StatusCode);
            return new List<UserDataDTO>();
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<UserDataDTO>>>();
        return result?.Data ?? new List<UserDataDTO>();
    }
    catch (Exception ex)
    {
        _logger?.LogError(ex, "Error fetching users by role {Role}", role);
        return new List<UserDataDTO>();
    }
}
```

### Business Analytics Usage

API này được sử dụng trong `BusinessAnalyticsService` để lấy thông tin customers:

```csharp
// Get customer analytics
var customers = await _userClient.GetUsersByRoleAsync("customer");

var customerAnalytics = new CustomerAnalyticsDTO
{
    TotalCustomers = customers.Count,
    ActiveCustomers = customers.Count(c => c.Status?.ToLower() == "active"),
    // ... more analytics
};
```

## Security

- ⚠️ **No Authorization**: API này KHÔNG yêu cầu authentication vì chỉ dùng cho internal microservice communication
- ✅ **Role Normalization**: Role được normalize thành lowercase (`role.ToLower()`)
- ✅ **Input Validation**: Kiểm tra role parameter không null/empty
- ✅ **Error Handling**: Try-catch với error messages rõ ràng
- 🔒 **Production Note**: Trong production, nên:
  - Sử dụng internal network/VPN để isolate microservices
  - Hoặc implement API Gateway pattern để restrict direct access
  - Hoặc sử dụng service mesh (Istio, Linkerd) cho mutual TLS

## Testing

### Postman/cURL Example

```bash
# Get all customers (No auth token required for internal calls)
curl -X GET "http://localhost:5001/api/User?role=customer"

# Get all staff members
curl -X GET "http://localhost:5001/api/User?role=staff"
```

### Expected Behavior

| Role              | Expected Result                      |
| ----------------- | ------------------------------------ |
| `customer`        | Returns all users with customer role |
| `staff`           | Returns all users with staff role    |
| `manager`         | Returns all users with manager role  |
| `CUSTOMER`        | Returns customers (case-insensitive) |
| `null` or `empty` | Returns 400 BadRequest               |

## Performance Considerations

- ⚠️ **N+1 Query**: Method gọi `_userProfileRepository.GetByUserIdAsync()` cho mỗi user
- 💡 **Optimization Suggestion**: Có thể tối ưu bằng cách:
  1. Lấy tất cả userIds trước
  2. Bulk query UserProfiles một lần
  3. Join trong memory

**Potential Optimization:**

```csharp
public async Task<List<UserProfileResponse>> GetUsersByRoleAsync(string role)
{
    var usersInRole = await _userManager.GetUsersInRoleAsync(role);
    var userIds = usersInRole.Select(u => u.Id).ToList();

    // Bulk fetch all profiles at once
    var profiles = await _userProfileRepository.GetByUserIdsAsync(userIds);
    var profileDict = profiles.ToDictionary(p => p.UserId);

    return usersInRole.Select(user => new UserProfileResponse
    {
        UserId = user.Id,
        Email = user.Email ?? string.Empty,
        // ... map with profileDict[user.Id]
    }).ToList();
}
```

## Related Endpoints

- `GET /api/User/profile/{userId}` - Get single user profile
- `GET /Workforce/BusinessAnalytics/customer` - Uses this API for customer analytics
- `GET /Workforce/BusinessAnalytics/dashboard` - Aggregates customer data

## Changelog

### 2024-11-16 - Initial Implementation

- ✅ Added `GetUsersByRoleAsync()` to IUserService
- ✅ Implemented in UserService using `UserManager.GetUsersInRoleAsync()`
- ✅ Added GET `/api/User?role={role}` endpoint to UserController
- ✅ Updated UserMicroClient in WorkforceService
- ✅ Build successful on both projects

## Notes

1. **Role Values**: Sử dụng lowercase roles (customer, staff, manager)
2. **Authorization**: Gateway headers (X-User-Id, X-User-Role) được forward từ API Gateway
3. **Empty Results**: API trả về empty array nếu không tìm thấy users, không throw exception
4. **Logging**: Full logging cho debugging và monitoring
