using System.ComponentModel.DataAnnotations;

namespace EV_StationRentalSystem.Core.DTO
{
    public class CreateFeedbackRequest
    {
        [Required(ErrorMessage = "RenterId is required")]
        public string RenterId { get; set; } = string.Empty;

        [Required(ErrorMessage = "RentalId is required")]
        public string RentalId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Score is required")]
        [Range(1, 5, ErrorMessage = "Score must be between 1 and 5")]
        public int Score { get; set; }

        [Required(ErrorMessage = "Comment is required")]
        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters")]
        public string Comment { get; set; } = string.Empty;
    }
}
