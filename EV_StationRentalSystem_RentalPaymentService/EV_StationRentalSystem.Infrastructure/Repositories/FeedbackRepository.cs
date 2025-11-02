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
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly RentalPaymentDbContext _context;

        public FeedbackRepository(RentalPaymentDbContext context)
        {
            _context = context;
        }

        public async Task<FeedbackRating> CreateAsync(FeedbackRating feedback)
        {
            await _context.FeedbackRatings.AddAsync(feedback);
            await _context.SaveChangesAsync();
            return feedback;
        }

        public async Task<FeedbackRating?> GetByIdAsync(Guid feedbackId)
        {
            return await _context.FeedbackRatings
                .FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);
        }

        public async Task<List<FeedbackRating>> GetAllAsync()
        {
            return await _context.FeedbackRatings
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<FeedbackRating>> GetByRentalIdAsync(Guid rentalId)
        {
            return await _context.FeedbackRatings
                .Where(f => f.RentalId == rentalId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<FeedbackRating>> GetByRenterIdAsync(Guid renterId)
        {
            return await _context.FeedbackRatings
                .Where(f => f.RenterId == renterId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }
    }
}
