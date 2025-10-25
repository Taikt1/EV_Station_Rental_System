using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class ChargingRecord
    {
        [Key]
        public Guid ChargingId { get; set; }

        public Guid VehicleId { get; set; }
        public Guid BranchId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double EnergyAdded { get; set; }
        public string? ChargedBy { get; set; } // logic → StaffID

        public Vehicle Vehicle { get; set; } = null!;
        public BranchDestination BranchDestination { get; set; } = null!;
    }

}
