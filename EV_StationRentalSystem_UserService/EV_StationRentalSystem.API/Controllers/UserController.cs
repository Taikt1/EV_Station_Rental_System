using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EV_StationRentalSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        [Authorize] // Protected endpoints
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            return Ok(new
            {
                UserId = userId,
                UserName = userName,
                Email = email,
                Message = "This is a protected endpoint"
            });
        }

        /// <summary>
        /// Simple API to get user info - For microservice communication
        /// </summary>
        /// <returns>User information with hardcoded data</returns>
        [HttpGet("info")]
        public async Task<IActionResult> GetUserInfo()
        {
            // Hardcoded data for testing
            var userInfo = new
            {
                UserId = "user001",
                UserName = "john_doe",
                Email = "john.doe@example.com",
                FullName = "John Doe",
                PhoneNumber = "+1234567890",
                DateOfBirth = new DateTime(1990, 5, 15),
                Address = "123 Main St, City, Country",
                Message = "Hardcoded user data from UserService",
                Timestamp = DateTime.UtcNow
            };

            return Ok(userInfo);
        }
    }
}
