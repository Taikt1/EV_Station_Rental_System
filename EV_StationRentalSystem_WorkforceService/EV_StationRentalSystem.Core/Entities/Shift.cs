using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class Shift
    {
        [Key]
        public Guid ShiftId { get; set; }

        [Required, MaxLength(50)]
        public string ShiftName { get; set; } // Morning / Afternoon / Night

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        // Navigation
        public ICollection<StaffAssignment> StaffAssignments { get; set; } = null!;

        public ICollection<StaffReassignment> StaffReassignments { get; set; } = null!;

    }

}
