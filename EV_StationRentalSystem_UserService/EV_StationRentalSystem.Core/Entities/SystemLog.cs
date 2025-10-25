using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class SystemLog
    {
        [Key]
        public Guid LogId { get; set; }

        [ForeignKey("Account")]
        public string? UserId { get; set; }

        public string Action { get; set; }
        public string EntityType { get; set; }
        public int? EntityId { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string IPAddress { get; set; }

        public ApplicationUser Account { get; set; }
    }

}
