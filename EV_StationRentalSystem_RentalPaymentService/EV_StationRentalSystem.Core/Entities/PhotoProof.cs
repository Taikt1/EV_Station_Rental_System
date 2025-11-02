using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class PhotoProof
    {
        [Key]
        public Guid PhotoId { get; set; }

        //public Guid RentalId { get; set; }
        public Guid? CheckinId { get; set; }
        public Guid? CheckoutId { get; set; }

        [Required]
        public string? PhotoUrl { get; set; }

        public string? Description { get; set; }
        public DateTime CapturedAt { get; set; } = DateTime.UtcNow;

        //public RentalOrder RentalOrder { get; set; }
        public Checkin Checkin { get; set; }
        public Checkout Checkout { get; set; }
    }

}
