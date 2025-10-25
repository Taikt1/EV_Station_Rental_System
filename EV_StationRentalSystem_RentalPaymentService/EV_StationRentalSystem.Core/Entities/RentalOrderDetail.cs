using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class RentalOrderDetail
    {
        public Guid Id { get; set; }

        public Guid RentalOrderId { get; set; }

        public Guid VehicleId { get; set; } // logic, từ Fleet Service

        public DateTime AssignedAt { get; set; }
        public DateTime? ReturnedAt { get; set; }


        public RentalOrder RentalOrder { get; set; } = null!;
        public ICollection<Checkin> Checkins { get; set; } = null!;
        public ICollection<Checkout> Checkouts { get; set; } = null!;
    }

}
