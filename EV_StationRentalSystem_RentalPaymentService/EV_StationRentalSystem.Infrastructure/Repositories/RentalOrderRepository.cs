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
            _context.RentalOrders.Add(rentalOrder);
            await _context.SaveChangesAsync();
            return rentalOrder;
        }

        public async Task<RentalOrder?> GetByIdAsync(Guid rentalId)
        {
            return await _context.RentalOrders
                .Include(r => r.RentalOrderDetails)
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

        public async Task<RentalOrder> UpdateStatusAsync(Guid id, string status)
        {
            var order = await _context.RentalOrders.FindAsync(id);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
            }
            return order;
        }

        public async Task<IEnumerable<RentalOrderDetail>> GetOrderDetailsAsync(Guid orderId)
        {
            return await _context.RentalOrderDetails
                .Where(x => x.RentalOrderId == orderId)
                .ToListAsync();
        }

        public async Task<RentalOrderDetail?> GetOrderDetailByIdAsync(Guid orderDetailId)
        {
            return await _context.RentalOrderDetails
                .FirstOrDefaultAsync(x => x.Id == orderDetailId);
        }

        public async Task<RentalOrder?> GetRentalOrderByDetailIdAsync(Guid orderDetailId)
        {
            return await _context.RentalOrders
                .Include(r => r.RentalOrderDetails)
                .FirstOrDefaultAsync(r => r.RentalOrderDetails != null && 
                                         r.RentalOrderDetails.Any(d => d.Id == orderDetailId));
        }

    }
}
