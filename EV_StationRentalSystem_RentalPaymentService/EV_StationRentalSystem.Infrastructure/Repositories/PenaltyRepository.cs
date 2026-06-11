using EV_StationRentalSystem.Core.Entities;
using EV_StationRentalSystem.Core.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Infrastructure.Repositories
{
    public class PenaltyRepository : IPenaltyRepository
    {
        private readonly RentalPaymentDbContext _context;
        public PenaltyRepository(RentalPaymentDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PenaltyRecord penalty)
        {
            _context.PenaltyRecords.Add(penalty);
            await _context.SaveChangesAsync();
        }

        public async Task<PenaltyRecord?> GetByIdAsync(Guid id)
        {
            return await _context.PenaltyRecords.FirstOrDefaultAsync(x => x.PenaltyId == id);
        }

        public async Task<IEnumerable<PenaltyRecord>> GetByRentalIdAsync(Guid rentalId)
        {
            return await _context.PenaltyRecords
                .Where(p => p.RentalId == rentalId)
                .ToListAsync();
        }

    }
}
