using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.HttpClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.API.Controllers
{
    /// <summary>
    /// API quản lý người dùng (chỉ dành cho Manager)
    /// Gọi tới UserService để thực hiện các thao tác quản lý
    /// </summary>
    [Route("Workforce/UserManagement")]
    [ApiController]
    //[Authorize(Roles = "manager")]
    public class UserManagementController : ControllerBase
    {
        private readonly UserMicroClient _userMicroClient;

        public UserManagementController(UserMicroClient userMicroClient)
        {
            _userMicroClient = userMicroClient;
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một user
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var gatewayHeaders = ExtractGatewayHeaders();

                var user = await _userMicroClient.GetUserProfileAsync(userId, token, gatewayHeaders);

                if (user == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy người dùng",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin người dùng thành công",
                    data = user
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
        /// Lấy danh sách nhiều users
        /// </summary>
        [HttpPost("bulk")]
        public async Task<IActionResult> GetMultipleUsers([FromBody] List<string> userIds)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var gatewayHeaders = ExtractGatewayHeaders();

                var users = await _userMicroClient.GetMultipleUserProfilesAsync(userIds, token, gatewayHeaders);

                return Ok(new
                {
                    success = true,
                    message = $"Lấy thông tin {users.Count}/{userIds.Count} người dùng thành công",
                    data = users
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
        /// Cập nhật thông tin user (chỉ manager)
        /// </summary>
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] AdminUpdateUserRequest request)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var gatewayHeaders = ExtractGatewayHeaders();

                var updatedUser = await _userMicroClient.AdminUpdateUserAsync(userId, request, token, gatewayHeaders);

                if (updatedUser == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Không thể cập nhật người dùng",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật người dùng thành công",
                    data = updatedUser
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
        /// Thay đổi role của user (chỉ manager)
        /// </summary>
        [HttpPut("{userId}/role")]
        public async Task<IActionResult> ChangeUserRole(string userId, [FromBody] ChangeUserRoleRequest request)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var gatewayHeaders = ExtractGatewayHeaders();

                var success = await _userMicroClient.ChangeUserRoleAsync(userId, request.NewRole, token, gatewayHeaders);

                if (!success)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Không thể thay đổi role",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = $"Đã thay đổi role của user {userId} thành {request.NewRole}",
                    data = new { userId, newRole = request.NewRole }
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
        /// Khóa user (chỉ manager)
        /// </summary>
        [HttpPut("{userId}/lock")]
        public async Task<IActionResult> LockUser(string userId, [FromBody] LockUserRequest request)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var gatewayHeaders = ExtractGatewayHeaders();

                var success = await _userMicroClient.LockUserAsync(
                    userId,
                    isLocked: true,
                    request.Reason,
                    token,
                    gatewayHeaders);

                if (!success)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Không thể khóa người dùng",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = $"Đã khóa người dùng {userId}",
                    data = new { userId, isLocked = true, reason = request.Reason }
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
        /// Mở khóa user (chỉ manager)
        /// </summary>
        [HttpPut("{userId}/unlock")]
        public async Task<IActionResult> UnlockUser(string userId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var gatewayHeaders = ExtractGatewayHeaders();

                var success = await _userMicroClient.LockUserAsync(
                    userId,
                    isLocked: false,
                    reason: null,
                    token,
                    gatewayHeaders);

                if (!success)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Không thể mở khóa người dùng",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = $"Đã mở khóa người dùng {userId}",
                    data = new { userId, isLocked = false }
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
        /// Xóa user (chỉ manager)
        /// </summary>
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId, [FromBody] DeleteUserRequest? request = null)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var gatewayHeaders = ExtractGatewayHeaders();

                var success = await _userMicroClient.DeleteUserAsync(
                    userId,
                    request?.Reason,
                    token,
                    gatewayHeaders);

                if (!success)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Không thể xóa người dùng",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = $"Đã xóa người dùng {userId}",
                    data = new { userId, reason = request?.Reason }
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
        /// Verify user - phê duyệt tài khoản (chỉ manager/staff)
        /// </summary>
        [HttpPut("{userId}/verify")]
        public async Task<IActionResult> VerifyUser(string userId, [FromBody] VerifyUserRequest request)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var gatewayHeaders = ExtractGatewayHeaders();

                // Gọi API verify từ UserService
                // Tạm thời sử dụng AdminUpdateUserRequest để update status
                var updateRequest = new AdminUpdateUserRequest
                {
                    Status = request.Status
                };

                var updatedUser = await _userMicroClient.AdminUpdateUserAsync(userId, updateRequest, token, gatewayHeaders);

                if (updatedUser == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Không thể verify người dùng",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = $"Đã cập nhật trạng thái người dùng thành {request.Status}",
                    data = updatedUser
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
}
