using EV_StationRentalSystem.Core.DTO.Request;
using EV_StationRentalSystem.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EV_StationRentalSystem.APIRentalPayment.Controllers
{
    [ApiController]
    [Route("api/checkout")]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        /// <summary>
        /// Nhận xe về, ghi nhận tình trạng và phí phát sinh
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCheckout([FromBody] CreateCheckoutRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _checkoutService.CreateCheckoutAsync(request);
                return Ok(new
                {
                    success = true,
                    message = "Trả xe thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = "Không thể trả xe",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy thông tin checkout theo order detail
        /// </summary>
        [HttpGet("/rental-contract/checkout/{orderId}")]
        public async Task<IActionResult> GetCheckoutByOrderId(Guid orderId)
        {
            var checkout = await _checkoutService.GetCheckoutByOrderIdAsync(orderId);
            if (checkout == null)
                return NotFound(new { Message = "Checkout record not found for this order." });

            return Ok(checkout);
        }

        /// <summary>
        /// Lấy danh sách checkout có phân trang
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPagedCheckouts([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _checkoutService.GetAllPagedAsync(pageIndex, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật trạng thái checkout
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateCheckoutStatus(Guid id, [FromBody] UpdateCheckoutStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _checkoutService.UpdateStatusAsync(id, request);
            if (updated == null)
                return NotFound(new { Message = "Checkout record not found." });

            return Ok(updated);
        }

        /// <summary>
        /// Lấy thông tin chi tiết 1 checkout
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _checkoutService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { Message = "Checkout not found." });

            return Ok(result);
        }
    }
}
