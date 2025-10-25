using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class Checkin
    {
        [Key]
        public Guid CheckinId { get; set; }

        public Guid RentalOrderDetailId { get; set; }  // FK thật
        public Guid StaffId { get; set; }   // logic ID

        public DateTime Datetime { get; set; }
        public int OdometerReading { get; set; }
        public int BatteryLevel { get; set; }
        public string? Status { get; set; } // Pending, Confirmed, Cancelled

        // Navigation
        public RentalOrderDetail RentalOrderDetail { get; set; } = null!;
        public ICollection<PhotoProof> PhotoProofs { get; set; } = null!;
    }

}
