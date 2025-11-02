using System;
using System.Collections.Generic;

namespace EV_StationRentalSystem.Core.DTO
{
    public class CheckOutResponse
    {
        public string CheckoutId { get; set; } = string.Empty;
        public string RentalId { get; set; } = string.Empty;
        public DateTime Datetime { get; set; }
        public int OdometerReading { get; set; }
        public int BatteryLevel { get; set; }
        public decimal ExtraFee { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Deposit { get; set; }
        public decimal RefundAmount { get; set; }
        public List<AdditionalFeeInfo> AdditionalFees { get; set; } = new List<AdditionalFeeInfo>();
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = "Check-out successful";
    }

    public class AdditionalFeeInfo
    {
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}

