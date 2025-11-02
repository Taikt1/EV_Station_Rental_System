using EV_StationRentalSystem.Core.DTO.Response;
using System;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.ServiceContracts
{
    /// <summary>
    /// Service cho phân tích và thống kê dữ liệu Renter
    /// </summary>
    public interface IAnalyticsService
    {
        /// <summary>
        /// Lấy phân tích tổng quan của Renter
        /// </summary>
        Task<RenterAnalyticsResponse> GetRenterAnalyticsAsync(Guid renterId, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
