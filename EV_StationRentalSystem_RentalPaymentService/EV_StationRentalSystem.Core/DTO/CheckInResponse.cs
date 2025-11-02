using System;

namespace EV_StationRentalSystem.Core.DTO
{
    public class CheckInResponse
    {
        public string CheckinId { get; set; } = string.Empty;
        public string RentalId { get; set; } = string.Empty;
        public DateTime Datetime { get; set; }
        public int OdometerReading { get; set; }
        public int BatteryLevel { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ContractUrl { get; set; }
        public string Message { get; set; } = "Check-in successful";
    }
}

