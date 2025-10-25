using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class FeedbackRating
    {
        [Key]
        public Guid FeedbackId { get; set; }

        public Guid RenterId { get; set; }  // logic ID
        public Guid RentalId { get; set; }

        public int Score { get; set; } // 1–5
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public RentalOrder RentalOrder { get; set; }
    }

}
