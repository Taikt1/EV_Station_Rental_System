using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? Status { get; set; } = "Active";

        public UserProfile? UserProfile { get; set; }
        public ICollection<SystemLog>? SystemLogs { get; set; }
    }
}
