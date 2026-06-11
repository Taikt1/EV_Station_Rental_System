using EV_StationRentalSystem.Core.DTO.Request;
using EV_StationRentalSystem.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EV_StationRentalSystem.APIRentalPayment.Controllers
{
    [ApiController]
    [Route("api/rental-contracts")]
    public class RentalContractController : ControllerBase
    {
        private readonly IRentalContractService _service;

        public RentalContractController(IRentalContractService service)
        {
            _service = service;
        }

        [HttpGet("by-rental/{rentalId}")]
        public async Task<IActionResult> GetByRentalId(Guid rentalId)
        {
            try
            {
                var result = await _service.GetByRentalIdAsync(rentalId);
                
                if (result == null)
                {
                    return Ok(new 
                    { 
                        success = false, 
                        message = "Chưa có hợp đồng cho đơn thuê này",
                        data = (object)null 
                    });
                }
                
                return Ok(new 
                { 
                    success = true, 
                    message = "Lấy hợp đồng thành công",
                    data = result 
                });
            }
            catch (Exception ex)
            {
                return Ok(new 
                { 
                    success = false, 
                    message = "Không thể lấy hợp đồng",
                    error = ex.Message 
                });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetByUserOrStaff([FromQuery] Guid? renterId, [FromQuery] Guid? staffId)
        {
            var result = await _service.GetByUserOrStaffAsync(renterId, staffId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRentalContractRequest request)
        {
            var result = await _service.CreateAsync(request);
            return Ok(result);
        }

        [HttpPut("{contractId}/signatures")]
        public async Task<IActionResult> UpdateSignature(int contractId, [FromBody] UpdateContractSignatureRequest request)
        {
            request.ContractId = contractId;
            var result = await _service.UpdateSignatureAsync(request);
            return Ok(result);
        }
    }
}
