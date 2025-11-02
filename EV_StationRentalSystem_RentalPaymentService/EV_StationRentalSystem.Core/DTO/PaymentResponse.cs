using System;

namespace EV_StationRentalSystem.Core.DTO
{
    public class PaymentResponse
    {
        public string PaymentId { get; set; } = string.Empty;
        public string RentalId { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime PaymentTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? TransactionRef { get; set; }
    }
}
