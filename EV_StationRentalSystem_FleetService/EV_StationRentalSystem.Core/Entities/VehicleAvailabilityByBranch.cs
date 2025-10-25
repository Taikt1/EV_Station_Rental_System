using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class VehicleAvailabilityByBranch
    {
        [Key]
        public Guid AvailabilityId { get; set; }

        public Guid BranchId { get; set; }
        public DateTime Date { get; set; }

        public int AvailableCount { get; set; }
        public int InUseCount { get; set; }
        public int MaintenanceCount { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public BranchDestination BranchDestination { get; set; } = null!;
    }

}
