# Business Analytics API - Quick Reference

## Base URL

```
https://{gateway-url}/api/BusinessAnalytics
```

## Authentication Headers

```http
X-User-Id: {guid}
X-User-Email: {email}
X-User-Role: Manager
X-User-Name: {name}
```

---

## 📊 Quick Examples

### 1. Get Current Month Dashboard

```http
GET /api/BusinessAnalytics/dashboard?StartDate=2024-11-01&EndDate=2024-11-30&Period=daily
X-User-Role: Manager
```

### 2. Get Revenue This Week

```http
GET /api/BusinessAnalytics/revenue?StartDate=2024-11-24&EndDate=2024-11-30&Period=daily
X-User-Role: Manager
```

### 3. Check Vehicle Utilization

```http
GET /api/BusinessAnalytics/vehicle-utilization
X-User-Role: Manager
```

### 4. Compare This Month vs Last Month

```http
GET /api/BusinessAnalytics/comparison?startDate=2024-11-01&endDate=2024-11-30
X-User-Role: Manager
```

### 5. Quick Summary for Dashboard (Staff Access)

```http
GET /api/BusinessAnalytics/summary
X-User-Role: Staff
```

---

## 📈 Key Metrics Returned

### Revenue Analytics

- Total Revenue, Paid/Pending/Refunded breakdown
- Average revenue per rental
- Revenue by period/branch/vehicle type
- Payment method distribution

### Vehicle Utilization

- Total/Available/In-use/Maintenance vehicles
- Utilization rate (%)
- Top performing vehicles
- Per-vehicle details

### Customer Analytics

- Total/Active/New customers
- Customer retention rate
- Customer lifetime value
- Top customers & segmentation

### Operational Metrics

- Average check-in/out times
- Penalties and amounts
- Customer ratings
- Issue breakdown

---

## 🎯 Common Query Parameters

| Parameter   | Type     | Example      | Description                           |
| ----------- | -------- | ------------ | ------------------------------------- |
| StartDate   | DateTime | 2024-11-01   | Filter start date                     |
| EndDate     | DateTime | 2024-11-30   | Filter end date                       |
| Period      | String   | "daily"      | Group by: daily/weekly/monthly/yearly |
| BranchId    | String   | "branch-001" | Specific branch                       |
| VehicleType | String   | "type-001"   | Specific vehicle type                 |

---

## 💡 Use Case Shortcuts

### Manager Monthly Review

```bash
# Get full dashboard
curl -X GET "https://api/BusinessAnalytics/dashboard?StartDate=2024-11-01&EndDate=2024-11-30&Period=daily" \
  -H "X-User-Role: Manager"

# Compare with previous month
curl -X GET "https://api/BusinessAnalytics/comparison?startDate=2024-11-01&endDate=2024-11-30" \
  -H "X-User-Role: Manager"
```

### Fleet Optimization Analysis

```bash
# Get vehicle utilization
curl -X GET "https://api/BusinessAnalytics/vehicle-utilization?StartDate=2024-01-01&EndDate=2024-11-30" \
  -H "X-User-Role: Manager"
```

### Customer Retention Analysis

```bash
# Get customer analytics
curl -X GET "https://api/BusinessAnalytics/customer?StartDate=2024-01-01&EndDate=2024-11-30" \
  -H "X-User-Role: Manager"
```

### Daily Operations Dashboard

```bash
# Get quick summary
curl -X GET "https://api/BusinessAnalytics/summary?StartDate=2024-11-30&EndDate=2024-11-30" \
  -H "X-User-Role: Staff"
```

---

## 🚀 Response Format

All endpoints return:

```json
{
  "success": true/false,
  "message": "Description",
  "filters": { /* Applied filters */ },
  "data": { /* Analytics data */ }
}
```

---

## ⚡ Performance Tips

1. **Use date ranges** to limit data volume
2. **Cache frequently accessed data** (5-30 minutes)
3. **Use /summary** for quick dashboards
4. **Use specific filters** (branch, vehicle type) for detailed analysis
5. **Period grouping**: daily for details, monthly for trends

---

## 🔒 Authorization Matrix

| Endpoint             | Manager | Staff | Customer |
| -------------------- | ------- | ----- | -------- |
| /revenue             | ✅      | ❌    | ❌       |
| /vehicle-utilization | ✅      | ❌    | ❌       |
| /customer            | ✅      | ❌    | ❌       |
| /operational         | ✅      | ❌    | ❌       |
| /dashboard           | ✅      | ❌    | ❌       |
| /comparison          | ✅      | ❌    | ❌       |
| /summary             | ✅      | ✅    | ❌       |

---

## 🎨 JavaScript/TypeScript Example

```typescript
// Fetch dashboard data
async function getBusinessDashboard(startDate: string, endDate: string) {
  const response = await fetch(
    `/api/BusinessAnalytics/dashboard?StartDate=${startDate}&EndDate=${endDate}&Period=daily`,
    {
      headers: {
        'X-User-Role': 'Manager',
        'X-User-Id': userId,
        'X-User-Email': userEmail,
        'X-User-Name': userName,
      },
    }
  )

  const result = await response.json()

  if (!result.success) {
    console.error(result.message)
    return null
  }

  return result.data
}

// Usage
const dashboard = await getBusinessDashboard('2024-11-01', '2024-11-30')
console.log('Total Revenue:', dashboard.revenueAnalytics.totalRevenue)
console.log(
  'Utilization Rate:',
  dashboard.vehicleUtilization.overallUtilizationRate
)
```

---

## 🐍 Python Example

```python
import requests
from datetime import datetime, timedelta

def get_revenue_analytics(start_date, end_date, period='monthly'):
    headers = {
        'X-User-Role': 'Manager',
        'X-User-Id': user_id,
        'X-User-Email': user_email,
        'X-User-Name': user_name
    }

    params = {
        'StartDate': start_date,
        'EndDate': end_date,
        'Period': period
    }

    response = requests.get(
        'https://api/BusinessAnalytics/revenue',
        headers=headers,
        params=params
    )

    result = response.json()

    if result['success']:
        return result['data']
    else:
        raise Exception(result['message'])

# Usage
analytics = get_revenue_analytics('2024-11-01', '2024-11-30')
print(f"Total Revenue: {analytics['totalRevenue']}")
print(f"Total Rentals: {analytics['totalRentals']}")
```

---

## 🔍 Troubleshooting

### 401 Unauthorized

- Check `X-User-Role` header is "Manager" (or "Staff" for /summary)
- Verify all Gateway headers are present

### 400 Bad Request

- Ensure `startDate < endDate` for comparison endpoint
- Check date format (yyyy-MM-dd)

### 500 Internal Server Error

- Check microservice availability (Fleet, RentalPayment, User)
- Review logs for connection issues

### Slow Response

- Use smaller date ranges
- Add specific filters (branch, vehicle type)
- Use period grouping (monthly instead of daily)

---

## 📞 Support

For questions or issues:

- Full Documentation: `BUSINESS_ANALYTICS_API.md`
- Swagger UI: `/swagger`
- Technical Support: dev-team@example.com

---

## 🎯 Quick Checklist

Before calling analytics APIs:

- [ ] Gateway headers configured
- [ ] User role is Manager (or Staff for /summary)
- [ ] Date range is valid (start < end)
- [ ] Period parameter is correct (daily/weekly/monthly/yearly)
- [ ] All microservices are running (Fleet, RentalPayment, User)
- [ ] HTTP client timeout is sufficient (5-10 seconds)

---

**Last Updated:** 2024-11-30
**Version:** 1.0.0
