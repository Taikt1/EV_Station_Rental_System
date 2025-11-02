using EV_StationRentalSystem.Core.DTO.Request;
using EV_StationRentalSystem.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EV_StationRentalSystem.APIRentalPayment.Controllers
{
    [ApiController]
    [Route("api/penalty")]
    public class PenaltyController : ControllerBase
    {
        private readonly IPenaltyService _penaltyService;

        public PenaltyController(IPenaltyService penaltyService)
        {
            _penaltyService = penaltyService;
        }

        /// <summary>
        /// Tạo mới bản ghi phạt (PenaltyRecord)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePenalty([FromBody] CreatePenaltyRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _penaltyService.CreatePenaltyAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin phạt theo ID
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPenaltyById(Guid id)
        {
            var result = await _penaltyService.GetPenaltyByIdAsync(id);
            if (result == null)
                return NotFound("Không tìm thấy bản ghi phạt.");
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách phạt theo mã đơn thuê (RentalId)
        /// </summary>
        [HttpGet("rental/{rentalId:guid}")]
        public async Task<IActionResult> GetPenaltiesByRentalId(Guid rentalId)
        {
            var result = await _penaltyService.GetPenaltiesByRentalIdAsync(rentalId);
            return Ok(result);
        }
    }
}
