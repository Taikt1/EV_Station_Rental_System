# Business Analytics API Documentation

## Overview

This API provides comprehensive business analytics for the EV Station Rental System's WorkforceService. It aggregates data from multiple microservices (FleetService, RentalPaymentService, UserService) to calculate key business metrics including revenue analytics, vehicle utilization rates, customer analytics, and operational metrics.

## Architecture

The Business Analytics system follows a distributed data aggregation pattern:

```
┌─────────────────────────┐
│  WorkforceService       │
│  (Analytics Engine)     │
└─────────────────────────┘
           │
           ├──────────────────┐
           │                  │
           ▼                  ▼
┌──────────────────┐   ┌──────────────────┐
│  FleetService    │   │ RentalPayment    │
│  - Vehicles      │   │ Service          │
│  - Vehicle Types │   │ - Rentals        │
│  - Status        │   │ - Payments       │
└──────────────────┘   │ - Analytics      │
                       └──────────────────┘
           │
           ▼
    ┌──────────────────┐
    │  UserService     │
    │  - Customers     │
    │  - Staff         │
    │  - Profiles      │
    └──────────────────┘
```

## Authentication

All endpoints require **Manager** role authentication via Gateway headers:

- `X-User-Id`: User's unique identifier
- `X-User-Email`: User's email
- `X-User-Role`: Must be "Manager" (except summary endpoint which allows "Staff")
- `X-User-Name`: User's display name

## Endpoints

### 1. Get Revenue Analytics

Retrieve comprehensive revenue metrics including total revenue, payment breakdown, and revenue by period/branch/vehicle type.

**Endpoint:** `GET /api/BusinessAnalytics/revenue`

**Authorization:** Manager only

**Query Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| StartDate | DateTime | No | Filter start date (yyyy-MM-dd) |
| EndDate | DateTime | No | Filter end date (yyyy-MM-dd) |
| BranchId | String | No | Specific branch ID |
| VehicleType | String | No | Specific vehicle type |
| Period | String | No | Grouping period: "daily", "weekly", "monthly", "yearly" (default: "monthly") |

**Example Request:**

```http
GET /api/BusinessAnalytics/revenue?StartDate=2024-01-01&EndDate=2024-12-31&Period=monthly
X-User-Id: 550e8400-e29b-41d4-a716-446655440000
X-User-Email: manager@example.com
X-User-Role: Manager
X-User-Name: John Manager
```

**Response Schema:**

```json
{
  "success": true,
  "message": "Revenue analytics retrieved successfully",
  "filters": {
    "startDate": "2024-01-01T00:00:00",
    "endDate": "2024-12-31T23:59:59",
    "period": "monthly"
  },
  "data": {
    "totalRevenue": 1500000.5,
    "paidRevenue": 1400000.0,
    "pendingRevenue": 80000.5,
    "refundedRevenue": 20000.0,
    "averageRevenuePerRental": 75000.25,
    "totalRentals": 200,
    "completedRentals": 180,
    "activeRentals": 15,
    "cancelledRentals": 5,
    "revenueByPeriod": [
      {
        "period": "2024-01",
        "revenue": 150000.0,
        "rentalCount": 20,
        "averageRevenue": 7500.0
      }
    ],
    "revenueByBranch": [
      {
        "branchId": "branch-001",
        "branchName": "Branch Downtown",
        "revenue": 500000.0,
        "rentalCount": 65,
        "percentage": 33.33
      }
    ],
    "revenueByVehicleType": [
      {
        "vehicleTypeId": "type-001",
        "vehicleTypeName": "Tesla Model 3",
        "revenue": 600000.0,
        "rentalCount": 80,
        "percentage": 40.0
      }
    ],
    "paymentMethodBreakdown": {
      "cashAmount": 200000.0,
      "cashCount": 30,
      "cardAmount": 500000.0,
      "cardCount": 70,
      "eWalletAmount": 600000.0,
      "eWalletCount": 85,
      "bankTransferAmount": 100000.0,
      "bankTransferCount": 15
    }
  }
}
```

---

### 2. Get Vehicle Utilization Analytics

Analyze vehicle usage patterns, utilization rates, and performance metrics.

**Endpoint:** `GET /api/BusinessAnalytics/vehicle-utilization`

**Authorization:** Manager only

**Query Parameters:** Same as Revenue Analytics

**Example Request:**

```http
GET /api/BusinessAnalytics/vehicle-utilization?StartDate=2024-11-01&EndDate=2024-11-30
X-User-Id: 550e8400-e29b-41d4-a716-446655440000
X-User-Role: Manager
```

**Response Schema:**

```json
{
  "success": true,
  "message": "Vehicle utilization analytics retrieved successfully",
  "data": {
    "totalVehicles": 50,
    "availableVehicles": 30,
    "inUseVehicles": 15,
    "maintenanceVehicles": 5,
    "overallUtilizationRate": 62.5,
    "averageRentalDurationHours": 8.5,
    "totalRentalHours": 1700,
    "utilizationByType": [
      {
        "vehicleTypeId": "type-001",
        "vehicleTypeName": "Tesla Model 3",
        "totalVehicles": 20,
        "availableVehicles": 12,
        "inUseVehicles": 6,
        "maintenanceVehicles": 2,
        "utilizationRate": 70.5,
        "totalRentals": 80,
        "totalRentalHours": 680
      }
    ],
    "vehicleDetails": [
      {
        "vehicleId": "veh-001",
        "plateNumber": "59A-12345",
        "vehicleType": "Tesla Model 3",
        "status": "Available",
        "rentalCount": 15,
        "rentalHours": 120,
        "utilizationRate": 75.0,
        "totalRevenue": 90000.0,
        "lastRentalDate": "2024-11-28T14:30:00"
      }
    ],
    "topPerformingVehicles": [
      {
        "vehicleId": "veh-001",
        "plateNumber": "59A-12345",
        "vehicleType": "Tesla Model 3",
        "rentalCount": 15,
        "totalRevenue": 90000.0,
        "utilizationRate": 75.0,
        "averageRating": 4.8
      }
    ]
  }
}
```

**Utilization Rate Formula:**

```
Utilization Rate = (Total Rental Hours / Total Possible Hours) × 100
Total Possible Hours = Total Vehicles × 24 hours × Number of Days
```

---

### 3. Get Customer Analytics

Analyze customer behavior, segmentation, and lifetime value.

**Endpoint:** `GET /api/BusinessAnalytics/customer`

**Authorization:** Manager only

**Query Parameters:** Same as Revenue Analytics

**Example Request:**

```http
GET /api/BusinessAnalytics/customer?StartDate=2024-01-01&EndDate=2024-12-31
X-User-Role: Manager
```

**Response Schema:**

```json
{
  "success": true,
  "message": "Customer analytics retrieved successfully",
  "data": {
    "totalCustomers": 500,
    "activeCustomers": 350,
    "newCustomersInPeriod": 120,
    "customerRetentionRate": 70.0,
    "averageCustomerLifetimeValue": 4285.71,
    "averageRentalsPerCustomer": 2.5,
    "topCustomers": [
      {
        "customerId": "cust-001",
        "customerName": "Nguyen Van A",
        "email": "customer@example.com",
        "totalRentals": 25,
        "totalSpent": 187500.0,
        "averageRating": 4.9,
        "lastRentalDate": "2024-11-29T10:00:00"
      }
    ],
    "customerSegments": [
      {
        "segment": "New",
        "customerCount": 120,
        "percentage": 24.0,
        "averageSpending": 50000.0
      },
      {
        "segment": "Regular",
        "customerCount": 200,
        "percentage": 40.0,
        "averageSpending": 125000.0
      },
      {
        "segment": "VIP",
        "customerCount": 80,
        "percentage": 16.0,
        "averageSpending": 350000.0
      },
      {
        "segment": "Inactive",
        "customerCount": 100,
        "percentage": 20.0,
        "averageSpending": 0
      }
    ]
  }
}
```

**Customer Segmentation Logic:**

- **New**: 0-1 rentals
- **Regular**: 2-5 rentals
- **VIP**: 6+ rentals
- **Inactive**: 0 rentals in period

---

### 4. Get Operational Metrics

Monitor operational efficiency and service quality metrics.

**Endpoint:** `GET /api/BusinessAnalytics/operational`

**Authorization:** Manager only

**Query Parameters:** Same as Revenue Analytics

**Response Schema:**

```json
{
  "success": true,
  "message": "Operational metrics retrieved successfully",
  "data": {
    "averageCheckInTime": 15.5,
    "averageCheckOutTime": 12.3,
    "totalPenalties": 25,
    "totalPenaltyAmount": 5000000.0,
    "maintenanceIssuesReported": 15,
    "averageCustomerRating": 4.5,
    "totalFeedbacks": 180,
    "positiveFeedbacks": 150,
    "negativeFeedbacks": 30,
    "issueBreakdown": [
      {
        "issueType": "Battery Issue",
        "count": 8,
        "percentage": 53.3
      },
      {
        "issueType": "Tire Problem",
        "count": 5,
        "percentage": 33.3
      }
    ]
  }
}
```

---

### 5. Get Complete Business Dashboard

Get all analytics in a single comprehensive dashboard response.

**Endpoint:** `GET /api/BusinessAnalytics/dashboard`

**Authorization:** Manager only

**Query Parameters:** Same as Revenue Analytics

**Example Request:**

```http
GET /api/BusinessAnalytics/dashboard?StartDate=2024-11-01&EndDate=2024-11-30&Period=daily
X-User-Role: Manager
X-User-Name: John Manager
```

**Response Schema:**

```json
{
  "success": true,
  "message": "Business dashboard retrieved successfully for John Manager",
  "data": {
    "generatedAt": "2024-11-30T15:30:00Z",
    "filters": {
      "startDate": "2024-11-01",
      "endDate": "2024-11-30",
      "period": "daily"
    },
    "revenueAnalytics": {
      /* Full revenue analytics object */
    },
    "vehicleUtilization": {
      /* Full vehicle utilization object */
    },
    "customerAnalytics": {
      /* Full customer analytics object */
    },
    "operationalMetrics": {
      /* Full operational metrics object */
    }
  }
}
```

---

### 6. Get Period Comparison

Compare business metrics between current and previous periods.

**Endpoint:** `GET /api/BusinessAnalytics/comparison`

**Authorization:** Manager only

**Query Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| startDate | DateTime | Yes | Current period start date |
| endDate | DateTime | Yes | Current period end date |

**Example Request:**

```http
GET /api/BusinessAnalytics/comparison?startDate=2024-11-01&endDate=2024-11-30
X-User-Role: Manager
```

**Response Schema:**

```json
{
  "success": true,
  "message": "Period comparison retrieved successfully",
  "data": {
    "currentPeriod": "2024-11-01 to 2024-11-30",
    "previousPeriod": "2024-10-01 to 2024-10-31",
    "currentRevenue": 150000.0,
    "previousRevenue": 120000.0,
    "revenueGrowthPercentage": 25.0,
    "currentRentals": 50,
    "previousRentals": 40,
    "rentalGrowthPercentage": 25.0,
    "currentUtilizationRate": 65.5,
    "previousUtilizationRate": 58.2,
    "utilizationChange": 7.3
  }
}
```

**Growth Calculation:**

```
Growth % = ((Current - Previous) / Previous) × 100
```

---

### 7. Get Quick Summary

Get essential metrics for dashboard cards (accessible by Manager and Staff).

**Endpoint:** `GET /api/BusinessAnalytics/summary`

**Authorization:** Manager or Staff

**Query Parameters:** Same as Revenue Analytics

**Example Request:**

```http
GET /api/BusinessAnalytics/summary
X-User-Role: Staff
```

**Response Schema:**

```json
{
  "success": true,
  "message": "Quick summary retrieved successfully",
  "data": {
    "totalRevenue": 1500000.5,
    "totalRentals": 200,
    "activeRentals": 15,
    "completedRentals": 180,
    "averageRevenuePerRental": 7500.25,
    "totalVehicles": 50,
    "availableVehicles": 30,
    "utilizationRate": 62.5,
    "totalCustomers": 500,
    "activeCustomers": 350,
    "newCustomers": 120
  }
}
```

---

## Data Sources

### FleetService APIs Used

- `GET /api/vehicles` - Get all vehicles
- `GET /api/vehicles/status/{status}` - Get vehicles by status
- `GET /api/vehicles/{id}` - Get vehicle details
- `GET /api/type-vehicles` - Get all vehicle types
- `GET /api/vehicles/status/summary` - Get status summary

### RentalPaymentService APIs Used

- `GET /api/rentals?fromDate={}&toDate={}&status={}` - Get rental orders
- `GET /api/rentals/{id}` - Get rental order details
- `GET /api/payments?fromDate={}&toDate={}` - Get payments
- `GET /api/payments/rental/{rentalId}` - Get payments by rental
- `GET /api/analytics/renter/{renterId}` - Get renter analytics

### UserService APIs Used

- `GET /api/User?role={role}` - Get users by role
- `GET /api/User/profile/{userId}` - Get user profile

---

## Use Cases

### UC1: Monthly Business Review

**Actor:** Manager

**Flow:**

1. Manager accesses business dashboard
2. Selects date range (current month)
3. Reviews comprehensive metrics:
   - Total revenue and growth trends
   - Vehicle utilization efficiency
   - Top performing vehicles
   - Customer acquisition and retention
   - Operational bottlenecks
4. Exports data for presentation

**API Calls:**

```http
GET /api/BusinessAnalytics/dashboard?StartDate=2024-11-01&EndDate=2024-11-30&Period=daily
GET /api/BusinessAnalytics/comparison?startDate=2024-11-01&endDate=2024-11-30
```

---

### UC2: Vehicle Fleet Optimization

**Actor:** Fleet Manager

**Flow:**

1. Accesses vehicle utilization analytics
2. Identifies underperforming vehicles (low utilization rate)
3. Reviews top performing vehicle types
4. Makes decisions:
   - Retire/sell low-performing vehicles
   - Purchase more of high-demand types
   - Redistribute vehicles across branches

**API Call:**

```http
GET /api/BusinessAnalytics/vehicle-utilization?StartDate=2024-01-01&EndDate=2024-11-30
```

**Key Metrics:**

- Vehicles with utilization < 30% → Consider retirement
- Vehicle types with utilization > 80% → Consider expansion
- Branch-wise distribution analysis

---

### UC3: Customer Retention Campaign

**Actor:** Marketing Manager

**Flow:**

1. Accesses customer analytics
2. Identifies customer segments
3. Analyzes inactive customers
4. Creates targeted campaigns:
   - Re-engagement offers for inactive customers
   - Loyalty rewards for VIP customers
   - Welcome discounts for new customers

**API Call:**

```http
GET /api/BusinessAnalytics/customer?StartDate=2024-01-01&EndDate=2024-11-30
```

---

### UC4: Revenue Forecast & Planning

**Actor:** Finance Manager

**Flow:**

1. Reviews revenue by period (monthly)
2. Analyzes payment method preferences
3. Identifies revenue trends by vehicle type
4. Creates financial forecasts and budgets

**API Calls:**

```http
GET /api/BusinessAnalytics/revenue?StartDate=2024-01-01&EndDate=2024-11-30&Period=monthly
GET /api/BusinessAnalytics/comparison?startDate=2024-11-01&endDate=2024-11-30
```

---

### UC5: Daily Operations Dashboard

**Actor:** Operations Staff

**Flow:**

1. Opens daily summary dashboard
2. Monitors real-time metrics:
   - Active rentals
   - Available vehicles
   - Today's revenue
   - Customer activity
3. Takes action on anomalies

**API Call:**

```http
GET /api/BusinessAnalytics/summary?StartDate=2024-11-30&EndDate=2024-11-30
```

---

## Error Responses

### 401 Unauthorized

```json
{
  "success": false,
  "message": "Only Manager can access analytics"
}
```

### 400 Bad Request

```json
{
  "success": false,
  "message": "Start date must be before end date"
}
```

### 500 Internal Server Error

```json
{
  "success": false,
  "message": "Error retrieving revenue analytics",
  "error": "Connection to RentalPayment service failed"
}
```

---

## Performance Considerations

### Caching Strategy

Recommended caching for high-traffic endpoints:

- **Dashboard Summary**: Cache for 5 minutes
- **Revenue Analytics**: Cache for 15 minutes
- **Customer Analytics**: Cache for 30 minutes
- **Period Comparison**: Cache for 1 hour

### Query Optimization

- Use date range filters to limit data volume
- Parallel API calls to microservices
- Async/await pattern for non-blocking operations
- Connection pooling for HTTP clients

### Response Time Targets

- Quick Summary: < 500ms
- Single Analytics: < 2s
- Full Dashboard: < 5s
- Period Comparison: < 3s

---

## Testing Scenarios

### Test 1: Revenue Analytics with No Data

**Request:**

```http
GET /api/BusinessAnalytics/revenue?StartDate=2025-01-01&EndDate=2025-01-31
```

**Expected:** Returns zero values with proper structure

### Test 2: Invalid Date Range

**Request:**

```http
GET /api/BusinessAnalytics/comparison?startDate=2024-12-01&endDate=2024-11-01
```

**Expected:** 400 Bad Request - "Start date must be before end date"

### Test 3: Staff Access to Manager-Only Endpoint

**Request:**

```http
GET /api/BusinessAnalytics/revenue
X-User-Role: Staff
```

**Expected:** 401 Unauthorized

### Test 4: Large Date Range Performance

**Request:**

```http
GET /api/BusinessAnalytics/dashboard?StartDate=2020-01-01&EndDate=2024-12-31
```

**Expected:** Response within 10 seconds, paginated if needed

---

## Best Practices

### 1. Date Range Selection

- **Daily**: For operational monitoring
- **Weekly**: For trend analysis
- **Monthly**: For business reviews
- **Yearly**: For strategic planning

### 2. Filter Combinations

```http
# Analyze specific branch performance
GET /api/BusinessAnalytics/revenue?BranchId=branch-001&StartDate=2024-11-01

# Analyze vehicle type profitability
GET /api/BusinessAnalytics/revenue?VehicleType=type-001&Period=monthly

# Combined filters for detailed analysis
GET /api/BusinessAnalytics/revenue?BranchId=branch-001&VehicleType=type-001&StartDate=2024-11-01&EndDate=2024-11-30
```

### 3. Error Handling

Always check `success` field before processing data:

```javascript
const response = await fetch('/api/BusinessAnalytics/dashboard')
const result = await response.json()

if (!result.success) {
  console.error(result.message)
  return
}

// Process result.data
```

### 4. Microservice Resilience

The analytics service implements:

- Circuit breaker pattern via Polly
- Retry policies for transient failures
- Graceful degradation (returns partial data if one service fails)

---

## Future Enhancements

### Phase 2 Features

- [ ] Real-time analytics via SignalR
- [ ] Export to Excel/PDF
- [ ] Scheduled reports via email
- [ ] Custom KPI definitions
- [ ] Predictive analytics (ML-based forecasting)
- [ ] Anomaly detection
- [ ] Geographic heat maps
- [ ] Mobile app integration

### Phase 3 Features

- [ ] Advanced filtering (multi-select, ranges)
- [ ] Saved report templates
- [ ] Role-based dashboards
- [ ] API rate limiting
- [ ] GraphQL support
- [ ] Webhook notifications for threshold alerts

---

## Support & Documentation

For technical support or questions about the Business Analytics API:

- API Documentation: `/swagger`
- Technical Contact: dev-team@example.com
- Issue Tracker: GitHub Issues

---

## Changelog

### Version 1.0.0 (2024-11-30)

- Initial release
- 7 analytics endpoints
- Integration with 3 microservices
- Comprehensive business metrics
- Gateway authentication support
- Manager/Staff role-based access
