using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.Entities;
using EV_StationRentalSystem.Core.RepositoryContracts;
using EV_StationRentalSystem.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackService(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        public async Task<FeedbackResponse> CreateFeedbackAsync(CreateFeedbackRequest request)
        {
            // Validate and parse GUIDs
            if (!Guid.TryParse(request.RenterId, out var renterId))
                throw new ArgumentException("Invalid RenterId format");
            if (!Guid.TryParse(request.RentalId, out var rentalId))
                throw new ArgumentException("Invalid RentalId format");

            var feedback = new FeedbackRating
            {
                FeedbackId = Guid.NewGuid(),
                RenterId = renterId,
                RentalId = rentalId,
                Score = request.Score,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            var createdFeedback = await _feedbackRepository.CreateAsync(feedback);

            return new FeedbackResponse
            {
                FeedbackId = createdFeedback.FeedbackId.ToString(),
                RenterId = createdFeedback.RenterId.ToString(),
                RentalId = createdFeedback.RentalId.ToString(),
                Score = createdFeedback.Score,
                Comment = createdFeedback.Comment,
                CreatedAt = createdFeedback.CreatedAt
            };
        }

        public async Task<List<FeedbackResponse>> GetAllFeedbacksAsync()
        {
            var feedbacks = await _feedbackRepository.GetAllAsync();

            return feedbacks.Select(f => new FeedbackResponse
            {
                FeedbackId = f.FeedbackId.ToString(),
                RenterId = f.RenterId.ToString(),
                RentalId = f.RentalId.ToString(),
                Score = f.Score,
                Comment = f.Comment,
                CreatedAt = f.CreatedAt
            }).ToList();
        }

        public async Task<FeedbackResponse?> GetFeedbackByIdAsync(Guid feedbackId)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(feedbackId);
            
            if (feedback == null)
                return null;

            return new FeedbackResponse
            {
                FeedbackId = feedback.FeedbackId.ToString(),
                RenterId = feedback.RenterId.ToString(),
                RentalId = feedback.RentalId.ToString(),
                Score = feedback.Score,
                Comment = feedback.Comment,
                CreatedAt = feedback.CreatedAt
            };
        }

        public async Task<List<FeedbackResponse>> GetFeedbacksByRentalIdAsync(Guid rentalId)
        {
            var feedbacks = await _feedbackRepository.GetByRentalIdAsync(rentalId);

            return feedbacks.Select(f => new FeedbackResponse
            {
                FeedbackId = f.FeedbackId.ToString(),
                RenterId = f.RenterId.ToString(),
                RentalId = f.RentalId.ToString(),
                Score = f.Score,
                Comment = f.Comment,
                CreatedAt = f.CreatedAt
            }).ToList();
        }

        public async Task<List<FeedbackResponse>> GetFeedbacksByRenterIdAsync(Guid renterId)
        {
            var feedbacks = await _feedbackRepository.GetByRenterIdAsync(renterId);

            return feedbacks.Select(f => new FeedbackResponse
            {
                FeedbackId = f.FeedbackId.ToString(),
                RenterId = f.RenterId.ToString(),
                RentalId = f.RentalId.ToString(),
                Score = f.Score,
                Comment = f.Comment,
                CreatedAt = f.CreatedAt
            }).ToList();
        }
    }
}
