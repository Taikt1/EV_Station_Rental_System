using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class StaffAssignment
    {
        [Key]
        public Guid AssignmentId { get; set; }

        [ForeignKey("Workday")]
        public Guid WorkdayId { get; set; }

        [ForeignKey("Shift")]
        public Guid ShiftId { get; set; }

        [MaxLength(100)]
        public string? Task { get; set; }  // e.g., Kiểm tra xe, Hỗ trợ khách

        public string? Status { get; set; } = "Assigned"; // Assigned, Completed, Absent

        // Navigation
        public Workday Workday { get; set; } = null!;
        public Shift Shift { get; set; } = null!;
        public ICollection<StaffReassignment> StaffReassignments { get; set; } = null!;
    }

}
