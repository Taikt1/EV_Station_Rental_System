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

        // Foreign Keys - CẢ 2 CỘT ĐỀU CÓ TRONG DB
        public Guid RentalId { get; set; }  // ✅ Cột cũ trong DB
        public Guid? CheckinId { get; set; }
        public Guid? CheckoutId { get; set; }
        public Guid RentalOrderRentalId { get; set; }  // ✅ Shadow property trong DB

        [Required]
        public string? PhotoUrl { get; set; }

        public string? Description { get; set; }
        public DateTime CapturedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Checkin Checkin { get; set; }
        public Checkout Checkout { get; set; }
        public RentalOrder RentalOrder { get; set; }
    }

}
