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
    /// Controller xá»­ lÃ½ thanh toÃ¡n cho Renter
    /// Phá»¥ trÃ¡ch: ThÃ nh viÃªn 1 - Renter Experience
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

        // POST: api/payments - Táº¡o thanh toÃ¡n
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new 
                { 
                    success = false, 
                    message = "D? li?u không h?p l?", 
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) 
                });
            }

            try
            {
                var result = await _paymentService.CreatePaymentAsync(request);
                return CreatedAtAction(
                    nameof(GetPaymentById), 
                    new { paymentId = result.PaymentId }, 
                    new { success = true, message = "T?o thanh toán thành công", data = result }
                );
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "T?o thanh toán th?t b?i", error = ex.Message });
            }
        }

        // GET: api/payments - Láº¥y táº¥t cáº£ thanh toÃ¡n (Admin/Staff)
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
                return Ok(new { success = false, message = "Không th? l?y danh sách thanh toán", error = ex.Message });
            }
        }

        // GET: api/payments/{paymentId} - Láº¥y chi tiáº¿t thanh toÃ¡n
        [HttpGet("{paymentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaymentById(string paymentId)
        {
            if (!Guid.TryParse(paymentId, out var guid))
            {
                return Ok(new { success = false, message = "Mã thanh toán không h?p l?" });
            }

            var result = await _paymentService.GetPaymentByIdAsync(guid);
            
            if (result == null)
            {
                return Ok(new { success = false, message = "Không tìm th?y thanh toán" });
            }

            return Ok(new { success = true, data = result });
        }

        // GET: api/payments/rental/{rentalId} - Láº¥y táº¥t cáº£ thanh toÃ¡n cá»§a 1 Ä‘Æ¡n thuÃª
        [HttpGet("rental/{rentalId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaymentsByRentalId(string rentalId)
        {
            if (!Guid.TryParse(rentalId, out var guid))
            {
                return Ok(new { success = false, message = "Mã don thuê không h?p l?" });
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
                return Ok(new { success = false, message = "Không th? l?y danh sách thanh toán", error = ex.Message });
            }
        }

        // GET: api/payments/renter/{renterId} - Láº¥y lá»‹ch sá»­ thanh toÃ¡n cá»§a Renter
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
                return Ok(new { success = false, message = "Mã ngu?i thuê không h?p l?" });
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
                return Ok(new { success = false, message = "Không th? l?y l?ch s? thanh toán", error = ex.Message });
            }
        }
    }
}

