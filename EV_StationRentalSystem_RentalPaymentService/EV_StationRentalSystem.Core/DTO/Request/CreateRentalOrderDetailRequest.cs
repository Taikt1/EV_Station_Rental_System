using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.DTO.Request
{
    public class CreateRentalOrderDetailRequest
    {
        [Required]
        public Guid VehicleId { get; set; }
    }
}
