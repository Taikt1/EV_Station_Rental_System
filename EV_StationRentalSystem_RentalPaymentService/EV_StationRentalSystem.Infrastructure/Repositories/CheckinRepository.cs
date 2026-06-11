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
    public class CheckinRepository : ICheckinRepository
    {
        private readonly RentalPaymentDbContext _context;

        public CheckinRepository(RentalPaymentDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<Checkin>> GetPagedAsync(int pageIndex, int pageSize)
        {
            var query = _context.Checkins
                .Include(c => c.PhotoProofs)
                .OrderByDescending(c => c.Datetime).AsNoTracking();

            var total = await query.CountAsync();
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<Checkin>
            {
                TotalCount = total,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Data = items
            };
        }

        public async Task<Checkin?> GetByOrderIdAsync(Guid orderId)
        {
            return await _context.Checkins
                .Include(c => c.PhotoProofs)
                .FirstOrDefaultAsync(c => c.RentalOrderDetailId == orderId);
        }

        public async Task<Checkin?> GetByIdAsync(Guid id)
        {
            return await _context.Checkins
                .Include(c => c.PhotoProofs)
                .FirstOrDefaultAsync(c => c.CheckinId == id);
        }

        public async Task<Checkin> AddAsync(Checkin entity)
        {
            _context.Checkins.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Checkin> UpdateAsync(Checkin checkin)
        {
            _context.Checkins.Update(checkin);
            await _context.SaveChangesAsync();
            return checkin;
        }
    }
}
