using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.API.Controllers
{
    /// <summary>
    /// API để quản lý báo cáo và khiếu nại từ người dùng
    /// </summary>
    [Route("Workforce/Report")]
    [ApiController]
    public class UserReportController : ControllerBase
    {
        private readonly IUserReportService _reportService;

        public UserReportController(IUserReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Tạo báo cáo mới (User, Staff, Manager)
        /// </summary>
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateReport([FromBody] CreateReportRequest request)
        {
            try
            {
                var gatewayHeaders = ExtractGatewayHeaders();

                // Extract user info from Gateway headers or JWT claims
                var userId = gatewayHeaders?["X-User-Id"] ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userEmail = gatewayHeaders?["X-User-Email"] ?? User.FindFirst(ClaimTypes.Email)?.Value;
                var userName = gatewayHeaders?["X-User-Name"] ?? User.FindFirst(ClaimTypes.Name)?.Value;

                Console.WriteLine(userId,userName);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Không xác định được người dùng",
                        data = (object?)null
                    });
                }

                var report = await _reportService.CreateReportAsync(request, userId, userEmail, userName);

                return CreatedAtAction(nameof(GetReportById), new { reportId = report.ReportId }, new
                {
                    success = true,
                    message = "Tạo báo cáo thành công",
                    data = report
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
        /// Lấy thông tin báo cáo theo ID
        /// </summary>
        //[Authorize]
        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetReportById(Guid reportId)
        {
            try
            {
                var gatewayHeaders = ExtractGatewayHeaders();
                var userId = gatewayHeaders?["X-User-Id"] ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var report = await _reportService.GetReportByIdAsync(reportId, userId);

                if (report == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy báo cáo",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin báo cáo thành công",
                    data = report
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
        /// Lấy danh sách báo cáo của tôi (User)
        /// </summary>
        //[Authorize]
        [HttpGet("my-reports")]
        public async Task<IActionResult> GetMyReports()
        {
            try
            {
                var gatewayHeaders = ExtractGatewayHeaders();
                var userId = gatewayHeaders?["X-User-Id"] ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Không xác định được người dùng",
                        data = (object?)null
                    });
                }

                var reports = await _reportService.GetMyReportsAsync(userId);

                return Ok(new
                {
                    success = true,
                    message = $"Lấy danh sách {reports.Count} báo cáo thành công",
                    data = reports
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
        /// Đánh giá báo cáo đã được giải quyết (User)
        /// </summary>
        //[Authorize]
        [HttpPost("{reportId}/rate")]
        public async Task<IActionResult> RateReport(Guid reportId, [FromBody] RateReportRequest request)
        {
            try
            {
                var gatewayHeaders = ExtractGatewayHeaders();
                var userId = gatewayHeaders?["X-User-Id"] ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Không xác định được người dùng",
                        data = (object?)null
                    });
                }

                var report = await _reportService.RateReportAsync(reportId, request, userId);

                if (report == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy báo cáo",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Đánh giá báo cáo thành công",
                    data = report
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
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
        /// Lấy danh sách tất cả báo cáo với filter (Manager/Staff)
        /// </summary>
        //[Authorize(Roles = "manager,staff")]
        [HttpGet]
        public async Task<IActionResult> GetReports([FromQuery] ReportFilterRequest filter)
        {
            try
            {
                var result = await _reportService.GetReportsAsync(filter);

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách báo cáo thành công",
                    data = result.Reports,
                    pagination = new
                    {
                        pageNumber = result.PageNumber,
                        pageSize = result.PageSize,
                        totalRecords = result.TotalRecords,
                        totalPages = result.TotalPages
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
        /// Lấy danh sách báo cáo được assign cho tôi (Staff)
        /// </summary>
        //[Authorize(Roles = "manager,staff")]
        [HttpGet("my-assigned")]
        public async Task<IActionResult> GetMyAssignedReports()
        {
            try
            {
                var gatewayHeaders = ExtractGatewayHeaders();
                var userId = gatewayHeaders?["X-User-Id"] ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Không xác định được người dùng",
                        data = (object?)null
                    });
                }

                var reports = await _reportService.GetMyAssignedReportsAsync(userId);

                return Ok(new
                {
                    success = true,
                    message = $"Lấy danh sách {reports.Count} báo cáo được assign thành công",
                    data = reports
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
        /// Assign báo cáo cho staff (Manager/Staff)
        /// </summary>
        //[Authorize(Roles = "manager,staff")]
        [HttpPut("{reportId}/assign")]
        public async Task<IActionResult> AssignReport(Guid reportId, [FromBody] AssignReportRequest request)
        {
            try
            {
                var gatewayHeaders = ExtractGatewayHeaders();
                var assignerName = gatewayHeaders?["X-User-Name"] ?? User.FindFirst(ClaimTypes.Name)?.Value;

                var report = await _reportService.AssignReportAsync(reportId, request, assignerName);

                if (report == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy báo cáo",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Assign báo cáo thành công",
                    data = report
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
        /// Cập nhật báo cáo (Manager/Staff)
        /// </summary>
        //[Authorize(Roles = "manager,staff")]
        [HttpPut("{reportId}")]
        public async Task<IActionResult> UpdateReport(Guid reportId, [FromBody] UpdateReportRequest request)
        {
            try
            {
                var report = await _reportService.UpdateReportAsync(reportId, request);

                if (report == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy báo cáo",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật báo cáo thành công",
                    data = report
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
        /// Giải quyết báo cáo (Staff)
        /// </summary>
        //[Authorize(Roles = "manager,staff")]
        [HttpPut("{reportId}/resolve")]
        public async Task<IActionResult> ResolveReport(Guid reportId, [FromBody] ResolveReportRequest request)
        {
            try
            {
                var report = await _reportService.ResolveReportAsync(reportId, request);

                if (report == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy báo cáo",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Giải quyết báo cáo thành công",
                    data = report
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
        /// Đóng báo cáo (Manager/Staff)
        /// </summary>
        //[Authorize(Roles = "manager,staff")]
        [HttpPut("{reportId}/close")]
        public async Task<IActionResult> CloseReport(Guid reportId)
        {
            try
            {
                var report = await _reportService.CloseReportAsync(reportId);

                if (report == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy báo cáo",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Đóng báo cáo thành công",
                    data = report
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
        /// Từ chối báo cáo (Manager/Staff)
        /// </summary>
        //[Authorize(Roles = "manager,staff")]
        [HttpPut("{reportId}/reject")]
        public async Task<IActionResult> RejectReport(Guid reportId, [FromBody] RejectReportRequest request)
        {
            try
            {
                var report = await _reportService.RejectReportAsync(reportId, request.Reason);

                if (report == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy báo cáo",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Từ chối báo cáo thành công",
                    data = report
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
        /// Xóa báo cáo (Manager)
        /// </summary>
        //[Authorize(Roles = "manager")]
        [HttpDelete("{reportId}")]
        public async Task<IActionResult> DeleteReport(Guid reportId)
        {
            try
            {
                var result = await _reportService.DeleteReportAsync(reportId);

                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy báo cáo",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Xóa báo cáo thành công",
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
        /// Lấy thống kê báo cáo (Manager)
        /// </summary>
        //[Authorize(Roles = "manager")]
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            try
            {
                var statistics = await _reportService.GetStatisticsAsync(fromDate, toDate);

                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê báo cáo thành công",
                    data = statistics
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
    }

    /// <summary>
    /// Request để từ chối báo cáo
    /// </summary>
    public class RejectReportRequest
    {
        public string Reason { get; set; } = string.Empty;
    }
}
