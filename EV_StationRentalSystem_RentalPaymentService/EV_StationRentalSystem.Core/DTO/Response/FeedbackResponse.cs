using System;

namespace EV_StationRentalSystem.Core.DTO.Response
{
    public class FeedbackResponse
    {
        public string FeedbackId { get; set; } = string.Empty;
        public string RenterId { get; set; } = string.Empty;
        public string RentalId { get; set; } = string.Empty;
        public int Score { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
