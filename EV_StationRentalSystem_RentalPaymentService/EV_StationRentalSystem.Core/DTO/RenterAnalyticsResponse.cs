using System;
using System.Collections.Generic;

namespace EV_StationRentalSystem.Core.DTO
{
    /// <summary>
    /// Response cho phân tích cá nhân của Renter
    /// </summary>
    public class RenterAnalyticsResponse
    {
        public string RenterId { get; set; } = string.Empty;
        
        // Tổng quan
        public int TotalRentals { get; set; }
        public int CompletedRentals { get; set; }
        public int CancelledRentals { get; set; }
        public int ActiveRentals { get; set; }
        
        // Chi phí
        public decimal TotalSpent { get; set; }
        public decimal AverageSpentPerRental { get; set; }
        public decimal TotalPenalties { get; set; }
        
        // Đánh giá
        public int TotalFeedbacks { get; set; }
        public double AverageRating { get; set; }
        
        // Thời gian thuê (giờ)
        public double TotalRentalHours { get; set; }
        public double AverageRentalHours { get; set; }
        
        // Phân tích thời gian
        public Dictionary<string, int> RentalsByDayOfWeek { get; set; } = new();
        public Dictionary<int, int> RentalsByHour { get; set; } = new();
        public Dictionary<string, int> RentalsByMonth { get; set; } = new();
        
        // Top branches
        public List<BranchUsageInfo> TopStartBranches { get; set; } = new();
        public List<BranchUsageInfo> TopEndBranches { get; set; } = new();
        
        // Lịch sử gần đây
        public List<RecentRentalInfo> RecentRentals { get; set; } = new();
    }

    public class BranchUsageInfo
    {
        public string BranchId { get; set; } = string.Empty;
        public int UsageCount { get; set; }
        public decimal TotalSpent { get; set; }
    }

    public class RecentRentalInfo
    {
        public string RentalId { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal? ActualCost { get; set; }
    }
}
