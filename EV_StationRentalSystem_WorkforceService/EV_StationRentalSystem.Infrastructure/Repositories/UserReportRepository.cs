using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.Entities;
using EV_StationRentalSystem.Core.RepositoryContracts;
using EV_StationRentalSystem.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Infrastructure.Repositories
{
    public class UserReportRepository : IUserReportRepository
    {
        private readonly WorkforceDbContext _context;

        public UserReportRepository(WorkforceDbContext context)
        {
            _context = context;
        }

        public async Task<UserReport> CreateAsync(UserReport report)
        {
            _context.UserReports.Add(report);
            await _context.SaveChangesAsync();
            return report;
        }

        public async Task<UserReport?> GetByIdAsync(Guid reportId)
        {
            return await _context.UserReports.FindAsync(reportId);
        }

        public async Task<UserReport> UpdateAsync(UserReport report)
        {
            report.UpdatedAt = DateTime.UtcNow;
            _context.UserReports.Update(report);
            await _context.SaveChangesAsync();
            return report;
        }

        public async Task<bool> DeleteAsync(Guid reportId)
        {
            var report = await GetByIdAsync(reportId);
            if (report == null) return false;

            _context.UserReports.Remove(report);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserReport>> GetByFilterAsync(ReportFilterRequest filter)
        {
            var query = _context.UserReports.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filter.ReportType))
                query = query.Where(r => r.ReportType == filter.ReportType);

            if (!string.IsNullOrEmpty(filter.Category))
                query = query.Where(r => r.Category == filter.Category);

            if (!string.IsNullOrEmpty(filter.Status))
                query = query.Where(r => r.Status == filter.Status);

            if (!string.IsNullOrEmpty(filter.Priority))
                query = query.Where(r => r.Priority == filter.Priority);

            if (!string.IsNullOrEmpty(filter.AssignedToStaffId))
                query = query.Where(r => r.AssignedToStaffId == filter.AssignedToStaffId);

            if (!string.IsNullOrEmpty(filter.ReporterId))
                query = query.Where(r => r.ReporterId == filter.ReporterId);

            if (filter.FromDate.HasValue)
                query = query.Where(r => r.CreatedAt >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(r => r.CreatedAt <= filter.ToDate.Value);

            if (!string.IsNullOrEmpty(filter.SearchKeyword))
            {
                var keyword = filter.SearchKeyword.ToLower();
                query = query.Where(r =>
                    r.Title.ToLower().Contains(keyword) ||
                    r.Description.ToLower().Contains(keyword) ||
                    (r.ReporterEmail != null && r.ReporterEmail.ToLower().Contains(keyword)) ||
                    (r.ReporterName != null && r.ReporterName.ToLower().Contains(keyword))
                );
            }

            // Pagination
            var skip = (filter.PageNumber - 1) * filter.PageSize;
            query = query.OrderByDescending(r => r.CreatedAt)
                        .Skip(skip)
                        .Take(filter.PageSize);

            return await query.ToListAsync();
        }

        public async Task<int> GetCountByFilterAsync(ReportFilterRequest filter)
        {
            var query = _context.UserReports.AsQueryable();

            // Apply same filters as GetByFilterAsync (without pagination)
            if (!string.IsNullOrEmpty(filter.ReportType))
                query = query.Where(r => r.ReportType == filter.ReportType);

            if (!string.IsNullOrEmpty(filter.Category))
                query = query.Where(r => r.Category == filter.Category);

            if (!string.IsNullOrEmpty(filter.Status))
                query = query.Where(r => r.Status == filter.Status);

            if (!string.IsNullOrEmpty(filter.Priority))
                query = query.Where(r => r.Priority == filter.Priority);

            if (!string.IsNullOrEmpty(filter.AssignedToStaffId))
                query = query.Where(r => r.AssignedToStaffId == filter.AssignedToStaffId);

            if (!string.IsNullOrEmpty(filter.ReporterId))
                query = query.Where(r => r.ReporterId == filter.ReporterId);

            if (filter.FromDate.HasValue)
                query = query.Where(r => r.CreatedAt >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(r => r.CreatedAt <= filter.ToDate.Value);

            if (!string.IsNullOrEmpty(filter.SearchKeyword))
            {
                var keyword = filter.SearchKeyword.ToLower();
                query = query.Where(r =>
                    r.Title.ToLower().Contains(keyword) ||
                    r.Description.ToLower().Contains(keyword) ||
                    (r.ReporterEmail != null && r.ReporterEmail.ToLower().Contains(keyword)) ||
                    (r.ReporterName != null && r.ReporterName.ToLower().Contains(keyword))
                );
            }

            return await query.CountAsync();
        }

        public async Task<List<UserReport>> GetByReporterIdAsync(string reporterId)
        {
            return await _context.UserReports
                .Where(r => r.ReporterId == reporterId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<UserReport>> GetByAssignedStaffIdAsync(string staffId)
        {
            return await _context.UserReports
                .Where(r => r.AssignedToStaffId == staffId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<UserReport>> GetByStatusAsync(string status)
        {
            return await _context.UserReports
                .Where(r => r.Status == status)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<ReportStatistics> GetStatisticsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.UserReports.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(r => r.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(r => r.CreatedAt <= toDate.Value);

            var reports = await query.ToListAsync();

            var statistics = new ReportStatistics
            {
                TotalReports = reports.Count,
                PendingReports = reports.Count(r => r.Status == "pending"),
                InProgressReports = reports.Count(r => r.Status == "in_progress"),
                ResolvedReports = reports.Count(r => r.Status == "resolved"),
                ClosedReports = reports.Count(r => r.Status == "closed"),
                RejectedReports = reports.Count(r => r.Status == "rejected"),

                ReportsByType = reports.GroupBy(r => r.ReportType)
                    .ToDictionary(g => g.Key, g => g.Count()),

                ReportsByCategory = reports.GroupBy(r => r.Category)
                    .ToDictionary(g => g.Key, g => g.Count()),

                ReportsByPriority = reports.GroupBy(r => r.Priority)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            // Calculate average resolution time
            var resolvedReports = reports.Where(r => r.ResolvedAt.HasValue && r.CreatedAt != null).ToList();
            if (resolvedReports.Any())
            {
                statistics.AverageResolutionTimeHours = resolvedReports
                    .Average(r => (r.ResolvedAt!.Value - r.CreatedAt).TotalHours);
            }

            // Calculate average rating
            var ratedReports = reports.Where(r => r.UserRating.HasValue).ToList();
            if (ratedReports.Any())
            {
                statistics.AverageRating = ratedReports.Average(r => r.UserRating!.Value);
            }

            return statistics;
        }
    }
}
