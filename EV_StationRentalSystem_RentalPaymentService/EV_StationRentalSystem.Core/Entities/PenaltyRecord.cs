using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class PenaltyRecord
    {
        [Key]
        public Guid PenaltyId { get; set; }

        public Guid RentalId { get; set; }
        public string? Reason { get; set; }
        public decimal PenaltyAmount { get; set; }
        public Guid IssuedBy { get; set; } // logic ID → Staff
        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;

        public RentalOrder RentalOrder { get; set; }
    }

}
