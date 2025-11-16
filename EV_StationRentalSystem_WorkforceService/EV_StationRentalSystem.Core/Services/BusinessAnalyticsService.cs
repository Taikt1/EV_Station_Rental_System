using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.HttpClients;
using EV_StationRentalSystem.Core.ServiceContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Services
{
    public class BusinessAnalyticsService : IBusinessAnalyticsService
    {
        private readonly FleetMicroClient _fleetClient;
        private readonly RentalPaymentMicroClient _rentalPaymentClient;
        private readonly UserMicroClient _userClient;
        private readonly ILogger<BusinessAnalyticsService> _logger;

        public BusinessAnalyticsService(
            FleetMicroClient fleetClient,
            RentalPaymentMicroClient rentalPaymentClient,
            UserMicroClient userClient,
            ILogger<BusinessAnalyticsService> logger)
        {
            _fleetClient = fleetClient;
            _rentalPaymentClient = rentalPaymentClient;
            _userClient = userClient;
            _logger = logger;
        }

        public async Task<RevenueAnalyticsDTO> GetRevenueAnalyticsAsync(AnalyticsFilterRequest filter)
        {
            try
            {
                _logger.LogInformation("Fetching revenue analytics with filter: {Filter}", filter);

                // Fetch rental orders and payments from RentalPayment Service
                var rentals = await _rentalPaymentClient.GetAllRentalOrdersAsync(
                    filter.StartDate,
                    filter.EndDate,
                    null);

                var payments = await _rentalPaymentClient.GetAllPaymentsAsync(
                    filter.StartDate,
                    filter.EndDate);

                // Fetch vehicle types for categorization
                var vehicleTypes = await _fleetClient.GetAllVehicleTypesAsync();
                var vehicleTypeDict = vehicleTypes.ToDictionary(vt => vt.TypeId, vt => vt.TypeName);

                var result = new RevenueAnalyticsDTO
                {
                    TotalRentals = rentals.Count,
                    CompletedRentals = rentals.Count(r => r.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase)),
                    ActiveRentals = rentals.Count(r => r.Status.Equals("Active", StringComparison.OrdinalIgnoreCase)),
                    CancelledRentals = rentals.Count(r => r.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase)),
                };

                // Calculate revenue from payments
                result.TotalRevenue = payments.Sum(p => p.Amount);
                result.PaidRevenue = payments
                    .Where(p => p.Status.Equals("Paid", StringComparison.OrdinalIgnoreCase))
                    .Sum(p => p.Amount);
                result.PendingRevenue = payments
                    .Where(p => p.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                    .Sum(p => p.Amount);
                result.RefundedRevenue = payments
                    .Where(p => p.Status.Equals("Refunded", StringComparison.OrdinalIgnoreCase))
                    .Sum(p => p.Amount);

                result.AverageRevenuePerRental = result.TotalRentals > 0
                    ? result.TotalRevenue / result.TotalRentals
                    : 0;

                // Group revenue by period
                result.RevenueByPeriod = GroupRevenueByPeriod(rentals, payments, filter.Period ?? "monthly");

                // Group revenue by branch (if filter applied)
                result.RevenueByBranch = GroupRevenueByBranch(rentals, payments);

                // Group revenue by vehicle type
                result.RevenueByVehicleType = await GroupRevenueByVehicleType(rentals, payments, vehicleTypeDict);

                // Payment method breakdown
                result.PaymentMethodBreakdown = CalculatePaymentMethodBreakdown(payments);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching revenue analytics");
                throw;
            }
        }

        public async Task<VehicleUtilizationDTO> GetVehicleUtilizationAnalyticsAsync(AnalyticsFilterRequest filter)
        {
            try
            {
                _logger.LogInformation("Fetching vehicle utilization analytics");

                // Fetch all vehicles from Fleet Service
                var vehicles = await _fleetClient.GetAllVehiclesAsync();
                var vehicleTypes = await _fleetClient.GetAllVehicleTypesAsync();
                var vehicleTypeDict = vehicleTypes.ToDictionary(vt => vt.TypeId, vt => vt.TypeName);

                // Fetch rental orders
                var rentals = await _rentalPaymentClient.GetAllRentalOrdersAsync(
                    filter.StartDate,
                    filter.EndDate,
                    null);

                var result = new VehicleUtilizationDTO
                {
                    TotalVehicles = vehicles.Count,
                    AvailableVehicles = vehicles.Count(v => v.Status.Equals("Available", StringComparison.OrdinalIgnoreCase)),
                    InUseVehicles = vehicles.Count(v => v.Status.Equals("In-use", StringComparison.OrdinalIgnoreCase)),
                    MaintenanceVehicles = vehicles.Count(v => v.Status.Equals("Maintenance", StringComparison.OrdinalIgnoreCase)),
                };

                // Calculate total rental hours
                var totalRentalHours = 0;
                foreach (var rental in rentals.Where(r => r.EndTime.HasValue))
                {
                    var duration = (rental.EndTime.Value - rental.StartTime).TotalHours;
                    totalRentalHours += (int)duration;
                }

                result.TotalRentalHours = totalRentalHours;
                result.AverageRentalDurationHours = rentals.Count(r => r.EndTime.HasValue) > 0
                    ? (double)totalRentalHours / rentals.Count(r => r.EndTime.HasValue)
                    : 0;

                // Calculate overall utilization rate
                var totalPossibleHours = result.TotalVehicles * 24 * 30; // Assuming 30 days
                if (filter.StartDate.HasValue && filter.EndDate.HasValue)
                {
                    var days = (filter.EndDate.Value - filter.StartDate.Value).Days;
                    totalPossibleHours = result.TotalVehicles * 24 * days;
                }
                result.OverallUtilizationRate = totalPossibleHours > 0
                    ? (double)totalRentalHours / totalPossibleHours * 100
                    : 0;

                // Utilization by type
                result.UtilizationByType = CalculateUtilizationByType(vehicles, rentals, vehicleTypeDict);

                // Vehicle details
                result.VehicleDetails = await CalculateVehicleDetails(vehicles, rentals);

                // Top performing vehicles
                result.TopPerformingVehicles = result.VehicleDetails
                    .OrderByDescending(v => v.TotalRevenue)
                    .Take(10)
                    .Select(v => new TopPerformingVehicleDTO
                    {
                        VehicleId = v.VehicleId,
                        PlateNumber = v.PlateNumber,
                        VehicleType = v.VehicleType,
                        RentalCount = v.RentalCount,
                        TotalRevenue = v.TotalRevenue,
                        UtilizationRate = v.UtilizationRate,
                        AverageRating = 0 // Can be enhanced with feedback data
                    })
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching vehicle utilization analytics");
                throw;
            }
        }

        public async Task<CustomerAnalyticsDTO> GetCustomerAnalyticsAsync(AnalyticsFilterRequest filter)
        {
            try
            {
                _logger.LogInformation("Fetching customer analytics");

                // Fetch customers from User Service
                var customers = await _userClient.GetUsersByRoleAsync("Customer");

                // Fetch rental orders
                var rentals = await _rentalPaymentClient.GetAllRentalOrdersAsync(
                    filter.StartDate,
                    filter.EndDate,
                    null);

                // Fetch payments
                var payments = await _rentalPaymentClient.GetAllPaymentsAsync(
                    filter.StartDate,
                    filter.EndDate);

                var result = new CustomerAnalyticsDTO
                {
                    TotalCustomers = customers.Count,
                };

                // Calculate active customers (those with rentals)
                var activeCustomerIds = rentals.Select(r => r.RenterId).Distinct().ToList();
                result.ActiveCustomers = activeCustomerIds.Count;

                // New customers in period
                if (filter.StartDate.HasValue && filter.EndDate.HasValue)
                {
                    result.NewCustomersInPeriod = customers.Count(c =>
                        c.CreatedAt >= filter.StartDate.Value &&
                        c.CreatedAt <= filter.EndDate.Value);
                }

                // Customer retention rate
                result.CustomerRetentionRate = result.TotalCustomers > 0
                    ? (double)result.ActiveCustomers / result.TotalCustomers * 100
                    : 0;

                // Average rentals per customer
                result.AverageRentalsPerCustomer = result.ActiveCustomers > 0
                    ? (double)rentals.Count / result.ActiveCustomers
                    : 0;

                // Average customer lifetime value
                var totalRevenue = payments.Sum(p => p.Amount);
                result.AverageCustomerLifetimeValue = result.ActiveCustomers > 0
                    ? (double)totalRevenue / result.ActiveCustomers
                    : 0;

                // Top customers
                result.TopCustomers = CalculateTopCustomers(customers, rentals, payments);

                // Customer segments
                result.CustomerSegments = CalculateCustomerSegments(customers, rentals, payments);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer analytics");
                throw;
            }
        }

        public async Task<OperationalMetricsDTO> GetOperationalMetricsAsync(AnalyticsFilterRequest filter)
        {
            try
            {
                _logger.LogInformation("Fetching operational metrics");

                var result = new OperationalMetricsDTO
                {
                    AverageCheckInTime = 15.5, // Mock data - can be enhanced with real data
                    AverageCheckOutTime = 12.3,
                    TotalPenalties = 0,
                    TotalPenaltyAmount = 0,
                    MaintenanceIssuesReported = 0,
                    AverageCustomerRating = 4.5,
                    TotalFeedbacks = 0,
                    PositiveFeedbacks = 0,
                    NegativeFeedbacks = 0,
                    IssueBreakdown = new List<IssueBreakdownDTO>()
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching operational metrics");
                throw;
            }
        }

        public async Task<BusinessDashboardDTO> GetBusinessDashboardAsync(AnalyticsFilterRequest filter)
        {
            try
            {
                _logger.LogInformation("Fetching complete business dashboard");

                var dashboard = new BusinessDashboardDTO
                {
                    GeneratedAt = DateTime.UtcNow,
                    Filters = filter,
                    RevenueAnalytics = await GetRevenueAnalyticsAsync(filter),
                    VehicleUtilization = await GetVehicleUtilizationAnalyticsAsync(filter),
                    CustomerAnalytics = await GetCustomerAnalyticsAsync(filter),
                    OperationalMetrics = await GetOperationalMetricsAsync(filter)
                };

                return dashboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching business dashboard");
                throw;
            }
        }

        public async Task<PeriodComparisonDTO> GetPeriodComparisonAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                _logger.LogInformation("Comparing periods: {Start} to {End}", startDate, endDate);

                var days = (endDate - startDate).Days;
                var previousStart = startDate.AddDays(-days);
                var previousEnd = startDate;

                var currentFilter = new AnalyticsFilterRequest
                {
                    StartDate = startDate,
                    EndDate = endDate
                };

                var previousFilter = new AnalyticsFilterRequest
                {
                    StartDate = previousStart,
                    EndDate = previousEnd
                };

                var currentAnalytics = await GetRevenueAnalyticsAsync(currentFilter);
                var previousAnalytics = await GetRevenueAnalyticsAsync(previousFilter);
                var currentUtilization = await GetVehicleUtilizationAnalyticsAsync(currentFilter);
                var previousUtilization = await GetVehicleUtilizationAnalyticsAsync(previousFilter);

                var comparison = new PeriodComparisonDTO
                {
                    CurrentPeriod = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
                    PreviousPeriod = $"{previousStart:yyyy-MM-dd} to {previousEnd:yyyy-MM-dd}",
                    CurrentRevenue = currentAnalytics.TotalRevenue,
                    PreviousRevenue = previousAnalytics.TotalRevenue,
                    CurrentRentals = currentAnalytics.TotalRentals,
                    PreviousRentals = previousAnalytics.TotalRentals,
                    CurrentUtilizationRate = currentUtilization.OverallUtilizationRate,
                    PreviousUtilizationRate = previousUtilization.OverallUtilizationRate
                };

                // Calculate growth percentages
                comparison.RevenueGrowthPercentage = previousAnalytics.TotalRevenue > 0
                    ? (double)(currentAnalytics.TotalRevenue - previousAnalytics.TotalRevenue) / (double)previousAnalytics.TotalRevenue * 100
                    : 0;

                comparison.RentalGrowthPercentage = previousAnalytics.TotalRentals > 0
                    ? (double)(currentAnalytics.TotalRentals - previousAnalytics.TotalRentals) / previousAnalytics.TotalRentals * 100
                    : 0;

                comparison.UtilizationChange = currentUtilization.OverallUtilizationRate - previousUtilization.OverallUtilizationRate;

                return comparison;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing periods");
                throw;
            }
        }

        // Helper methods
        private List<RevenueByPeriodDTO> GroupRevenueByPeriod(
            List<RentalOrderDataDTO> rentals,
            List<PaymentDataDTO> payments,
            string period)
        {
            var result = new List<RevenueByPeriodDTO>();

            var groupedPayments = payments.GroupBy(p =>
            {
                return period.ToLower() switch
                {
                    "daily" => p.PaymentTime.ToString("yyyy-MM-dd"),
                    "weekly" => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                        p.PaymentTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString(),
                    "yearly" => p.PaymentTime.ToString("yyyy"),
                    _ => p.PaymentTime.ToString("yyyy-MM") // monthly
                };
            });

            foreach (var group in groupedPayments)
            {
                var periodRevenue = group.Sum(p => p.Amount);
                var rentalCount = rentals.Count(r =>
                    FormatPeriod(r.StartTime, period) == group.Key);

                result.Add(new RevenueByPeriodDTO
                {
                    Period = group.Key,
                    Revenue = periodRevenue,
                    RentalCount = rentalCount,
                    AverageRevenue = rentalCount > 0 ? periodRevenue / rentalCount : 0
                });
            }

            return result.OrderBy(r => r.Period).ToList();
        }

        private string FormatPeriod(DateTime date, string period)
        {
            return period.ToLower() switch
            {
                "daily" => date.ToString("yyyy-MM-dd"),
                "weekly" => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                    date, CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString(),
                "yearly" => date.ToString("yyyy"),
                _ => date.ToString("yyyy-MM")
            };
        }

        private List<RevenueByBranchDTO> GroupRevenueByBranch(
            List<RentalOrderDataDTO> rentals,
            List<PaymentDataDTO> payments)
        {
            var result = new List<RevenueByBranchDTO>();
            var totalRevenue = payments.Sum(p => p.Amount);

            var branchGroups = rentals.GroupBy(r => r.BranchStartId);

            foreach (var group in branchGroups)
            {
                var rentalIds = group.Select(r => r.RentalId).ToList();
                var branchRevenue = payments
                    .Where(p => rentalIds.Contains(p.RentalId))
                    .Sum(p => p.Amount);

                result.Add(new RevenueByBranchDTO
                {
                    BranchId = group.Key,
                    BranchName = $"Branch {group.Key}", // Can be enhanced with actual branch names
                    Revenue = branchRevenue,
                    RentalCount = group.Count(),
                    Percentage = totalRevenue > 0 ? (double)(branchRevenue / totalRevenue) * 100 : 0
                });
            }

            return result.OrderByDescending(r => r.Revenue).ToList();
        }

        private async Task<List<RevenueByVehicleTypeDTO>> GroupRevenueByVehicleType(
            List<RentalOrderDataDTO> rentals,
            List<PaymentDataDTO> payments,
            Dictionary<string, string> vehicleTypeDict)
        {
            var result = new List<RevenueByVehicleTypeDTO>();
            var totalRevenue = payments.Sum(p => p.Amount);

            // Get vehicle data to map vehicle to type
            var vehicles = await _fleetClient.GetAllVehiclesAsync();
            var vehicleToType = vehicles.ToDictionary(v => v.VehicleId, v => v.TypeId);

            var typeGroups = rentals
                .Where(r => vehicleToType.ContainsKey(r.VehicleId))
                .GroupBy(r => vehicleToType[r.VehicleId]);

            foreach (var group in typeGroups)
            {
                var rentalIds = group.Select(r => r.RentalId).ToList();
                var typeRevenue = payments
                    .Where(p => rentalIds.Contains(p.RentalId))
                    .Sum(p => p.Amount);

                var typeName = vehicleTypeDict.ContainsKey(group.Key)
                    ? vehicleTypeDict[group.Key]
                    : "Unknown";

                result.Add(new RevenueByVehicleTypeDTO
                {
                    VehicleTypeId = group.Key,
                    VehicleTypeName = typeName,
                    Revenue = typeRevenue,
                    RentalCount = group.Count(),
                    Percentage = totalRevenue > 0 ? (double)(typeRevenue / totalRevenue) * 100 : 0
                });
            }

            return result.OrderByDescending(r => r.Revenue).ToList();
        }

        private PaymentMethodBreakdownDTO CalculatePaymentMethodBreakdown(List<PaymentDataDTO> payments)
        {
            var breakdown = new PaymentMethodBreakdownDTO();

            var paidPayments = payments.Where(p => p.Status.Equals("Paid", StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var payment in paidPayments)
            {
                var method = payment.PaymentMethod?.ToLower() ?? "unknown";
                if (method.Contains("cash"))
                {
                    breakdown.CashAmount += payment.Amount;
                    breakdown.CashCount++;
                }
                else if (method.Contains("card"))
                {
                    breakdown.CardAmount += payment.Amount;
                    breakdown.CardCount++;
                }
                else if (method.Contains("wallet") || method.Contains("momo") || method.Contains("zalopay"))
                {
                    breakdown.EWalletAmount += payment.Amount;
                    breakdown.EWalletCount++;
                }
                else if (method.Contains("bank") || method.Contains("transfer"))
                {
                    breakdown.BankTransferAmount += payment.Amount;
                    breakdown.BankTransferCount++;
                }
            }

            return breakdown;
        }

        private List<VehicleUtilizationByTypeDTO> CalculateUtilizationByType(
            List<VehicleDataDTO> vehicles,
            List<RentalOrderDataDTO> rentals,
            Dictionary<string, string> vehicleTypeDict)
        {
            var result = new List<VehicleUtilizationByTypeDTO>();

            var typeGroups = vehicles.GroupBy(v => v.TypeId);

            foreach (var group in typeGroups)
            {
                var typeVehicles = group.ToList();
                var typeRentals = rentals.Where(r => typeVehicles.Any(v => v.VehicleId == r.VehicleId)).ToList();

                var totalRentalHours = 0;
                foreach (var rental in typeRentals.Where(r => r.EndTime.HasValue))
                {
                    var duration = (rental.EndTime.Value - rental.StartTime).TotalHours;
                    totalRentalHours += (int)duration;
                }

                var totalPossibleHours = typeVehicles.Count * 24 * 30; // 30 days
                var utilizationRate = totalPossibleHours > 0
                    ? (double)totalRentalHours / totalPossibleHours * 100
                    : 0;

                var typeName = vehicleTypeDict.ContainsKey(group.Key)
                    ? vehicleTypeDict[group.Key]
                    : "Unknown";

                result.Add(new VehicleUtilizationByTypeDTO
                {
                    VehicleTypeId = group.Key,
                    VehicleTypeName = typeName,
                    TotalVehicles = typeVehicles.Count,
                    AvailableVehicles = typeVehicles.Count(v => v.Status.Equals("Available", StringComparison.OrdinalIgnoreCase)),
                    InUseVehicles = typeVehicles.Count(v => v.Status.Equals("In-use", StringComparison.OrdinalIgnoreCase)),
                    MaintenanceVehicles = typeVehicles.Count(v => v.Status.Equals("Maintenance", StringComparison.OrdinalIgnoreCase)),
                    UtilizationRate = utilizationRate,
                    TotalRentals = typeRentals.Count,
                    TotalRentalHours = totalRentalHours
                });
            }

            return result.OrderByDescending(r => r.UtilizationRate).ToList();
        }

        private async Task<List<VehicleUtilizationDetailDTO>> CalculateVehicleDetails(
            List<VehicleDataDTO> vehicles,
            List<RentalOrderDataDTO> rentals)
        {
            var result = new List<VehicleUtilizationDetailDTO>();
            var payments = await _rentalPaymentClient.GetAllPaymentsAsync(null, null);

            foreach (var vehicle in vehicles)
            {
                var vehicleRentals = rentals.Where(r => r.VehicleId == vehicle.VehicleId).ToList();

                var rentalHours = 0;
                foreach (var rental in vehicleRentals.Where(r => r.EndTime.HasValue))
                {
                    var duration = (rental.EndTime.Value - rental.StartTime).TotalHours;
                    rentalHours += (int)duration;
                }

                var totalPossibleHours = 24 * 30; // 30 days
                var utilizationRate = totalPossibleHours > 0
                    ? (double)rentalHours / totalPossibleHours * 100
                    : 0;

                var rentalIds = vehicleRentals.Select(r => r.RentalId).ToList();
                var totalRevenue = payments
                    .Where(p => rentalIds.Contains(p.RentalId))
                    .Sum(p => p.Amount);

                var lastRental = vehicleRentals.OrderByDescending(r => r.StartTime).FirstOrDefault();

                result.Add(new VehicleUtilizationDetailDTO
                {
                    VehicleId = vehicle.VehicleId,
                    PlateNumber = vehicle.PlateNumber,
                    VehicleType = vehicle.TypeName,
                    Status = vehicle.Status,
                    RentalCount = vehicleRentals.Count,
                    RentalHours = rentalHours,
                    UtilizationRate = utilizationRate,
                    TotalRevenue = totalRevenue,
                    LastRentalDate = lastRental?.StartTime
                });
            }

            return result.OrderByDescending(v => v.UtilizationRate).ToList();
        }

        private List<TopCustomerDTO> CalculateTopCustomers(
            List<UserDataDTO> customers,
            List<RentalOrderDataDTO> rentals,
            List<PaymentDataDTO> payments)
        {
            var result = new List<TopCustomerDTO>();

            var customerGroups = rentals.GroupBy(r => r.RenterId);

            foreach (var group in customerGroups.Take(20))
            {
                var customer = customers.FirstOrDefault(c => c.UserId == group.Key);
                if (customer == null) continue;

                var rentalIds = group.Select(r => r.RentalId).ToList();
                var totalSpent = payments
                    .Where(p => rentalIds.Contains(p.RentalId))
                    .Sum(p => p.Amount);

                var lastRental = group.OrderByDescending(r => r.StartTime).FirstOrDefault();

                result.Add(new TopCustomerDTO
                {
                    CustomerId = customer.UserId,
                    CustomerName = customer.FullName,
                    Email = customer.Email,
                    TotalRentals = group.Count(),
                    TotalSpent = totalSpent,
                    AverageRating = 4.5, // Can be enhanced with feedback data
                    LastRentalDate = lastRental?.StartTime ?? DateTime.MinValue
                });
            }

            return result.OrderByDescending(c => c.TotalSpent).ToList();
        }

        private List<CustomerSegmentDTO> CalculateCustomerSegments(
            List<UserDataDTO> customers,
            List<RentalOrderDataDTO> rentals,
            List<PaymentDataDTO> payments)
        {
            var result = new List<CustomerSegmentDTO>();

            var customerRentals = rentals.GroupBy(r => r.RenterId).ToDictionary(g => g.Key, g => g.Count());
            var customerSpending = new Dictionary<string, decimal>();

            foreach (var group in rentals.GroupBy(r => r.RenterId))
            {
                var rentalIds = group.Select(r => r.RentalId).ToList();
                var spending = payments
                    .Where(p => rentalIds.Contains(p.RentalId))
                    .Sum(p => p.Amount);
                customerSpending[group.Key] = spending;
            }

            // Segment customers
            var newCustomers = customers.Where(c => !customerRentals.ContainsKey(c.UserId) ||
                customerRentals[c.UserId] <= 1).ToList();
            var regularCustomers = customers.Where(c => customerRentals.ContainsKey(c.UserId) &&
                customerRentals[c.UserId] > 1 && customerRentals[c.UserId] <= 5).ToList();
            var vipCustomers = customers.Where(c => customerRentals.ContainsKey(c.UserId) &&
                customerRentals[c.UserId] > 5).ToList();
            var inactiveCustomers = customers.Where(c => !customerRentals.ContainsKey(c.UserId)).ToList();

            result.Add(new CustomerSegmentDTO
            {
                Segment = "New",
                CustomerCount = newCustomers.Count,
                Percentage = customers.Count > 0 ? (double)newCustomers.Count / customers.Count * 100 : 0,
                AverageSpending = newCustomers.Count > 0
                    ? newCustomers.Where(c => customerSpending.ContainsKey(c.UserId))
                        .Average(c => customerSpending[c.UserId])
                    : 0
            });

            result.Add(new CustomerSegmentDTO
            {
                Segment = "Regular",
                CustomerCount = regularCustomers.Count,
                Percentage = customers.Count > 0 ? (double)regularCustomers.Count / customers.Count * 100 : 0,
                AverageSpending = regularCustomers.Count > 0
                    ? regularCustomers.Where(c => customerSpending.ContainsKey(c.UserId))
                        .Average(c => customerSpending[c.UserId])
                    : 0
            });

            result.Add(new CustomerSegmentDTO
            {
                Segment = "VIP",
                CustomerCount = vipCustomers.Count,
                Percentage = customers.Count > 0 ? (double)vipCustomers.Count / customers.Count * 100 : 0,
                AverageSpending = vipCustomers.Count > 0
                    ? vipCustomers.Where(c => customerSpending.ContainsKey(c.UserId))
                        .Average(c => customerSpending[c.UserId])
                    : 0
            });

            result.Add(new CustomerSegmentDTO
            {
                Segment = "Inactive",
                CustomerCount = inactiveCustomers.Count,
                Percentage = customers.Count > 0 ? (double)inactiveCustomers.Count / customers.Count * 100 : 0,
                AverageSpending = 0
            });

            return result;
        }
    }
}
