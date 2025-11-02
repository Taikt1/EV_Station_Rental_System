using System;
using System.ComponentModel.DataAnnotations;

namespace EV_StationRentalSystem.Core.DTO
{
    public class BranchDestinationCreateRequest
    {
        [Required, MaxLength(100)]
        public string BranchName { get; set; } = string.Empty;
        
        [Required, MaxLength(255)]
        public string Address { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? City { get; set; }
        
        [Required]
        public double Latitude { get; set; }
        
        [Required]
        public double Longitude { get; set; }
        
        public string? ContactNumber { get; set; }
        public string? WorkingHours { get; set; }
    }

    public class BranchDestinationUpdateRequest
    {
        [MaxLength(100)]
        public string? BranchName { get; set; }
        
        [MaxLength(255)]
        public string? Address { get; set; }
        
        [MaxLength(100)]
        public string? City { get; set; }
        
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? ContactNumber { get; set; }
        public string? WorkingHours { get; set; }
        public string? Status { get; set; }
    }

    public class BranchDestinationResponse
    {
        public Guid BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? City { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? ContactNumber { get; set; }
        public string? WorkingHours { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalVehicles { get; set; }
        public int AvailableVehicles { get; set; }
    }
}

