using System;
using System.Collections.Generic;

namespace EV_StationRentalSystem.Core.DTO
{
    public class WorkdayDTO
    {
        public Guid WorkdayId { get; set; }
        public Guid StaffId { get; set; }
        public Guid BranchId { get; set; }
        public DateTime Date { get; set; }
        public List<StaffAssignmentDTO> Assignments { get; set; } = new();

        // Thông tin từ UserService
        public UserProfileResponse? StaffInfo { get; set; }
    }

    public class CreateWorkdayRequest
    {
        public Guid StaffId { get; set; }
        public Guid BranchId { get; set; }
        public DateTime Date { get; set; }
    }

    public class UpdateWorkdayRequest
    {
        public Guid? StaffId { get; set; }
        public Guid? BranchId { get; set; }
        public DateTime? Date { get; set; }
    }

    public class WorkdayFilterRequest
    {
        public Guid? StaffId { get; set; }
        public Guid? BranchId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
