using EV_StationRentalSystem.Core.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.ServiceContracts
{
    public interface IWorkforceService
    {
        // Shift Management
        Task<ShiftDTO?> GetShiftByIdAsync(Guid shiftId);
        Task<List<ShiftDTO>> GetAllShiftsAsync();
        Task<ShiftDTO> CreateShiftAsync(CreateShiftRequest request);
        Task<ShiftDTO?> UpdateShiftAsync(Guid shiftId, UpdateShiftRequest request);
        Task<bool> DeleteShiftAsync(Guid shiftId);

        // Workday Management
        Task<WorkdayDTO?> GetWorkdayByIdAsync(Guid workdayId, string? authToken = null);
        Task<List<WorkdayDTO>> GetWorkdaysByFilterAsync(WorkdayFilterRequest filter, string? authToken = null);
        Task<WorkdayDTO> CreateWorkdayAsync(CreateWorkdayRequest request);
        Task<WorkdayDTO?> UpdateWorkdayAsync(Guid workdayId, UpdateWorkdayRequest request);
        Task<bool> DeleteWorkdayAsync(Guid workdayId);

        // Assignment Management
        Task<StaffAssignmentDTO?> GetAssignmentByIdAsync(Guid assignmentId);
        Task<List<StaffAssignmentDTO>> GetAssignmentsByWorkdayAsync(Guid workdayId);
        Task<StaffAssignmentDTO> CreateAssignmentAsync(CreateAssignmentRequest request);
        Task<StaffAssignmentDTO?> UpdateAssignmentAsync(Guid assignmentId, UpdateAssignmentRequest request);
        Task<bool> DeleteAssignmentAsync(Guid assignmentId);
        Task<List<StaffAssignmentDTO>> CreateBulkAssignmentsAsync(BulkAssignmentRequest request);

        // Special Queries
        Task<List<WorkdayDTO>> GetStaffScheduleAsync(Guid staffId, DateTime startDate, DateTime endDate, string? authToken = null);
        Task<List<WorkdayDTO>> GetBranchScheduleAsync(Guid branchId, DateTime startDate, DateTime endDate, string? authToken = null);
    }
}
