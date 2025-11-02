using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.DTO.Request
{
    public class CreateCheckoutRequest
    {
        public Guid RentalOrderDetailId { get; set; }
        public Guid StaffId { get; set; }
        public int OdometerReading { get; set; }
        public int BatteryLevel { get; set; }
        public decimal? ExtraFee { get; set; }
        public string? Status { get; set; }
        public List<PhotoProofRequest>? Photos { get; set; }
    }
}
