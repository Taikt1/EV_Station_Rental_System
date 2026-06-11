using AutoMapper;
using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.Entities;
using EV_StationRentalSystem.Core.RepositoryContracts;
using EV_StationRentalSystem.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Services
{
    public class UserReportService : IUserReportService
    {
        private readonly IUserReportRepository _reportRepository;
        private readonly IMapper _mapper;

        public UserReportService(IUserReportRepository reportRepository, IMapper mapper)
        {
            _reportRepository = reportRepository;
            _mapper = mapper;
        }

        public async Task<UserReportDTO> CreateReportAsync(
            CreateReportRequest request,
            string reporterId,
            string? reporterEmail = null,
            string? reporterName = null)
        {
            var report = new UserReport
            {
                ReportId = Guid.NewGuid(),
                ReporterId = reporterId,
                ReporterEmail = reporterEmail,
                ReporterName = reporterName,
                ReportType = request.ReportType,
                Category = request.Category,
                Priority = request.Priority,
                Title = request.Title,
                Description = request.Description,
                RelatedStationId = request.RelatedStationId,
                RelatedVehicleId = request.RelatedVehicleId,
                RelatedRentalId = request.RelatedRentalId,
                AttachmentUrls = request.AttachmentUrls != null && request.AttachmentUrls.Any()
                    ? JsonSerializer.Serialize(request.AttachmentUrls)
                    : null,
                Status = "pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdReport = await _reportRepository.CreateAsync(report);
            return MapToDTO(createdReport);
        }

        public async Task<List<UserReportDTO>> GetMyReportsAsync(string userId)
        {
            var reports = await _reportRepository.GetByReporterIdAsync(userId);
            return reports.Select(MapToDTO).ToList();
        }

        public async Task<UserReportDTO?> GetReportByIdAsync(Guid reportId, string? userId = null)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null) return null;

            // If userId is provided, check if user is the reporter (for privacy)
            if (!string.IsNullOrEmpty(userId) && report.ReporterId != userId)
            {
                // Only allow if user is staff/manager (would be checked in controller)
                // For now, we allow all authenticated users to view
            }

            return MapToDTO(report);
        }

        public async Task<UserReportDTO?> RateReportAsync(Guid reportId, RateReportRequest request, string userId)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null) return null;

            // Only reporter can rate
            if (report.ReporterId != userId)
                throw new UnauthorizedAccessException("Only the reporter can rate this report");

            // Only resolved/closed reports can be rated
            if (report.Status != "resolved" && report.Status != "closed")
                throw new InvalidOperationException("Only resolved or closed reports can be rated");

            report.UserRating = request.Rating;
            report.UserFeedback = request.Feedback;
            report.UpdatedAt = DateTime.UtcNow;

            var updatedReport = await _reportRepository.UpdateAsync(report);
            return MapToDTO(updatedReport);
        }

        public async Task<ReportListResponse> GetReportsAsync(ReportFilterRequest filter)
        {
            var reports = await _reportRepository.GetByFilterAsync(filter);
            var totalCount = await _reportRepository.GetCountByFilterAsync(filter);

            return new ReportListResponse
            {
                Reports = reports.Select(MapToDTO).ToList(),
                TotalRecords = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<UserReportDTO?> AssignReportAsync(Guid reportId, AssignReportRequest request, string? assignerName = null)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null) return null;

            report.AssignedToStaffId = request.StaffId;
            report.AssignedToStaffName = assignerName;
            report.Status = "in_progress";
            report.StartedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(request.Notes))
            {
                report.StaffNotes = request.Notes;
            }

            report.UpdatedAt = DateTime.UtcNow;

            var updatedReport = await _reportRepository.UpdateAsync(report);
            return MapToDTO(updatedReport);
        }

        public async Task<UserReportDTO?> UpdateReportAsync(Guid reportId, UpdateReportRequest request)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null) return null;

            if (!string.IsNullOrEmpty(request.Priority))
                report.Priority = request.Priority;

            if (!string.IsNullOrEmpty(request.Status))
                report.Status = request.Status;

            if (!string.IsNullOrEmpty(request.AssignedToStaffId))
                report.AssignedToStaffId = request.AssignedToStaffId;

            if (!string.IsNullOrEmpty(request.StaffNotes))
                report.StaffNotes = request.StaffNotes;

            if (!string.IsNullOrEmpty(request.Resolution))
                report.Resolution = request.Resolution;

            report.UpdatedAt = DateTime.UtcNow;

            var updatedReport = await _reportRepository.UpdateAsync(report);
            return MapToDTO(updatedReport);
        }

        public async Task<UserReportDTO?> ResolveReportAsync(Guid reportId, ResolveReportRequest request)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null) return null;

            report.Status = "resolved";
            report.Resolution = request.Resolution;

            if (!string.IsNullOrEmpty(request.StaffNotes))
            {
                report.StaffNotes = string.IsNullOrEmpty(report.StaffNotes)
                    ? request.StaffNotes
                    : $"{report.StaffNotes}\n\n{request.StaffNotes}";
            }

            report.ResolvedAt = DateTime.UtcNow;
            report.UpdatedAt = DateTime.UtcNow;

            var updatedReport = await _reportRepository.UpdateAsync(report);
            return MapToDTO(updatedReport);
        }

        public async Task<UserReportDTO?> CloseReportAsync(Guid reportId)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null) return null;

            report.Status = "closed";
            report.UpdatedAt = DateTime.UtcNow;

            var updatedReport = await _reportRepository.UpdateAsync(report);
            return MapToDTO(updatedReport);
        }

        public async Task<UserReportDTO?> RejectReportAsync(Guid reportId, string reason)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null) return null;

            report.Status = "rejected";
            report.Resolution = $"Rejected: {reason}";
            report.UpdatedAt = DateTime.UtcNow;

            var updatedReport = await _reportRepository.UpdateAsync(report);
            return MapToDTO(updatedReport);
        }

        public async Task<bool> DeleteReportAsync(Guid reportId)
        {
            return await _reportRepository.DeleteAsync(reportId);
        }

        public async Task<ReportStatistics> GetStatisticsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            return await _reportRepository.GetStatisticsAsync(fromDate, toDate);
        }

        public async Task<List<UserReportDTO>> GetMyAssignedReportsAsync(string staffId)
        {
            var reports = await _reportRepository.GetByAssignedStaffIdAsync(staffId);
            return reports.Select(MapToDTO).ToList();
        }

        private UserReportDTO MapToDTO(UserReport report)
        {
            return new UserReportDTO
            {
                ReportId = report.ReportId,
                ReporterId = report.ReporterId,
                ReporterEmail = report.ReporterEmail,
                ReporterName = report.ReporterName,
                ReportType = report.ReportType,
                Category = report.Category,
                Priority = report.Priority,
                Title = report.Title,
                Description = report.Description,
                RelatedStationId = report.RelatedStationId,
                RelatedVehicleId = report.RelatedVehicleId,
                RelatedRentalId = report.RelatedRentalId,
                AttachmentUrls = !string.IsNullOrEmpty(report.AttachmentUrls)
                    ? JsonSerializer.Deserialize<List<string>>(report.AttachmentUrls)
                    : null,
                Status = report.Status,
                AssignedToStaffId = report.AssignedToStaffId,
                AssignedToStaffName = report.AssignedToStaffName,
                StaffNotes = report.StaffNotes,
                Resolution = report.Resolution,
                CreatedAt = report.CreatedAt,
                UpdatedAt = report.UpdatedAt,
                StartedAt = report.StartedAt,
                ResolvedAt = report.ResolvedAt,
                UserRating = report.UserRating,
                UserFeedback = report.UserFeedback
            };
        }
    }
}
