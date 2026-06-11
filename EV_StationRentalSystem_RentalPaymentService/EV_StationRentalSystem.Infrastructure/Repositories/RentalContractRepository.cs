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
    public class RentalContractRepository : IRentalContractRepository
    {
        private readonly RentalPaymentDbContext _context;

        public RentalContractRepository(RentalPaymentDbContext context)
        {
            _context = context;
        }

        public async Task<RentalContract?> GetByIdAsync(int contractId)
        {
            return await _context.RentalContracts
                .Include(c => c.RentalOrder)
                .FirstOrDefaultAsync(c => c.ContractId == contractId);
        }

        public async Task<RentalContract?> GetByRentalIdAsync(Guid rentalId)
        {
            return await _context.RentalContracts
                .Include(c => c.RentalOrder)
                .FirstOrDefaultAsync(c => c.RentalId == rentalId);
        }

        public async Task<IEnumerable<RentalContract>> GetByUserOrStaffAsync(Guid? renterId, Guid? staffId)
        {
            var query = _context.RentalContracts
                .Include(c => c.RentalOrder)
                .AsQueryable();

            if (renterId.HasValue)
                query = query.Where(c => c.RentalOrder.RenterId == renterId.Value);

            if (staffId.HasValue)
                query = query.Where(c => c.RentalOrder.StaffId == staffId.Value);

            return await query.ToListAsync();
        }

        public async Task AddAsync(RentalContract contract)
        {
            await _context.RentalContracts.AddAsync(contract);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RentalContract contract)
        {
            _context.RentalContracts.Update(contract);
            await _context.SaveChangesAsync();
        }
    }
}
