using EV_StationRentalSystem.Core.DTO.Response;
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
    public class CheckoutRepository : ICheckoutRepository
    {
        private readonly RentalPaymentDbContext _context;
        public CheckoutRepository(RentalPaymentDbContext context)
        {
            _context = context;
        }

        public async Task<Checkout?> GetByOrderDetailIdAsync(Guid orderDetailId)
        {
            return await _context.Checkouts
                .Include(c => c.PhotoProofs)
                .FirstOrDefaultAsync(c => c.RentalOrderDetailId == orderDetailId);
        }

        public async Task<Checkout> AddAsync(Checkout checkout)
        {
            _context.Checkouts.Add(checkout);
            await _context.SaveChangesAsync();
            return checkout;
        }

        public async Task<PagedResponse<Checkout>> GetPagedAsync(int pageIndex, int pageSize)
        {
            var query = _context.Checkouts
                .Include(c => c.PhotoProofs)
                .OrderByDescending(c => c.Datetime).AsNoTracking();

            var total = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new PagedResponse<Checkout>
            {
                TotalCount = total,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Data = items
            };
        }

        public async Task<Checkout> UpdateAsync(Checkout checkout)
        {
            _context.Checkouts.Update(checkout);
            await _context.SaveChangesAsync();
            return checkout;
        }

        public async Task<Checkout?> GetByIdAsync(Guid id)
        {
            return await _context.Checkouts
                .Include(c => c.PhotoProofs)
                .FirstOrDefaultAsync(c => c.CheckoutId == id);
        }
    }
}
