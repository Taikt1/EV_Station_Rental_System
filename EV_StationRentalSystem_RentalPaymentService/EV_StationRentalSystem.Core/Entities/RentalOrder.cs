using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class RentalOrder
    {
        [Key]
        public Guid RentalId { get; set; }

        public Guid RenterId { get; set; }   // FK logic → User_Profile
        public Guid? StaffId { get; set; }   // FK logic → User_Profile (Staff)
        public Guid VehicleId { get; set; }  // FK logic → Vehicle
        public Guid BranchStartId { get; set; } // FK logic → Branch_Destination
        public Guid BranchEndId { get; set; }   // FK logic → Branch_Destination

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; } // Pending, Active, Completed, Cancelled

        public decimal EstimatedCost { get; set; }
        public decimal? ActualCost { get; set; }

        // Navigation

        public ICollection<RentalOrderDetail> RentalOrderDetails { get; set; }

        public RentalContract RentalContract { get; set; }
        public ICollection<Checkin> Checkins { get; set; }
        public ICollection<Checkout> Checkouts { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<PenaltyRecord> PenaltyRecords { get; set; }
        public ICollection<FeedbackRating> FeedbackRatings { get; set; }
    }
}
