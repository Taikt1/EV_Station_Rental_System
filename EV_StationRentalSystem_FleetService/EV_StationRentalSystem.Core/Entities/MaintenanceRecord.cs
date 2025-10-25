using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class MaintenanceRecord
    {
        [Key]
        public Guid MaintenanceId { get; set; }

        public Guid VehicleId { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public string? Description { get; set; }
        public decimal Cost { get; set; }
        public int PerformedBy { get; set; } // logic → StaffID
        public DateTime? NextMaintenanceDate { get; set; }

        public Vehicle Vehicle { get; set; } = null!;
    }

}
