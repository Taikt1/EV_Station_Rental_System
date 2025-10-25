using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class Payment
    {
        [Key]
        public Guid PaymentId { get; set; }

        [ForeignKey("RentalOrder")]
        public Guid RentalId { get; set; }

        public string? PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentTime { get; set; }
        public string? Status { get; set; } // Pending, Paid, Failed, Refunded
        public string? TransactionRef { get; set; }

        public RentalOrder RentalOrder { get; set; }
    }

}
