using System;

namespace EV_StationRentalSystem.Core.DTO
{
    public class UserProfileResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime? Dob { get; set; }
        public string? Address { get; set; }
        public string? AvatarUrl { get; set; }
        public string? CCCDUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
