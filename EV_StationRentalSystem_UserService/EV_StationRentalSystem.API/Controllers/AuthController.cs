using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.ServiceContracts;
using EV_StationRentalSystem.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EV_StationRentalSystem.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IGoogleAuthService _googleAuthService;

        public AuthController(IUserService userService, IGoogleAuthService googleAuthService)
        {
            _userService = userService;
            _googleAuthService = googleAuthService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var result = await _userService.Login(loginRequest);
            if (result != null && result.Success)
            {
                return Ok(result);
            }

            return Unauthorized(new { message = "Invalid email or password" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {

            var result = await _userService.Register(registerRequest);
            if (result != null && result.Success)
            {
                return Ok(result);
            }

            return BadRequest(new { message = "Registration failed. Email might already exist." });
        }

        [HttpPost("logout")]
        [Authorize] // Requires user to be authenticated
        public async Task<IActionResult> Logout()
        {
            var result = await _userService.Logout();
            if (result)
            {
                return Ok(new { success = true, message = "Logged out successfully" });
            }

            return BadRequest(new { success = false, message = "Logout failed" });
        }

        [HttpPost("google-login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                var result = await _googleAuthService.AuthenticateGoogleUser(request.IdToken);

                return Ok(new
                {
                    success = true,
                    message = "Đăng nhập Google thành công",
                    data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message,
                    data = (object?)null
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = "Đăng nhập Google thất bại: " + ex.Message,
                    data = (object?)null
                });
            }
        }
    }
}
