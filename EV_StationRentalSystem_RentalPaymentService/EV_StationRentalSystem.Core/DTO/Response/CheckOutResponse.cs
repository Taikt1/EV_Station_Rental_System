using System;
using System.Collections.Generic;

namespace EV_StationRentalSystem.Core.DTO.Response
{
    public class CheckoutResponse
    {
        public Guid CheckoutId { get; set; }
        public Guid RentalOrderDetailId { get; set; }
        public DateTime Datetime { get; set; }
        public int OdometerReading { get; set; }
        public int BatteryLevel { get; set; }
        public decimal? ExtraFee { get; set; }
        public string? Status { get; set; }
        public List<PhotoProofResponse> Photos { get; set; } = new();

    }
}

