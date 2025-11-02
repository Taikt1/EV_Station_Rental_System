using System.ComponentModel.DataAnnotations;

namespace EV_StationRentalSystem.Core.DTO.Request
{
    public class CreatePaymentRequest
    {
        [Required(ErrorMessage = "RentalId is required")]
        public string RentalId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "PaymentMethod is required")]
        [StringLength(50, ErrorMessage = "PaymentMethod cannot exceed 50 characters")]
        public string PaymentMethod { get; set; } = string.Empty;

        public string? TransactionRef { get; set; }
    }
}
