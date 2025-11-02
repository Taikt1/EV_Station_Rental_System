using EV_StationRentalSystem.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.API.Controllers
{
    /// <summary>
    /// Controller xử lý phân tích và thống kê cá nhân cho Renter
    /// Phụ trách: Thành viên 1 - Renter Experience
    /// </summary>
    [Route("api/analytics")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        // GET: api/analytics/renter/{renterId} - Phân tích tổng quan của Renter
        [HttpGet("renter/{renterId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRenterAnalytics(
            string renterId,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            if (!Guid.TryParse(renterId, out var guid))
            {
                return Ok(new { success = false, message = "Mã người thuê không hợp lệ" });
            }

            try
            {
                var result = await _analyticsService.GetRenterAnalyticsAsync(guid, fromDate, toDate);
                return Ok(new 
                { 
                    success = true,
                    message = "Lấy thống kê thành công",
                    filters = new { fromDate, toDate },
                    data = result 
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Không thể lấy thống kê", error = ex.Message });
            }
        }

        // GET: api/analytics/renter/{renterId}/summary - Tóm tắt nhanh thống kê
        [HttpGet("renter/{renterId}/summary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRenterSummary(string renterId)
        {
            if (!Guid.TryParse(renterId, out var guid))
            {
                return Ok(new { success = false, message = "Mã người thuê không hợp lệ" });
            }

            try
            {
                var result = await _analyticsService.GetRenterAnalyticsAsync(guid);
                
                // Trả về summary đơn giản
                var summary = new
                {
                    totalRentals = result.TotalRentals,
                    completedRentals = result.CompletedRentals,
                    activeRentals = result.ActiveRentals,
                    totalSpent = result.TotalSpent,
                    averageRating = result.AverageRating,
                    totalRentalHours = result.TotalRentalHours
                };

                return Ok(new 
                { 
                    success = true,
                    renterId = renterId,
                    data = summary 
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Không thể lấy tổng quan", error = ex.Message });
            }
        }
    }
}

