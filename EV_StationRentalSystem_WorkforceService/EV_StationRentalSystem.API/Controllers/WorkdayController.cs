using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkdayController : ControllerBase
    {
        private readonly IWorkforceService _workforceService;

        public WorkdayController(IWorkforceService workforceService)
        {
            _workforceService = workforceService;
        }

        /// <summary>
        /// Lấy danh sách workday theo filter
        /// </summary>
        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> GetWorkdays([FromQuery] WorkdayFilterRequest filter)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var workdays = await _workforceService.GetWorkdaysByFilterAsync(filter, token);
                
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách ngày làm việc thành công",
                    data = workdays,
                    pagination = new
                    {
                        pageNumber = filter.PageNumber,
                        pageSize = filter.PageSize,
                        totalRecords = workdays.Count
                    }
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
        /// Lấy thông tin workday theo ID
        /// </summary>
        //[Authorize]
        [HttpGet("{workdayId}")]
        public async Task<IActionResult> GetWorkdayById(Guid workdayId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var workday = await _workforceService.GetWorkdayByIdAsync(workdayId, token);
                
                if (workday == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy ngày làm việc",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin ngày làm việc thành công",
                    data = workday
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
        /// Tạo workday mới (chỉ manager)
        /// </summary>
        //[Authorize(Roles = "manager")]
        [HttpPost]
        public async Task<IActionResult> CreateWorkday([FromBody] CreateWorkdayRequest request)
        {
            try
            {
                var workday = await _workforceService.CreateWorkdayAsync(request);
                return CreatedAtAction(nameof(GetWorkdayById), new { workdayId = workday.WorkdayId }, new
                {
                    success = true,
                    message = "Tạo ngày làm việc thành công",
                    data = workday
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
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

        /// <summary>
        /// Cập nhật workday (chỉ manager)
        /// </summary>
        //[Authorize(Roles = "manager")]
        [HttpPut("{workdayId}")]
        public async Task<IActionResult> UpdateWorkday(Guid workdayId, [FromBody] UpdateWorkdayRequest request)
        {
            try
            {
                var workday = await _workforceService.UpdateWorkdayAsync(workdayId, request);
                if (workday == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy ngày làm việc",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật ngày làm việc thành công",
                    data = workday
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
        /// Xóa workday (chỉ manager)
        /// </summary>
        //[Authorize(Roles = "manager")]
        [HttpDelete("{workdayId}")]
        public async Task<IActionResult> DeleteWorkday(Guid workdayId)
        {
            try
            {
                var result = await _workforceService.DeleteWorkdayAsync(workdayId);
                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy ngày làm việc",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Xóa ngày làm việc thành công",
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

        /// <summary>
        /// Lấy lịch làm việc của một nhân viên
        /// </summary>
        //[Authorize]
        [HttpGet("staff/{staffId}/schedule")]
        public async Task<IActionResult> GetStaffSchedule(
            Guid staffId, 
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var schedule = await _workforceService.GetStaffScheduleAsync(staffId, startDate, endDate, token);
                
                return Ok(new
                {
                    success = true,
                    message = "Lấy lịch làm việc nhân viên thành công",
                    data = schedule
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
        /// Lấy lịch làm việc của một chi nhánh
        /// </summary>
        //[Authorize(Roles = "manager,staff")]
        [HttpGet("branch/{branchId}/schedule")]
        public async Task<IActionResult> GetBranchSchedule(
            Guid branchId, 
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var schedule = await _workforceService.GetBranchScheduleAsync(branchId, startDate, endDate, token);
                
                return Ok(new
                {
                    success = true,
                    message = "Lấy lịch làm việc chi nhánh thành công",
                    data = schedule
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
