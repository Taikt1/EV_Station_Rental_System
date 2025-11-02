using EV_StationRentalSystem.Core.Entities;
using EV_StationRentalSystem.Core.RepositoryContracts;
using EV_StationRentalSystem.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Infrastructure.Repositories
{
    public class RentalOrderRepository : IRentalOrderRepository
    {
        private readonly RentalPaymentDbContext _context;

        public RentalOrderRepository(RentalPaymentDbContext context)
        {
            _context = context;
        }

        public async Task<RentalOrder> CreateAsync(RentalOrder rentalOrder)
        {
            await _context.RentalOrders.AddAsync(rentalOrder);
            await _context.SaveChangesAsync();
            return rentalOrder;
        }

        public async Task<RentalOrder?> GetByIdAsync(Guid rentalId)
        {
            return await _context.RentalOrders
                .Include(r => r.Payments)
                .Include(r => r.FeedbackRatings)
                .Include(r => r.PenaltyRecords)
                .FirstOrDefaultAsync(r => r.RentalId == rentalId);
        }

        public async Task<List<RentalOrder>> GetAllAsync()
        {
            return await _context.RentalOrders
                .OrderByDescending(r => r.StartTime)
                .ToListAsync();
        }

        public async Task<List<RentalOrder>> GetByRenterIdAsync(Guid renterId)
        {
            return await _context.RentalOrders
                .Where(r => r.RenterId == renterId)
                .Include(r => r.Payments)
                .Include(r => r.FeedbackRatings)
                .OrderByDescending(r => r.StartTime)
                .ToListAsync();
        }

        public async Task<RentalOrder> UpdateAsync(RentalOrder rentalOrder)
        {
            _context.RentalOrders.Update(rentalOrder);
            await _context.SaveChangesAsync();
            return rentalOrder;
        }

        public async Task<bool> DeleteAsync(Guid rentalId)
        {
            var rentalOrder = await _context.RentalOrders.FindAsync(rentalId);
            if (rentalOrder == null)
                return false;

            _context.RentalOrders.Remove(rentalOrder);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
