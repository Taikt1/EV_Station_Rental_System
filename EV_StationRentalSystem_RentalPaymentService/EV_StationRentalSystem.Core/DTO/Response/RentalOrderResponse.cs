using System;

namespace EV_StationRentalSystem.Core.DTO.Response
{
    public class RentalOrderResponse
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
        public DateTime CreatedAt { get; set; }

        public int DetailCount { get; set; }
    }
}
