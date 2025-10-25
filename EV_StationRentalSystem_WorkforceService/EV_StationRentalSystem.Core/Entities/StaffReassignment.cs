using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class StaffReassignment
    {
        [Key]
        public Guid ReassignId { get; set; }

        [ForeignKey("StaffAssignment")]
        public Guid AssignmentId { get; set; }



        public Guid ToBranchId { get; set; }   // logic → Branch_Destination
        public DateTime Date { get; set; }
        public Guid ShiftId { get; set; }      // optional logic reference

        [MaxLength(255)]
        public string? Reason { get; set; }     // High Demand, Emergency, etc.
        public string Status { get; set; } = "Planned"; // Planned, Active, Completed

        // Navigation
        public StaffAssignment StaffAssignment { get; set; } = null!;
        public Shift Shift { get; set; } = null!;
    }

}
