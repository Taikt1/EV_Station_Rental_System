using EV_StationRentalSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.RepositoryContracts
{
    public interface IFeedbackRepository
    {
        Task<FeedbackRating> CreateAsync(FeedbackRating feedback);
        Task<FeedbackRating?> GetByIdAsync(Guid feedbackId);
        Task<List<FeedbackRating>> GetAllAsync();
        Task<List<FeedbackRating>> GetByRentalIdAsync(Guid rentalId);
        Task<List<FeedbackRating>> GetByRenterIdAsync(Guid renterId);
    }
}
