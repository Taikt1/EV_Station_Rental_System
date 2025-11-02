using System;
using System.ComponentModel.DataAnnotations;

namespace EV_StationRentalSystem.Core.DTO.Request
{
    public class CreateRentalOrderRequest
    {
        [Required(ErrorMessage = "RenterId is required")]
        public string RenterId { get; set; } = string.Empty;

        public Guid? StaffId { get; set; }

        [Required(ErrorMessage = "BranchStartId is required")]
        public string BranchStartId { get; set; } = string.Empty;

        [Required(ErrorMessage = "BranchEndId is required")]
        public string BranchEndId { get; set; } = string.Empty;

        [Required(ErrorMessage = "StartTime is required")]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [Required(ErrorMessage = "EstimatedCost is required")]
        [Range(0, double.MaxValue, ErrorMessage = "EstimatedCost must be greater than 0")]
        public decimal EstimatedCost { get; set; }

        [Required]
        public List<CreateRentalOrderDetailRequest> Details { get; set; } = new();
    }
}
