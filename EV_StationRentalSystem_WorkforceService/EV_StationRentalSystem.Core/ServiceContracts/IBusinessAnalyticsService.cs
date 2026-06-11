using EV_StationRentalSystem.Core.DTO;
using System;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.ServiceContracts
{
    public interface IBusinessAnalyticsService
    {
        /// <summary>
        /// Get comprehensive revenue analytics
        /// </summary>
        Task<RevenueAnalyticsDTO> GetRevenueAnalyticsAsync(AnalyticsFilterRequest filter);

        /// <summary>
        /// Get vehicle utilization analytics
        /// </summary>
        Task<VehicleUtilizationDTO> GetVehicleUtilizationAnalyticsAsync(AnalyticsFilterRequest filter);

        /// <summary>
        /// Get customer analytics
        /// </summary>
        Task<CustomerAnalyticsDTO> GetCustomerAnalyticsAsync(AnalyticsFilterRequest filter);

        /// <summary>
        /// Get operational metrics
        /// </summary>
        Task<OperationalMetricsDTO> GetOperationalMetricsAsync(AnalyticsFilterRequest filter);

        /// <summary>
        /// Get complete business dashboard
        /// </summary>
        Task<BusinessDashboardDTO> GetBusinessDashboardAsync(AnalyticsFilterRequest filter);

        /// <summary>
        /// Get period comparison analytics
        /// </summary>
        Task<PeriodComparisonDTO> GetPeriodComparisonAsync(DateTime startDate, DateTime endDate);
    }
}
