using System;
using System.ComponentModel.DataAnnotations;

namespace EV_StationRentalSystem.Core.Entities
{
    /// <summary>
    /// Entity để lưu các báo cáo/khiếu nại từ người dùng
    /// </summary>
    public class UserReport
    {
        [Key]
        public Guid ReportId { get; set; }

        /// <summary>
        /// User ID của người tạo báo cáo
        /// </summary>
        [Required]
        [MaxLength(450)]
        public string ReporterId { get; set; } = string.Empty;

        /// <summary>
        /// Email của người báo cáo (denormalized để dễ truy vấn)
        /// </summary>
        [MaxLength(100)]
        public string? ReporterEmail { get; set; }

        /// <summary>
        /// Tên người báo cáo (denormalized)
        /// </summary>
        [MaxLength(100)]
        public string? ReporterName { get; set; }

        /// <summary>
        /// Loại báo cáo: "complaint", "feedback", "technical_issue", "safety_issue", "other"
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ReportType { get; set; } = string.Empty;

        /// <summary>
        /// Category: "station", "vehicle", "payment", "staff", "app", "other"
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Mức độ ưu tiên: "low", "medium", "high", "urgent"
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Priority { get; set; } = "medium";

        /// <summary>
        /// Tiêu đề báo cáo
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Nội dung chi tiết
        /// </summary>
        [Required]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Station/Branch ID liên quan (nếu có)
        /// </summary>
        public Guid? RelatedStationId { get; set; }

        /// <summary>
        /// Vehicle ID liên quan (nếu có)
        /// </summary>
        public Guid? RelatedVehicleId { get; set; }

        /// <summary>
        /// Rental ID liên quan (nếu có)
        /// </summary>
        public Guid? RelatedRentalId { get; set; }

        /// <summary>
        /// URLs của hình ảnh đính kèm (JSON array)
        /// </summary>
        public string? AttachmentUrls { get; set; }

        /// <summary>
        /// Trạng thái: "pending", "in_progress", "resolved", "closed", "rejected"
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "pending";

        /// <summary>
        /// Staff ID đang xử lý
        /// </summary>
        [MaxLength(450)]
        public string? AssignedToStaffId { get; set; }

        /// <summary>
        /// Tên staff xử lý
        /// </summary>
        [MaxLength(100)]
        public string? AssignedToStaffName { get; set; }

        /// <summary>
        /// Ghi chú từ staff
        /// </summary>
        public string? StaffNotes { get; set; }

        /// <summary>
        /// Phản hồi cho người dùng
        /// </summary>
        public string? Resolution { get; set; }

        /// <summary>
        /// Thời gian tạo báo cáo
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Thời gian cập nhật cuối
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Thời gian bắt đầu xử lý
        /// </summary>
        public DateTime? StartedAt { get; set; }

        /// <summary>
        /// Thời gian hoàn thành
        /// </summary>
        public DateTime? ResolvedAt { get; set; }

        /// <summary>
        /// Đánh giá từ người dùng sau khi xử lý (1-5)
        /// </summary>
        public int? UserRating { get; set; }

        /// <summary>
        /// Nhận xét từ người dùng
        /// </summary>
        public string? UserFeedback { get; set; }
    }
}
