using EV_StationRentalSystem.Core.DTO.Request;
using EV_StationRentalSystem.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.API.Controllers
{
    /// <summary>
    /// Controller xử lý các chức năng đặt xe và quản lý đơn thuê cho Renter
    /// Phụ trách: Thành viên 1 - Renter Experience
    /// </summary>
    [Route("api/rentals")]
    [ApiController]
    public class RentalOrderController : ControllerBase
    {
        private readonly IRentalOrderService _rentalOrderService;

        public RentalOrderController(IRentalOrderService rentalOrderService)
        {
            _rentalOrderService = rentalOrderService;
        }

        // POST: api/rentals - Đặt xe (Booking)
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateRentalOrder([FromBody] CreateRentalOrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new 
                { 
                    success = false, 
                    message = "Dữ liệu không hợp lệ", 
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) 
                });
            }

            try
            {
                var result = await _rentalOrderService.CreateRentalOrderAsync(request);
                return Ok(new { success = true, message = "Đặt xe thành công", data = result });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Đặt xe thất bại", error = ex.Message });
            }
        }

        // GET: api/rentals - Lấy tất cả đơn thuê (Admin/Staff)
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllRentalOrders(
            [FromQuery] string? status = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _rentalOrderService.GetAllRentalOrdersAsync();
                
                // Apply filters
                var filteredResult = result.AsQueryable();
                
                if (!string.IsNullOrEmpty(status))
                    filteredResult = filteredResult.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
                
                if (fromDate.HasValue)
                    filteredResult = filteredResult.Where(r => r.StartTime >= fromDate.Value);
                
                if (toDate.HasValue)
                    filteredResult = filteredResult.Where(r => r.StartTime <= toDate.Value);

                var pagedResult = filteredResult
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(new 
                { 
                    success = true,
                    totalRecords = filteredResult.Count(),
                    page = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling(filteredResult.Count() / (double)pageSize),
                    filters = new { status, fromDate, toDate },
                    data = pagedResult 
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Không thể lấy danh sách đơn thuê", error = ex.Message });
            }
        }

        // GET: api/rentals/{rentalId} - Lấy chi tiết đơn thuê
        [HttpGet("{rentalId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRentalOrderById(string rentalId)
        {
            if (!Guid.TryParse(rentalId, out var guid))
            {
                return Ok(new { success = false, message = "Mã đơn thuê không hợp lệ" });
            }

            var result = await _rentalOrderService.GetRentalOrderByIdAsync(guid);
            
            if (result == null)
            {
                return Ok(new { success = false, message = "Không tìm thấy đơn thuê" });
            }

            return Ok(new { success = true, message = "Lấy thông tin đơn thuê thành công", data = result });
        }

        // GET: api/rentals/renter/{renterId} - Lấy lịch sử thuê xe của Renter
        [HttpGet("renter/{renterId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRentalHistory(
            string renterId, 
            [FromQuery] string? status = null, 
            [FromQuery] DateTime? fromDate = null, 
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (!Guid.TryParse(renterId, out var guid))
            {
                return Ok(new { success = false, message = "Mã người thuê không hợp lệ" });
            }

            try
            {
                var result = await _rentalOrderService.GetRentalHistoryByRenterIdAsync(guid, status, fromDate, toDate);
                
                var pagedResult = result
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(new 
                { 
                    success = true,
                    message = "Lấy lịch sử thuê xe thành công",
                    renterId = renterId,
                    totalOrders = result.Count,
                    page = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling(result.Count / (double)pageSize),
                    filters = new 
                    {
                        status = status,
                        fromDate = fromDate,
                        toDate = toDate
                    },
                    data = pagedResult 
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Không thể lấy lịch sử thuê xe", error = ex.Message });
            }
        }

        // PUT: api/rentals/{rentalId}/cancel - Hủy đơn thuê
        [HttpPut("{rentalId}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CancelRentalOrder(string rentalId, [FromBody] CancelRentalRequest request)
        {
            if (!Guid.TryParse(rentalId, out var guid))
            {
                return Ok(new { success = false, message = "Mã đơn thuê không hợp lệ" });
            }

            if (!ModelState.IsValid)
            {
                return Ok(new 
                { 
                    success = false, 
                    message = "Dữ liệu không hợp lệ", 
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) 
                });
            }

            try
            {
                var result = await _rentalOrderService.CancelRentalOrderAsync(guid, request.Reason);
                if (!result)
                {
                    return Ok(new { success = false, message = "Không tìm thấy đơn thuê" });
                }

                return Ok(new 
                { 
                    success = true, 
                    message = "Hủy đơn thuê thành công", 
                    rentalId = rentalId,
                    reason = request.Reason,
                    cancelledAt = DateTime.UtcNow
                });
            }
            catch (InvalidOperationException ex)
            {
                // Trả về message cụ thể từ service
                return Ok(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Có lỗi xảy ra khi hủy đơn thuê", error = ex.Message });
            }
        }

        // PUT /api/rental-orders/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
        {
            var result = await _rentalOrderService.UpdateStatusAsync(id, request.Status);
            return CreatedAtAction(nameof(GetRentalOrderById), new { rentalId = result.RentalId }, result);
        }

        // GET /api/rental-orders/{id}/details
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetDetails(Guid id)
        {
            var details = await _rentalOrderService.GetOrderDetailsAsync(id);
            return Ok(details);
        }
    }
}
