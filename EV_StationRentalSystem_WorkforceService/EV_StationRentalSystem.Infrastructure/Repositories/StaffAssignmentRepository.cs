using EV_StationRentalSystem.Core.Entities;
using EV_StationRentalSystem.Core.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Infrastructure.Repositories
{
    public class StaffAssignmentRepository : IStaffAssignmentRepository
    {
        private readonly WorkforceDbContext _context;

        public StaffAssignmentRepository(WorkforceDbContext context)
        {
            _context = context;
        }

        public async Task<StaffAssignment?> GetByIdAsync(Guid assignmentId, bool includeRelations = false)
        {
            var query = _context.StaffAssignments.AsQueryable();

            if (includeRelations)
            {
                query = query.Include(a => a.Shift)
                            .Include(a => a.Workday);
            }

            return await query.FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);
        }

        public async Task<List<StaffAssignment>> GetAllAsync(bool includeRelations = false)
        {
            var query = _context.StaffAssignments.AsQueryable();

            if (includeRelations)
            {
                query = query.Include(a => a.Shift)
                            .Include(a => a.Workday);
            }

            return await query.ToListAsync();
        }

        public async Task<List<StaffAssignment>> GetByWorkdayIdAsync(Guid workdayId, bool includeRelations = false)
        {
            var query = _context.StaffAssignments.Where(a => a.WorkdayId == workdayId);

            if (includeRelations)
            {
                query = query.Include(a => a.Shift)
                            .Include(a => a.Workday);
            }

            return await query.ToListAsync();
        }

        public async Task<List<StaffAssignment>> GetByShiftIdAsync(Guid shiftId, bool includeRelations = false)
        {
            var query = _context.StaffAssignments.Where(a => a.ShiftId == shiftId);

            if (includeRelations)
            {
                query = query.Include(a => a.Shift)
                            .Include(a => a.Workday);
            }

            return await query.ToListAsync();
        }

        public async Task<StaffAssignment> AddAsync(StaffAssignment assignment)
        {
            await _context.StaffAssignments.AddAsync(assignment);
            await _context.SaveChangesAsync();
            return assignment;
        }

        public async Task<List<StaffAssignment>> AddRangeAsync(List<StaffAssignment> assignments)
        {
            await _context.StaffAssignments.AddRangeAsync(assignments);
            await _context.SaveChangesAsync();
            return assignments;
        }

        public async Task<StaffAssignment> UpdateAsync(StaffAssignment assignment)
        {
            _context.StaffAssignments.Update(assignment);
            await _context.SaveChangesAsync();
            return assignment;
        }

        public async Task<bool> DeleteAsync(Guid assignmentId)
        {
            var assignment = await GetByIdAsync(assignmentId);
            if (assignment == null) return false;

            _context.StaffAssignments.Remove(assignment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid assignmentId)
        {
            return await _context.StaffAssignments.AnyAsync(a => a.AssignmentId == assignmentId);
        }

        public async Task<bool> WorkdayHasShiftAsync(Guid workdayId, Guid shiftId)
        {
            return await _context.StaffAssignments
                .AnyAsync(a => a.WorkdayId == workdayId && a.ShiftId == shiftId);
        }
    }
}
