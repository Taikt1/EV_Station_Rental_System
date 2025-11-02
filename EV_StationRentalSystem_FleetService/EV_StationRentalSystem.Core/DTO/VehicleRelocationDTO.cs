using System;
using System.ComponentModel.DataAnnotations;

namespace EV_StationRentalSystem.Core.DTO
{
    public class VehicleRelocationCreateRequest
    {
        [Required]
        public Guid VehicleId { get; set; }
        
        [Required]
        public Guid ToBranchId { get; set; }
        
        [Required]
        public Guid AdminId { get; set; }
        
        [Required]
        public DateTime RelocationDate { get; set; }
        
        public DateTime? ExpectedArrivalTime { get; set; }
        public string? Reason { get; set; }
    }

    public class VehicleRelocationUpdateRequest
    {
        public DateTime? ArrivalTime { get; set; }
        public string? Status { get; set; } // Planned, In Transit, Completed
        public string? Reason { get; set; }
    }

    public class VehicleRelocationResponse
    {
        public Guid RelocationId { get; set; }
        public Guid VehicleId { get; set; }
        public string? VehiclePlateNumber { get; set; }
        public Guid ToBranchId { get; set; }
        public string? ToBranchName { get; set; }
        public Guid AdminId { get; set; }
        public DateTime RelocationDate { get; set; }
        public DateTime? ExpectedArrivalTime { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

