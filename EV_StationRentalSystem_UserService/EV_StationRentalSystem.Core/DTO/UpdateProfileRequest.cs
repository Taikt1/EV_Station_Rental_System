using System;
using System.ComponentModel.DataAnnotations;

namespace EV_StationRentalSystem.Core.DTO
{
    public class UpdateProfileRequest
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        public DateTime? Dob { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
