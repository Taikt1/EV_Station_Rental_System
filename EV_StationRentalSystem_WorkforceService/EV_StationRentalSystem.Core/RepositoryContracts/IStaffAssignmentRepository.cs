using EV_StationRentalSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.RepositoryContracts
{
    public interface IStaffAssignmentRepository
    {
        Task<StaffAssignment?> GetByIdAsync(Guid assignmentId, bool includeRelations = false);
        Task<List<StaffAssignment>> GetAllAsync(bool includeRelations = false);
        Task<List<StaffAssignment>> GetByWorkdayIdAsync(Guid workdayId, bool includeRelations = false);
        Task<List<StaffAssignment>> GetByShiftIdAsync(Guid shiftId, bool includeRelations = false);
        Task<StaffAssignment> AddAsync(StaffAssignment assignment);
        Task<List<StaffAssignment>> AddRangeAsync(List<StaffAssignment> assignments);
        Task<StaffAssignment> UpdateAsync(StaffAssignment assignment);
        Task<bool> DeleteAsync(Guid assignmentId);
        Task<bool> ExistsAsync(Guid assignmentId);
        Task<bool> WorkdayHasShiftAsync(Guid workdayId, Guid shiftId);
    }
}
