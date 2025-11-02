using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EV_StationRentalSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("profile/{userId}")]
        public async Task<IActionResult> GetProfile(string userId)
        {
            try
            {
                // Check if user is requesting their own profile or is an admin
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (currentUserId != userId && userRole != "manager" && userRole != "staff")
                {
                    return Forbid();
                }

                var profile = await _userService.GetUserProfileAsync(userId);
                if (profile == null)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "User not found",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "User profile retrieved successfully",
                    data = profile
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error retrieving profile: {ex.Message}",
                    data = (object?)null
                });
            }
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User not authenticated",
                        data = (object?)null
                    });
                }

                var profile = await _userService.GetUserProfileAsync(userId);
                if (profile == null)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "User not found",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "User profile retrieved successfully",
                    data = profile
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error retrieving profile: {ex.Message}",
                    data = (object?)null
                });
            }
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User not authenticated",
                        data = (object?)null
                    });
                }

                var updatedProfile = await _userService.UpdateProfileAsync(userId, request);
                if (updatedProfile == null)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Failed to update profile",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Profile updated successfully",
                    data = updatedProfile
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error updating profile: {ex.Message}",
                    data = (object?)null
                });
            }
        }

        [Authorize]
        [HttpPost("documents")]
        public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User not authenticated",
                        data = (object?)null
                    });
                }

                var fileUrl = await _userService.UploadDocumentAsync(userId, request);

                return Ok(new
                {
                    success = true,
                    message = "Document uploaded successfully",
                    data = new { fileUrl }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error uploading document: {ex.Message}",
                    data = (object?)null
                });
            }
        }

        [Authorize(Roles = "manager,staff")]
        [HttpGet("{userId}/verification-status")]
        public async Task<IActionResult> GetVerificationStatus(string userId)
        {
            try
            {
                var profile = await _userService.GetUserProfileAsync(userId);
                if (profile == null)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "User not found",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "User verification status retrieved",
                    data = new
                    {
                        userId = profile.UserId,
                        email = profile.Email,
                        fullName = profile.FullName,
                        status = profile.Status,
                        hasCCCD = !string.IsNullOrEmpty(profile.CCCDUrl),
                        hasAvatar = !string.IsNullOrEmpty(profile.AvatarUrl),
                        cccdUrl = profile.CCCDUrl,
                        avatarUrl = profile.AvatarUrl
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error retrieving verification status: {ex.Message}",
                    data = (object?)null
                });
            }
        }

        [Authorize(Roles = "manager,staff")]
        [HttpPut("{userId}/verify")]
        public async Task<IActionResult> VerifyUser(string userId, [FromBody] VerifyUserRequest request)
        {
            try
            {
                var success = await _userService.VerifyUserAsync(userId, request.Status);
                if (!success)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Failed to verify user",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = $"User status updated to {request.Status}",
                    data = new { userId, status = request.Status }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error verifying user: {ex.Message}",
                    data = (object?)null
                });
            }
        }
    }
}
