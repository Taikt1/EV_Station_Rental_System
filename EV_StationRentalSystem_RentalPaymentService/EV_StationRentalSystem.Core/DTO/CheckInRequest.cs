using System.ComponentModel.DataAnnotations;

namespace EV_StationRentalSystem.Core.DTO
{
    public class CheckInRequest
    {
        [Required(ErrorMessage = "StaffId is required")]
        public string StaffId { get; set; } = string.Empty;

        [Required(ErrorMessage = "OdometerReading is required")]
        [Range(0, int.MaxValue, ErrorMessage = "OdometerReading must be greater than or equal to 0")]
        public int OdometerReading { get; set; }

        [Required(ErrorMessage = "BatteryLevel is required")]
        [Range(0, 100, ErrorMessage = "BatteryLevel must be between 0 and 100")]
        public int BatteryLevel { get; set; }

        public string? Notes { get; set; }
    }
}

