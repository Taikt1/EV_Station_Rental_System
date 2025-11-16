using EV_StationRentalSystem.Core.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.ServiceContracts
{
    public interface IUserReportService
    {
        // User creates report
        Task<UserReportDTO> CreateReportAsync(CreateReportRequest request, string reporterId, string? reporterEmail = null, string? reporterName = null);

        // User views their own reports
        Task<List<UserReportDTO>> GetMyReportsAsync(string userId);

        // User views specific report
        Task<UserReportDTO?> GetReportByIdAsync(Guid reportId, string? userId = null);

        // User rates resolved report
        Task<UserReportDTO?> RateReportAsync(Guid reportId, RateReportRequest request, string userId);

        // Staff/Manager views all reports with filter
        Task<ReportListResponse> GetReportsAsync(ReportFilterRequest filter);

        // Staff/Manager assigns report to staff
        Task<UserReportDTO?> AssignReportAsync(Guid reportId, AssignReportRequest request, string? assignerName = null);

        // Staff/Manager updates report status/priority
        Task<UserReportDTO?> UpdateReportAsync(Guid reportId, UpdateReportRequest request);

        // Staff resolves report
        Task<UserReportDTO?> ResolveReportAsync(Guid reportId, ResolveReportRequest request);

        // Staff/Manager closes report
        Task<UserReportDTO?> CloseReportAsync(Guid reportId);

        // Staff/Manager rejects report
        Task<UserReportDTO?> RejectReportAsync(Guid reportId, string reason);

        // Manager deletes report
        Task<bool> DeleteReportAsync(Guid reportId);

        // Get statistics
        Task<ReportStatistics> GetStatisticsAsync(DateTime? fromDate = null, DateTime? toDate = null);

        // Get reports assigned to staff
        Task<List<UserReportDTO>> GetMyAssignedReportsAsync(string staffId);
    }
}
