using System;
using System.Collections.Generic;

namespace EV_StationRentalSystem.Core.DTO
{
    /// <summary>
    /// Request để tạo báo cáo mới
    /// </summary>
    public class CreateReportRequest
    {
        public string ReportType { get; set; } = string.Empty; // "complaint", "feedback", "technical_issue", etc.
        public string Category { get; set; } = string.Empty; // "station", "vehicle", "payment", etc.
        public string Priority { get; set; } = "medium"; // "low", "medium", "high", "urgent"
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? RelatedStationId { get; set; }
        public Guid? RelatedVehicleId { get; set; }
        public Guid? RelatedRentalId { get; set; }
        public List<string>? AttachmentUrls { get; set; }
    }

    /// <summary>
    /// Request để cập nhật báo cáo
    /// </summary>
    public class UpdateReportRequest
    {
        public string? Priority { get; set; }
        public string? Status { get; set; }
        public string? AssignedToStaffId { get; set; }
        public string? StaffNotes { get; set; }
        public string? Resolution { get; set; }
    }

    /// <summary>
    /// Request để assign báo cáo cho staff
    /// </summary>
    public class AssignReportRequest
    {
        public string StaffId { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Request để giải quyết báo cáo
    /// </summary>
    public class ResolveReportRequest
    {
        public string Resolution { get; set; } = string.Empty;
        public string? StaffNotes { get; set; }
    }

    /// <summary>
    /// Request để đánh giá báo cáo đã giải quyết
    /// </summary>
    public class RateReportRequest
    {
        public int Rating { get; set; } // 1-5
        public string? Feedback { get; set; }
    }

    /// <summary>
    /// Request để filter báo cáo
    /// </summary>
    public class ReportFilterRequest
    {
        public string? ReportType { get; set; }
        public string? Category { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public string? AssignedToStaffId { get; set; }
        public string? ReporterId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SearchKeyword { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    /// <summary>
    /// DTO cho báo cáo
    /// </summary>
    public class UserReportDTO
    {
        public Guid ReportId { get; set; }
        public string ReporterId { get; set; } = string.Empty;
        public string? ReporterEmail { get; set; }
        public string? ReporterName { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? RelatedStationId { get; set; }
        public Guid? RelatedVehicleId { get; set; }
        public Guid? RelatedRentalId { get; set; }
        public List<string>? AttachmentUrls { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? AssignedToStaffId { get; set; }
        public string? AssignedToStaffName { get; set; }
        public string? StaffNotes { get; set; }
        public string? Resolution { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public int? UserRating { get; set; }
        public string? UserFeedback { get; set; }
    }

    /// <summary>
    /// Response cho danh sách báo cáo
    /// </summary>
    public class ReportListResponse
    {
        public List<UserReportDTO> Reports { get; set; } = new();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    }

    /// <summary>
    /// Statistics cho báo cáo
    /// </summary>
    public class ReportStatistics
    {
        public int TotalReports { get; set; }
        public int PendingReports { get; set; }
        public int InProgressReports { get; set; }
        public int ResolvedReports { get; set; }
        public int ClosedReports { get; set; }
        public int RejectedReports { get; set; }
        public Dictionary<string, int> ReportsByType { get; set; } = new();
        public Dictionary<string, int> ReportsByCategory { get; set; } = new();
        public Dictionary<string, int> ReportsByPriority { get; set; } = new();
        public double AverageResolutionTimeHours { get; set; }
        public double AverageRating { get; set; }
    }
}
