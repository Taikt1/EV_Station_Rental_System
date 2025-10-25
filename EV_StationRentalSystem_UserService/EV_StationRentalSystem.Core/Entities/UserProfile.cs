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
    public class UserProfile
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Account")]
        public string UserId { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        public DateTime? Dob { get; set; }

        [MaxLength(255)]
        public string Address { get; set; }

        public string AvatarUrl { get; set; }
        public string CCCDUrl { get; set; }

        // Navigation
        public ApplicationUser Account { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public PersonalAnalytics PersonalAnalytics { get; set; }
    }

}
