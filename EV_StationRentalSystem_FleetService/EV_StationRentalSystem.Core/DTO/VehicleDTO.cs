using System;
using System.ComponentModel.DataAnnotations;

namespace EV_StationRentalSystem.Core.DTO
{
    public class VehicleCreateRequest
    {
        [Required, MaxLength(20)]
        public string PlateNumber { get; set; } = string.Empty;

        public string? ChassisNumber { get; set; }
        
        [Required]
        public double BatteryCapacity { get; set; }
        
        [Required]
        public Guid TypeId { get; set; }
        
        [Required]
        public int ManufactureYear { get; set; }
        
        public string? Color { get; set; }
        public string? QRCode { get; set; }
    }

    public class VehicleUpdateRequest
    {
        [MaxLength(20)]
        public string? PlateNumber { get; set; }
        
        public string? ChassisNumber { get; set; }
        public double? BatteryCapacity { get; set; }
        public string? Status { get; set; }
        public Guid? TypeId { get; set; }
        public int? ManufactureYear { get; set; }
        public string? Color { get; set; }
        public string? QRCode { get; set; }
    }

    public class VehicleResponse
    {
        public Guid VehicleId { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string? ChassisNumber { get; set; }
        public double BatteryCapacity { get; set; }
        public string Status { get; set; } = string.Empty;
        public Guid TypeId { get; set; }
        public string? TypeName { get; set; }
        public int ManufactureYear { get; set; }
        public string? Color { get; set; }
        public string? QRCode { get; set; }
    }

    public class VehicleStatusSummaryResponse
    {
        public int TotalVehicles { get; set; }
        public int AvailableCount { get; set; }
        public int InUseCount { get; set; }
        public int MaintenanceCount { get; set; }
    }
}

