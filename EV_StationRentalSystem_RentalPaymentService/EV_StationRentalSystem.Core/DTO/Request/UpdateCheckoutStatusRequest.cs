using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.DTO.Request
{
    public class UpdateCheckoutStatusRequest
    {
        public string Status { get; set; } = null!;
    }
}
