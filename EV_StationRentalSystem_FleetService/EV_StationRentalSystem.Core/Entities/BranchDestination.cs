using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class BranchDestination
    {
        [Key]
        public Guid BranchId { get; set; }

        [Required, MaxLength(100)]
        public string? BranchName { get; set; }

        [Required, MaxLength(255)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? ContactNumber { get; set; }
        public string? WorkingHours { get; set; }
        public string Status { get; set; } = "Active";

        // Navigation
        //public ICollection<Vehicle> Vehicles { get; set; } = null!;
        public ICollection<VehicleAvailabilityByBranch> VehicleAvailabilities { get; set; } = null!;
    }
}

