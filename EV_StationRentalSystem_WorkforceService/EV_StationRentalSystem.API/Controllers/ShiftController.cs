using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.API.Controllers
{
    [Route("Workforce/Shift")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private readonly IWorkforceService _workforceService;

        public ShiftController(IWorkforceService workforceService)
        {
            _workforceService = workforceService;
        }

        /// <summary>
        /// Lấy tất cả các ca làm việc
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllShifts()
        {
            try
            {
                var shifts = await _workforceService.GetAllShiftsAsync();
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách ca làm việc thành công",
                    data = shifts
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Lỗi: {ex.Message}",
                    data = (object?)null
                });
            }
        }

        /// <summary>
        /// Lấy thông tin ca làm việc theo ID
        /// </summary>
        [HttpGet("{shiftId}")]
        public async Task<IActionResult> GetShiftById(Guid shiftId)
        {
            try
            {
                var shift = await _workforceService.GetShiftByIdAsync(shiftId);
                if (shift == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy ca làm việc",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin ca làm việc thành công",
                    data = shift
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Lỗi: {ex.Message}",
                    data = (object?)null
                });
            }
        }

        /// <summary>
        /// Helper method để extract Gateway headers
        /// </summary>
        private Dictionary<string, string>? ExtractGatewayHeaders()
        {
            var headers = new Dictionary<string, string>();

            // Extract các headers từ Gateway
            var userId = Request.Headers["X-User-Id"].FirstOrDefault();
            var userEmail = Request.Headers["X-User-Email"].FirstOrDefault();
            var userRole = Request.Headers["X-User-Role"].FirstOrDefault();
            var userName = Request.Headers["X-User-Name"].FirstOrDefault();

            if (!string.IsNullOrEmpty(userId))
            {
                headers["X-User-Id"] = userId;
                if (!string.IsNullOrEmpty(userEmail)) headers["X-User-Email"] = userEmail;
                if (!string.IsNullOrEmpty(userRole)) headers["X-User-Role"] = userRole;
                if (!string.IsNullOrEmpty(userName)) headers["X-User-Name"] = userName;

                return headers;
            }

            return null;
        }

        /// <summary>
        /// Tạo ca làm việc mới (chỉ manager)
        /// </summary>
        //[Authorize(Roles = "manager")]
        [HttpPost]
        public async Task<IActionResult> CreateShift([FromBody] CreateShiftRequest request)
        {
            try
            {
                var shift = await _workforceService.CreateShiftAsync(request);
                return CreatedAtAction(nameof(GetShiftById), new { shiftId = shift.ShiftId }, new
                {
                    success = true,
                    message = "Tạo ca làm việc thành công",
                    data = shift
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Lỗi: {ex.Message}",
                    data = (object?)null
                });
            }
        }

        /// <summary>
        /// Cập nhật ca làm việc (chỉ manager)
        /// </summary>
        //[Authorize(Roles = "manager")]
        [HttpPut("{shiftId}")]
        public async Task<IActionResult> UpdateShift(Guid shiftId, [FromBody] UpdateShiftRequest request)
        {
            try
            {
                var shift = await _workforceService.UpdateShiftAsync(shiftId, request);
                if (shift == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy ca làm việc",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật ca làm việc thành công",
                    data = shift
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Lỗi: {ex.Message}",
                    data = (object?)null
                });
            }
        }

        /// <summary>
        /// Xóa ca làm việc (chỉ manager)
        /// </summary>
        //[Authorize(Roles = "manager")]
        [HttpDelete("{shiftId}")]
        public async Task<IActionResult> DeleteShift(Guid shiftId)
        {
            try
            {
                var result = await _workforceService.DeleteShiftAsync(shiftId);
                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy ca làm việc",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Xóa ca làm việc thành công",
                    data = (object?)null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Lỗi: {ex.Message}",
                    data = (object?)null
                });
            }
        }
    }
}
