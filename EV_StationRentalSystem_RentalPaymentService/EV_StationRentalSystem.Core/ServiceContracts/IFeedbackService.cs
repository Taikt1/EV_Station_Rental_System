using EV_StationRentalSystem.Core.DTO.Request;
using EV_StationRentalSystem.Core.DTO.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.ServiceContracts
{
    public interface IFeedbackService
    {
        Task<FeedbackResponse> CreateFeedbackAsync(CreateFeedbackRequest request);
        Task<FeedbackResponse?> GetFeedbackByIdAsync(Guid feedbackId);
        Task<List<FeedbackResponse>> GetAllFeedbacksAsync();
        Task<List<FeedbackResponse>> GetFeedbacksByRentalIdAsync(Guid rentalId);
        Task<List<FeedbackResponse>> GetFeedbacksByRenterIdAsync(Guid renterId);
    }
}
