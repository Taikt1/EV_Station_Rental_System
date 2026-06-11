using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.DTO.Response
{
    public class RentalOrderDetailInfoResponse
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
    }
}
