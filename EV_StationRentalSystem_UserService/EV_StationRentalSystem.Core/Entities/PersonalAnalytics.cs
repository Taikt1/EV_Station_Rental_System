using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class PersonalAnalytics
    {
        [Key]
        public Guid AnalyticsId { get; set; }

        [ForeignKey("UserProfile")]
        public Guid UserId { get; set; }

        public int TotalRentals { get; set; }
        public double TotalDistance { get; set; }
        public decimal TotalSpent { get; set; }
        public string FavoriteBranch { get; set; }
        public string PeakRentTime { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;


        public UserProfile UserProfile { get; set; }
    }

}
