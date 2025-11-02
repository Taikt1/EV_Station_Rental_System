using System;
using System.Collections.Generic;

namespace EV_StationRentalSystem.Core.DTO
{
    public class RentalOrderDetailResponse
    {
        public string RentalId { get; set; } = string.Empty;
        public string RenterId { get; set; } = string.Empty;
        public string? StaffId { get; set; }
        public string VehicleId { get; set; } = string.Empty;
        public string BranchStartId { get; set; } = string.Empty;
        public string BranchEndId { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal EstimatedCost { get; set; }
        public decimal? ActualCost { get; set; }
        
        // Additional details
        public List<PaymentInfo>? Payments { get; set; }
        public List<FeedbackInfo>? Feedbacks { get; set; }
    }

    public class PaymentInfo
    {
        public string PaymentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
    }

    public class FeedbackInfo
    {
        public string FeedbackId { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
