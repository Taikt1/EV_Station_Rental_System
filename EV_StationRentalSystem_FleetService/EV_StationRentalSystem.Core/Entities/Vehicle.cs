using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class Vehicle
    {
        [Key]
        public Guid VehicleId { get; set; }

        [Required, MaxLength(20)]
        public string? PlateNumber { get; set; }

        public string? ChassisNumber { get; set; }
        public double BatteryCapacity { get; set; }
        public string Status { get; set; } = "Available"; // Available, In-use, Maintenance

        //[ForeignKey("BranchDestination")]
        //public Guid BranchId { get; set; }

        [ForeignKey("TypeVehicle")]
        public Guid TypeId { get; set; }

        public int ManufactureYear { get; set; }
        public string? Color { get; set; }
        public string? QRCode { get; set; }

        // Navigation
        //public BranchDestination BranchDestination { get; set; } = null!;
        public TypeVehicle TypeVehicle { get; set; } = null!;
        public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = null!;
        public ICollection<ChargingRecord> ChargingRecords { get; set; } = null!;
        public ICollection<VehicleSchedule> VehicleSchedules { get; set; } = null!;
        public ICollection<VehicleRelocation> VehicleRelocations { get; set; } = null!;
    }

}
