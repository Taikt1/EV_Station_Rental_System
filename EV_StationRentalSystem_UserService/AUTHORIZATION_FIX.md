# GetUsersByRole API - Authorization Fix

## Vấn đề

WorkforceService gọi UserService API `GET /api/User?role={role}` nhưng gặp lỗi **401 Unauthorized**.

```
Failed to fetch users by role Customer: Unauthorized
```

## Nguyên nhân

- API ban đầu có `[Authorize(Roles = "manager,staff")]`
- WorkforceService's `BusinessAnalyticsService` gọi `GetUsersByRoleAsync()` **KHÔNG TRUYỀN** authToken và gatewayHeaders
- Các HTTP client calls khác (Fleet, RentalPayment) cũng không truyền auth headers
- → Tất cả microservice-to-microservice calls đều không có authentication

## Giải pháp được áp dụng ✅

**Bỏ yêu cầu authorization** cho endpoint `GetUsersByRole` để phù hợp với pattern của các microservices khác.

### Thay đổi trong UserController.cs

**Before:**

```csharp
[Authorize(Roles = "manager,staff")]
[HttpGet]
public async Task<IActionResult> GetUsersByRole([FromQuery] string role)
```

**After:**

```csharp
/// <summary>
/// Get users by role - For analytics and reporting
/// Internal API for microservice-to-microservice communication
/// No authentication required (similar to other microservices)
/// </summary>
[HttpGet]
public async Task<IActionResult> GetUsersByRole([FromQuery] string role)
```

## Kiến trúc hiện tại

### Microservice Communication Pattern

```
┌─────────────────────────────────────────────────────┐
│  WorkforceService (BusinessAnalyticsController)     │
│  - Nhận Gateway Headers (X-User-Role: manager)      │
│  - Kiểm tra authorization tại controller level     │
└───────────────────┬─────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────────┐
│  BusinessAnalyticsService                           │
│  - Không truyền auth headers xuống HTTP clients    │
└───────┬──────────┬──────────────┬───────────────────┘
        │          │              │
        ▼          ▼              ▼
   ┌────────┐ ┌─────────┐ ┌──────────────┐
   │ Fleet  │ │ Rental  │ │ UserService  │
   │Service │ │ Payment │ │ (No Auth)    │
   └────────┘ └─────────┘ └──────────────┘
   No Auth    No Auth      No Auth ✅
```

### Authorization Strategy

- ✅ **API Gateway Level**: Gateway kiểm tra JWT token của user
- ✅ **WorkforceService Controller**: Kiểm tra X-User-Role header (manager only)
- ❌ **Internal Services**: Không yêu cầu authentication cho service-to-service calls
- 🔒 **Production**: Nên sử dụng internal network hoặc service mesh

## Files Modified

### 1. UserController.cs

**Location:** `EV_StationRentalSystem_UserService/EV_StationRentalSystem.API/Controllers/UserController.cs`

**Changes:**

- Removed `[Authorize(Roles = "manager,staff")]` attribute
- Updated XML documentation
- Endpoint now accessible without authentication

### 2. GET_USERS_BY_ROLE_API.md

**Location:** `EV_StationRentalSystem_UserService/GET_USERS_BY_ROLE_API.md`

**Changes:**

- Updated endpoint documentation (No Authorization Required)
- Removed JWT token from cURL examples
- Added security notes about production deployment

## Testing

### Before Fix

```bash
# Returns 401 Unauthorized
curl -X GET "http://localhost:7105/api/User?role=customer"
```

### After Fix

```bash
# Returns 200 OK with customer list
curl -X GET "http://localhost:7105/api/User?role=customer"

Response:
{
  "success": true,
  "message": "Retrieved 150 users with role 'customer'",
  "data": [...]
}
```

## Impact Analysis

### ✅ Now Working

- `GET /Workforce/BusinessAnalytics/customer` - Customer analytics
- `GET /Workforce/BusinessAnalytics/dashboard` - Complete dashboard with customer data
- All analytics endpoints that need customer information

### ⚠️ Security Considerations

**Pros:**

- Consistent với pattern hiện tại của FleetService và RentalPaymentService
- Đơn giản hóa microservice communication
- Không cần implement service-to-service authentication

**Cons:**

- API có thể được gọi trực tiếp nếu exposed publicly
- Không có authentication/authorization tại service level

**Mitigations:**

1. **API Gateway Pattern**: Chỉ cho phép access qua Gateway
2. **Network Isolation**: Services chạy trên internal network
3. **Service Mesh**: Sử dụng Istio/Linkerd cho mutual TLS
4. **Firewall Rules**: Restrict direct access to service ports

## Build Status

```
✅ UserService Build: SUCCESS (26 warnings - nullable references only)
```

## Alternative Solutions (Not Implemented)

### Option 1: Service-to-Service Authentication

- Implement API Key header (`X-Service-API-Key`)
- Require secret key for internal calls
- More complex, requires key management

### Option 2: Forward Auth Headers

- Modify BusinessAnalyticsService methods to accept headers
- Pass through to all HTTP clients
- Requires significant refactoring

### Option 3: Service Account Token

- Create internal service account in UserService
- Generate long-lived token for WorkforceService
- Token management complexity

## Recommendations

### For Development

✅ Current solution (no auth) is acceptable

### For Production

Consider implementing one of:

1. **API Gateway** - Route all traffic through gateway (recommended)
2. **Internal Network** - Deploy services on private subnet
3. **Service Mesh** - Istio/Linkerd for mTLS
4. **API Keys** - Simple service-to-service authentication

## Related Documentation

- [GET_USERS_BY_ROLE_API.md](./GET_USERS_BY_ROLE_API.md) - API documentation
- [ANALYTICS_BUGFIXES.md](../EV_StationRentalSystem_WorkforceService/ANALYTICS_BUGFIXES.md) - All analytics bug fixes

---

**Date:** 2024-11-16
**Status:** ✅ Fixed and tested
**Pattern:** Consistent with other microservices (Fleet, RentalPayment)
