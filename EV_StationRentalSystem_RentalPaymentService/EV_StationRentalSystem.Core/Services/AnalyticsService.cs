using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.RepositoryContracts;
using EV_StationRentalSystem.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IRentalOrderRepository _rentalOrderRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IFeedbackRepository _feedbackRepository;

        public AnalyticsService(
            IRentalOrderRepository rentalOrderRepository,
            IPaymentRepository paymentRepository,
            IFeedbackRepository feedbackRepository)
        {
            _rentalOrderRepository = rentalOrderRepository;
            _paymentRepository = paymentRepository;
            _feedbackRepository = feedbackRepository;
        }

        public async Task<RenterAnalyticsResponse> GetRenterAnalyticsAsync(Guid renterId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            // Lấy tất cả đơn thuê của renter
            var rentals = await _rentalOrderRepository.GetByRenterIdAsync(renterId);
            
            // Apply date filter
            if (fromDate.HasValue)
                rentals = rentals.Where(r => r.StartTime >= fromDate.Value).ToList();
            if (toDate.HasValue)
                rentals = rentals.Where(r => r.StartTime <= toDate.Value).ToList();

            // Lấy payments và feedbacks
            var payments = await _paymentRepository.GetByRenterIdAsync(renterId);
            var feedbacks = await _feedbackRepository.GetByRenterIdAsync(renterId);

            // Tính toán thống kê
            var analytics = new RenterAnalyticsResponse
            {
                RenterId = renterId.ToString(),
                
                // Tổng quan
                TotalRentals = rentals.Count,
                CompletedRentals = rentals.Count(r => r.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase)),
                CancelledRentals = rentals.Count(r => r.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase)),
                ActiveRentals = rentals.Count(r => r.Status.Equals("Active", StringComparison.OrdinalIgnoreCase) || 
                                                   r.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)),
                
                // Chi phí
                TotalSpent = payments.Where(p => p.Status == "Paid").Sum(p => p.Amount),
                AverageSpentPerRental = rentals.Count > 0 
                    ? rentals.Where(r => r.ActualCost.HasValue).Average(r => r.ActualCost ?? 0) 
                    : 0,
                TotalPenalties = 0, // TODO: Calculate from PenaltyRecords if needed
                
                // Đánh giá
                TotalFeedbacks = feedbacks.Count,
                AverageRating = feedbacks.Count > 0 ? feedbacks.Average(f => f.Score) : 0,
                
                // Thời gian thuê
                TotalRentalHours = rentals
                    .Where(r => r.EndTime.HasValue)
                    .Sum(r => (r.EndTime!.Value - r.StartTime).TotalHours),
                AverageRentalHours = rentals
                    .Where(r => r.EndTime.HasValue)
                    .DefaultIfEmpty()
                    .Average(r => r != null && r.EndTime.HasValue ? (r.EndTime.Value - r.StartTime).TotalHours : 0),
            };

            // Phân tích theo ngày trong tuần
            analytics.RentalsByDayOfWeek = rentals
                .GroupBy(r => r.StartTime.DayOfWeek.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            // Phân tích theo giờ trong ngày
            analytics.RentalsByHour = rentals
                .GroupBy(r => r.StartTime.Hour)
                .ToDictionary(g => g.Key, g => g.Count());

            // Phân tích theo tháng
            analytics.RentalsByMonth = rentals
                .GroupBy(r => r.StartTime.ToString("yyyy-MM"))
                .ToDictionary(g => g.Key, g => g.Count());

            // Top branches sử dụng nhiều nhất (Start)
            analytics.TopStartBranches = rentals
                .GroupBy(r => r.BranchStartId)
                .Select(g => new BranchUsageInfo
                {
                    BranchId = g.Key.ToString(),
                    UsageCount = g.Count(),
                    TotalSpent = g.Where(r => r.ActualCost.HasValue).Sum(r => r.ActualCost ?? 0)
                })
                .OrderByDescending(b => b.UsageCount)
                .Take(5)
                .ToList();

            // Top branches sử dụng nhiều nhất (End)
            analytics.TopEndBranches = rentals
                .GroupBy(r => r.BranchEndId)
                .Select(g => new BranchUsageInfo
                {
                    BranchId = g.Key.ToString(),
                    UsageCount = g.Count(),
                    TotalSpent = g.Where(r => r.ActualCost.HasValue).Sum(r => r.ActualCost ?? 0)
                })
                .OrderByDescending(b => b.UsageCount)
                .Take(5)
                .ToList();

            // Lịch sử gần đây (5 đơn gần nhất)
            analytics.RecentRentals = rentals
                .OrderByDescending(r => r.StartTime)
                .Take(5)
                .Select(r => new RecentRentalInfo
                {
                    RentalId = r.RentalId.ToString(),
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    Status = r.Status,
                    ActualCost = r.ActualCost
                })
                .ToList();

            return analytics;
        }
    }
}
