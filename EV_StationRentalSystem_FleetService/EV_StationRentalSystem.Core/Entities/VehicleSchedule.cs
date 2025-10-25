using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class VehicleSchedule
    {
        [Key]
        public Guid VehicleScheduleId { get; set; }

        public Guid VehicleId { get; set; }
        public Guid BranchId { get; set; }

        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string? Status { get; set; } // Available, Reserved, In-use, Under Maintenance

        public Vehicle Vehicle { get; set; } = null!;
        public BranchDestination BranchDestination { get; set; } = null!;
    }

}
