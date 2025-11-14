using System;

namespace EV_StationRentalSystem.Core.DTO
{
    public class StaffAssignmentDTO
    {
        public Guid AssignmentId { get; set; }
        public Guid WorkdayId { get; set; }
        public Guid ShiftId { get; set; }
        public string? Task { get; set; }
        public string Status { get; set; } = "Assigned";
        
        // Navigation info
        public ShiftDTO? Shift { get; set; }
        public WorkdayDTO? Workday { get; set; }
    }

    public class CreateAssignmentRequest
    {
        public Guid WorkdayId { get; set; }
        public Guid ShiftId { get; set; }
        public string? Task { get; set; }
    }

    public class UpdateAssignmentRequest
    {
        public Guid? ShiftId { get; set; }
        public string? Task { get; set; }
        public string? Status { get; set; }
    }

    public class BulkAssignmentRequest
    {
        public Guid StaffId { get; set; }
        public Guid BranchId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Guid> ShiftIds { get; set; } = new();
        public string? Task { get; set; }
    }
}
