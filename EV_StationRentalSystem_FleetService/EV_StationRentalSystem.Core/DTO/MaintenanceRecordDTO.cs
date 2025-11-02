using System;
using System.ComponentModel.DataAnnotations;

namespace EV_StationRentalSystem.Core.DTO
{
    public class MaintenanceRecordCreateRequest
    {
        [Required]
        public Guid VehicleId { get; set; }
        
        [Required]
        public DateTime MaintenanceDate { get; set; }
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public decimal Cost { get; set; }
        
        [Required]
        public int PerformedBy { get; set; }
        
        public DateTime? NextMaintenanceDate { get; set; }
    }

    public class MaintenanceRecordUpdateRequest
    {
        public DateTime? MaintenanceDate { get; set; }
        public string? Description { get; set; }
        public decimal? Cost { get; set; }
        public int? PerformedBy { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
    }

    public class MaintenanceRecordResponse
    {
        public Guid MaintenanceId { get; set; }
        public Guid VehicleId { get; set; }
        public string? VehiclePlateNumber { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public int PerformedBy { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
    }
}

