using System.ComponentModel.DataAnnotations;

namespace EV_StationRentalSystem.Core.DTO.Request
{
    public class CreateCheckinRequest
    {
        [Required]
        public Guid RentalOrderDetailId { get; set; }

        [Required(ErrorMessage = "StaffId is required")]
        public Guid StaffId { get; set; } 

        [Required(ErrorMessage = "OdometerReading is required")]
        [Range(0, int.MaxValue, ErrorMessage = "OdometerReading must be greater than or equal to 0")]
        public int OdometerReading { get; set; }

        [Required(ErrorMessage = "BatteryLevel is required")]
        [Range(0, 100, ErrorMessage = "BatteryLevel must be between 0 and 100")]
        public int BatteryLevel { get; set; }
        public string? Status { get; set; } = "Pending";

        public List<PhotoProofRequest>? Photos { get; set; }
    }

    public class PhotoProofRequest
    {
        [Required]
        public string PhotoUrl { get; set; } = null!;

        public string? Description { get; set; }
    }
}

