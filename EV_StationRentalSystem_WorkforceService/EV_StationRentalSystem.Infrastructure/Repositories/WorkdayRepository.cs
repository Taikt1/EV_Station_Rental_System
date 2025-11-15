using EV_StationRentalSystem.Core.Entities;
using EV_StationRentalSystem.Core.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Infrastructure.Repositories
{
    public class WorkdayRepository : IWorkdayRepository
    {
        private readonly WorkforceDbContext _context;

        public WorkdayRepository(WorkforceDbContext context)
        {
            _context = context;
        }

        public async Task<Workday?> GetByIdAsync(Guid workdayId, bool includeAssignments = false)
        {
            var query = _context.Workdays.AsQueryable();

            if (includeAssignments)
            {
                query = query.Include(w => w.StaffAssignments)
                            .ThenInclude(a => a.Shift);
            }

            return await query.FirstOrDefaultAsync(w => w.WorkdayId == workdayId);
        }

        public async Task<List<Workday>> GetAllAsync(bool includeAssignments = false)
        {
            var query = _context.Workdays.AsQueryable();

            if (includeAssignments)
            {
                query = query.Include(w => w.StaffAssignments)
                            .ThenInclude(a => a.Shift);
            }

            return await query.ToListAsync();
        }

        public async Task<List<Workday>> GetByStaffIdAsync(Guid staffId, bool includeAssignments = false)
        {
            var query = _context.Workdays.Where(w => w.StaffId == staffId);

            if (includeAssignments)
            {
                query = query.Include(w => w.StaffAssignments)
                            .ThenInclude(a => a.Shift);
            }

            return await query.OrderBy(w => w.Date).ToListAsync();
        }

        public async Task<List<Workday>> GetByBranchIdAsync(Guid branchId, bool includeAssignments = false)
        {
            var query = _context.Workdays.Where(w => w.BranchId == branchId);

            if (includeAssignments)
            {
                query = query.Include(w => w.StaffAssignments)
                            .ThenInclude(a => a.Shift);
            }

            return await query.OrderBy(w => w.Date).ToListAsync();
        }

        public async Task<List<Workday>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, bool includeAssignments = false)
        {
            var query = _context.Workdays.Where(w => w.Date >= startDate && w.Date <= endDate);

            if (includeAssignments)
            {
                query = query.Include(w => w.StaffAssignments)
                            .ThenInclude(a => a.Shift);
            }

            return await query.OrderBy(w => w.Date).ToListAsync();
        }

        public async Task<List<Workday>> GetFilteredAsync(Guid? staffId, Guid? branchId, DateTime? startDate, DateTime? endDate, bool includeAssignments = false)
        {
            var query = _context.Workdays.AsQueryable();

            if (staffId.HasValue)
                query = query.Where(w => w.StaffId == staffId.Value);

            if (branchId.HasValue)
                query = query.Where(w => w.BranchId == branchId.Value);

            if (startDate.HasValue)
                query = query.Where(w => w.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(w => w.Date <= endDate.Value);

            if (includeAssignments)
            {
                query = query.Include(w => w.StaffAssignments)
                            .ThenInclude(a => a.Shift);
            }

            return await query.OrderBy(w => w.Date).ToListAsync();
        }

        public async Task<Workday> AddAsync(Workday workday)
        {
            await _context.Workdays.AddAsync(workday);
            await _context.SaveChangesAsync();
            return workday;
        }

        public async Task<Workday> UpdateAsync(Workday workday)
        {
            _context.Workdays.Update(workday);
            await _context.SaveChangesAsync();
            return workday;
        }

        public async Task<bool> DeleteAsync(Guid workdayId)
        {
            var workday = await GetByIdAsync(workdayId);
            if (workday == null) return false;

            _context.Workdays.Remove(workday);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid workdayId)
        {
            return await _context.Workdays.AnyAsync(w => w.WorkdayId == workdayId);
        }

        public async Task<bool> StaffHasWorkdayOnDateAsync(Guid staffId, DateTime date)
        {
            return await _context.Workdays.AnyAsync(w => w.StaffId == staffId && w.Date.Date == date.Date);
        }
    }
}
