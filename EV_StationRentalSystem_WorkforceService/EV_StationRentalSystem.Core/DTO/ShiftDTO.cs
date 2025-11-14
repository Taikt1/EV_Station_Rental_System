using System;

namespace EV_StationRentalSystem.Core.DTO
{
    public class ShiftDTO
    {
        public Guid ShiftId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class CreateShiftRequest
    {
        public string ShiftName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class UpdateShiftRequest
    {
        public string? ShiftName { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
