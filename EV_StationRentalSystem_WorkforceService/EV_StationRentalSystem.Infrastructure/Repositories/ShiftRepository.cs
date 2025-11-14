using EV_StationRentalSystem.Core.Entities;
using EV_StationRentalSystem.Core.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Infrastructure.Repositories
{
    public class ShiftRepository : IShiftRepository
    {
        private readonly WorkforceDbContext _context;

        public ShiftRepository(WorkforceDbContext context)
        {
            _context = context;
        }

        public async Task<Shift?> GetByIdAsync(Guid shiftId)
        {
            return await _context.Shifts.FindAsync(shiftId);
        }

        public async Task<List<Shift>> GetAllAsync()
        {
            return await _context.Shifts.ToListAsync();
        }

        public async Task<Shift> AddAsync(Shift shift)
        {
            await _context.Shifts.AddAsync(shift);
            await _context.SaveChangesAsync();
            return shift;
        }

        public async Task<Shift> UpdateAsync(Shift shift)
        {
            _context.Shifts.Update(shift);
            await _context.SaveChangesAsync();
            return shift;
        }

        public async Task<bool> DeleteAsync(Guid shiftId)
        {
            var shift = await GetByIdAsync(shiftId);
            if (shift == null) return false;

            _context.Shifts.Remove(shift);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid shiftId)
        {
            return await _context.Shifts.AnyAsync(s => s.ShiftId == shiftId);
        }
    }
}
