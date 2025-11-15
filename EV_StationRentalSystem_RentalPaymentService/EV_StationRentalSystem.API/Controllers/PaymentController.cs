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
    /// Controller xử lý thanh toán cho Renter
    /// Phụ trách: Thành viên 1 - Renter Experience
    /// </summary>
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
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
                var result = await _paymentService.CreatePaymentAsync(request);
                return Ok(new 
                { 
                    success = true, 
                    message = "Tạo thanh toán thành công", 
                    data = result 
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Tạo thanh toán thất bại", error = ex.Message });
            }
        }

        // GET: api/payments - Lấy tất cả thanh toán (Admin/Staff)
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPayments(
            [FromQuery] string? status = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _paymentService.GetAllPaymentsAsync();
                
                // Apply filters
                var filteredResult = result.AsQueryable();
                
                if (!string.IsNullOrEmpty(status))
                    filteredResult = filteredResult.Where(p => p.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
                
                if (fromDate.HasValue)
                    filteredResult = filteredResult.Where(p => p.PaymentTime >= fromDate.Value);
                
                if (toDate.HasValue)
                    filteredResult = filteredResult.Where(p => p.PaymentTime <= toDate.Value);

                var pagedResult = filteredResult
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(new 
                { 
                    success = true,
                    totalRecords = filteredResult.Count(),
                    totalAmount = filteredResult.Sum(p => p.Amount),
                    page = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling(filteredResult.Count() / (double)pageSize),
                    filters = new { status, fromDate, toDate },
                    data = pagedResult 
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Kh�ng th? l?y danh s�ch thanh to�n", error = ex.Message });
            }
        }

        // GET: api/payments/{paymentId} - Lấy chi tiết thanh toán
        [HttpGet("{paymentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaymentById(string paymentId)
        {
            if (!Guid.TryParse(paymentId, out var guid))
            {
                return Ok(new { success = false, message = "M� thanh to�n kh�ng h?p l?" });
            }

            var result = await _paymentService.GetPaymentByIdAsync(guid);
            
            if (result == null)
            {
                return Ok(new { success = false, message = "Kh�ng t�m th?y thanh to�n" });
            }

            return Ok(new { success = true, data = result });
        }

        // GET: api/payments/rental/{rentalId} - Lấy tất cả thanh toán của 1 đơn thuê
        [HttpGet("rental/{rentalId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaymentsByRentalId(string rentalId)
        {
            if (!Guid.TryParse(rentalId, out var guid))
            {
                return Ok(new { success = false, message = "M� don thu� kh�ng h?p l?" });
            }

            try
            {
                var result = await _paymentService.GetPaymentsByRentalIdAsync(guid);
                return Ok(new 
                { 
                    success = true,
                    rentalId = rentalId,
                    totalPayments = result.Count,
                    totalAmount = result.Sum(p => p.Amount),
                    data = result 
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Kh�ng th? l?y danh s�ch thanh to�n", error = ex.Message });
            }
        }

        // GET: api/payments/renter/{renterId} - Lấy lịch sử thanh toán của Renter
        [HttpGet("renter/{renterId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaymentsByRenterId(
            string renterId,
            [FromQuery] string? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (!Guid.TryParse(renterId, out var guid))
            {
                return Ok(new { success = false, message = "M� ngu?i thu� kh�ng h?p l?" });
            }

            try
            {
                var result = await _paymentService.GetPaymentsByRenterIdAsync(guid);
                
                // Apply filters
                if (!string.IsNullOrEmpty(status))
                    result = result.Where(p => p.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();

                var pagedResult = result
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(new 
                { 
                    success = true,
                    renterId = renterId,
                    totalPayments = result.Count,
                    totalAmount = result.Sum(p => p.Amount),
                    page = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling(result.Count / (double)pageSize),
                    filters = new { status },
                    data = pagedResult 
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Kh�ng th? l?y l?ch s? thanh to�n", error = ex.Message });
            }
        }
    }
}

