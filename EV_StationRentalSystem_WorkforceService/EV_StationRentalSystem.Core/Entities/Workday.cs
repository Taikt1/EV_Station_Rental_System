using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class Workday
    {
        [Key]
        public Guid WorkdayId { get; set; }

        public Guid StaffId { get; set; }     // logic → User_Profile
        public Guid BranchId { get; set; }    // logic → Branch_Destination

        [Required]
        public DateTime Date { get; set; }

        // Navigation
        public ICollection<StaffAssignment> StaffAssignments { get; set; } 
    }

}
