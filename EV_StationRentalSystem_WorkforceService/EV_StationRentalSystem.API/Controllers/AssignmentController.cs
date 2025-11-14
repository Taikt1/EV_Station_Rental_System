using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.API.Controllers
{
    [Route("Workforce/Assignment")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IWorkforceService _workforceService;

        public AssignmentController(IWorkforceService workforceService)
        {
            _workforceService = workforceService;
        }

        /// <summary>
        /// Lấy thông tin assignment theo ID
        /// </summary>
        //[Authorize]
        [HttpGet("{assignmentId}")]
        public async Task<IActionResult> GetAssignmentById(Guid assignmentId)
        {
            try
            {
                var assignment = await _workforceService.GetAssignmentByIdAsync(assignmentId);
                if (assignment == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy phân công",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin phân công thành công",
                    data = assignment
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
        /// Lấy các assignment của một workday
        /// </summary>
        //[Authorize]
        [HttpGet("workday/{workdayId}")]
        public async Task<IActionResult> GetAssignmentsByWorkday(Guid workdayId)
        {
            try
            {
                var assignments = await _workforceService.GetAssignmentsByWorkdayAsync(workdayId);
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách phân công thành công",
                    data = assignments
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
        /// Tạo assignment mới (chỉ manager)
        /// </summary>
        //[Authorize(Roles = "manager")]
        [HttpPost]
        public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentRequest request)
        {
            try
            {
                var assignment = await _workforceService.CreateAssignmentAsync(request);
                return CreatedAtAction(nameof(GetAssignmentById), new { assignmentId = assignment.AssignmentId }, new
                {
                    success = true,
                    message = "Tạo phân công thành công",
                    data = assignment
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
        /// Tạo nhiều assignments cùng lúc (chỉ manager)
        /// </summary>
        //[Authorize(Roles = "manager")]
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulkAssignments([FromBody] BulkAssignmentRequest request)
        {
            try
            {
                var assignments = await _workforceService.CreateBulkAssignmentsAsync(request);
                return Ok(new
                {
                    success = true,
                    message = $"Tạo {assignments.Count} phân công thành công",
                    data = assignments
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
        /// Cập nhật assignment (manager hoặc staff)
        /// </summary>
        //[Authorize(Roles = "manager,staff")]
        [HttpPut("{assignmentId}")]
        public async Task<IActionResult> UpdateAssignment(Guid assignmentId, [FromBody] UpdateAssignmentRequest request)
        {
            try
            {
                var assignment = await _workforceService.UpdateAssignmentAsync(assignmentId, request);
                if (assignment == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy phân công",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật phân công thành công",
                    data = assignment
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
        /// Xóa assignment (chỉ manager)
        /// </summary>
        //[Authorize(Roles = "manager")]
        [HttpDelete("{assignmentId}")]
        public async Task<IActionResult> DeleteAssignment(Guid assignmentId)
        {
            try
            {
                var result = await _workforceService.DeleteAssignmentAsync(assignmentId);
                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy phân công",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Xóa phân công thành công",
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
