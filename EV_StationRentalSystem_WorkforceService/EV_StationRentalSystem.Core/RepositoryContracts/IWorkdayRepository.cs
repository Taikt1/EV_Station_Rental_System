using EV_StationRentalSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.RepositoryContracts
{
    public interface IWorkdayRepository
    {
        Task<Workday?> GetByIdAsync(Guid workdayId, bool includeAssignments = false);
        Task<List<Workday>> GetAllAsync(bool includeAssignments = false);
        Task<List<Workday>> GetByStaffIdAsync(Guid staffId, bool includeAssignments = false);
        Task<List<Workday>> GetByBranchIdAsync(Guid branchId, bool includeAssignments = false);
        Task<List<Workday>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, bool includeAssignments = false);
        Task<List<Workday>> GetFilteredAsync(Guid? staffId, Guid? branchId, DateTime? startDate, DateTime? endDate, bool includeAssignments = false);
        Task<Workday> AddAsync(Workday workday);
        Task<Workday> UpdateAsync(Workday workday);
        Task<bool> DeleteAsync(Guid workdayId);
        Task<bool> ExistsAsync(Guid workdayId);
        Task<bool> StaffHasWorkdayOnDateAsync(Guid staffId, DateTime date);
    }
}
