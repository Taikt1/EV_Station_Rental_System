using EV_StationRentalSystem.Core.HttpClients;
using Microsoft.AspNetCore.Mvc;

namespace EV_StationRentalSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserIntegrationController : ControllerBase
    {
        private readonly UserMicroClient _userMicroClient;

        public UserIntegrationController(UserMicroClient userMicroClient)
        {
            _userMicroClient = userMicroClient;
        }

        /// <summary>
        /// Test API to get user information from UserService
        /// </summary>
        /// <returns>User profile information</returns>
        [HttpGet("user-info")]
        public async Task<IActionResult> GetUserInfoFromUserService()
        {
            try
            {
                var user = await _userMicroClient.GetUserInfoAsync();

                if (user == null)
                {
                    return NotFound(new { Message = "Could not retrieve user info from UserService" });
                }

                return Ok(new
                {
                    Message = "Successfully retrieved user info from UserService",
                    Data = user,
                    Source = "UserService via RentalPaymentService",
                    CalledAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Error communicating with UserService",
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Test connection to UserService
        /// </summary>
        /// <returns>Connection test result</returns>
        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                // Try to get user info to test connection
                var userInfo = await _userMicroClient.GetUserInfoAsync();

                return Ok(new
                {
                    Message = "Connection to UserService successful",
                    Timestamp = DateTime.UtcNow,
                    ReceivedData = userInfo != null,
                    Status = "Connected"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Connection to UserService failed",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message,
                    Status = "Disconnected"
                });
            }
        }
    }
}
