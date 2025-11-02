using System;

namespace EV_StationRentalSystem.Core.DTO.Response
{
    public class CheckinResponse
    {
        public Guid CheckinId { get; set; }
        public Guid RentalOrderDetailId { get; set; }
        public Guid StaffId { get; set; }
        public DateTime Datetime { get; set; }
        public int OdometerReading { get; set; }
        public int BatteryLevel { get; set; }
        public string? Status { get; set; }

        public List<PhotoProofResponse>? Photos { get; set; }
    }

    public class PhotoProofResponse
    {
        public Guid PhotoId { get; set; }
        public string PhotoUrl { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CapturedAt { get; set; }
    }
}

