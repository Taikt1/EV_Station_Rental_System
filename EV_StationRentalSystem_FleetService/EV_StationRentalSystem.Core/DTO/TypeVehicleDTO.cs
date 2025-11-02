using System;
using System.ComponentModel.DataAnnotations;

namespace EV_StationRentalSystem.Core.DTO
{
    public class TypeVehicleCreateRequest
    {
        [Required, MaxLength(100)]
        public string TypeName { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? Brand { get; set; }
        
        [MaxLength(100)]
        public string? Model { get; set; }
        
        [Required]
        public double DefaultBattery { get; set; }
        
        [Required]
        public decimal BasePrice { get; set; }
        
        public string? Description { get; set; }
    }

    public class TypeVehicleUpdateRequest
    {
        [MaxLength(100)]
        public string? TypeName { get; set; }
        
        [MaxLength(100)]
        public string? Brand { get; set; }
        
        [MaxLength(100)]
        public string? Model { get; set; }
        
        public double? DefaultBattery { get; set; }
        public decimal? BasePrice { get; set; }
        public string? Description { get; set; }
    }

    public class TypeVehicleResponse
    {
        public Guid TypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public double DefaultBattery { get; set; }
        public decimal BasePrice { get; set; }
        public string? Description { get; set; }
        public int VehicleCount { get; set; }
    }
}

