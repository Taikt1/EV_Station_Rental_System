using EV_StationRentalSystem.Core.DTO.Request;
using EV_StationRentalSystem.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EV_StationRentalSystem.APIRentalPayment.Controllers
{
    [ApiController]
    [Route("api/checkin")]
    public class CheckinController : ControllerBase
    {
        private readonly ICheckinService _service;

        public CheckinController(ICheckinService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách bàn giao xe (phân trang)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetPagedAsync(pageIndex, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết bàn giao theo đơn thuê
        /// </summary>
        [HttpGet("/rental-contract/checkin/{orderId}")]
        public async Task<IActionResult> GetByOrderId(Guid orderId)
        {
            var result = await _service.GetByOrderIdAsync(orderId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Tạo mới một check-in (bàn giao xe)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCheckinRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetByOrderId),
                new { orderId = result.RentalOrderDetailId }, result);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateCheckinStatusRequest request)
        {
            var success = await _service.UpdateStatusAsync(id, request);
            if (success == null)
                return NotFound(new { message = "Checkin not found" });
            return Ok(success);
        }
    }
}
