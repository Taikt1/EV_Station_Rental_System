using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.RepositoryContracts
{
    public interface IUserReportRepository
    {
        Task<UserReport> CreateAsync(UserReport report);
        Task<UserReport?> GetByIdAsync(Guid reportId);
        Task<UserReport> UpdateAsync(UserReport report);
        Task<bool> DeleteAsync(Guid reportId);
        Task<List<UserReport>> GetByFilterAsync(ReportFilterRequest filter);
        Task<int> GetCountByFilterAsync(ReportFilterRequest filter);
        Task<List<UserReport>> GetByReporterIdAsync(string reporterId);
        Task<List<UserReport>> GetByAssignedStaffIdAsync(string staffId);
        Task<List<UserReport>> GetByStatusAsync(string status);
        Task<ReportStatistics> GetStatisticsAsync(DateTime? fromDate = null, DateTime? toDate = null);
    }
}
