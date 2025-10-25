using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class VehicleRelocation
    {
        [Key]
        public Guid RelocationId { get; set; }

        public Guid VehicleId { get; set; }
        public Guid ToBranchId { get; set; }
        public Guid AdminId { get; set; } // logic → Account

        public DateTime RelocationDate { get; set; }
        public DateTime? ExpectedArrivalTime { get; set; }
        public DateTime? ArrivalTime { get; set; }

        public string? Reason { get; set; }
        public string? Status { get; set; } // Planned, In Transit, Completed

        public Vehicle Vehicle { get; set; } = null!;
        public BranchDestination ToBranch { get; set; } = null!;
    }

}
