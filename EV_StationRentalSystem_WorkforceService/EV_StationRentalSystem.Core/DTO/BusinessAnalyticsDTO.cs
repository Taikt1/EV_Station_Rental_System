using System;
using System.Collections.Generic;

namespace EV_StationRentalSystem.Core.DTO
{
    // Request DTOs
    public class AnalyticsFilterRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? BranchId { get; set; }
        public string? VehicleType { get; set; }
        public string? Period { get; set; } // daily, weekly, monthly, yearly
    }

    // Response DTOs for Revenue Analytics
    public class RevenueAnalyticsDTO
    {
        public decimal TotalRevenue { get; set; }
        public decimal PaidRevenue { get; set; }
        public decimal PendingRevenue { get; set; }
        public decimal RefundedRevenue { get; set; }
        public decimal AverageRevenuePerRental { get; set; }
        public int TotalRentals { get; set; }
        public int CompletedRentals { get; set; }
        public int ActiveRentals { get; set; }
        public int CancelledRentals { get; set; }
        public List<RevenueByPeriodDTO> RevenueByPeriod { get; set; } = new();
        public List<RevenueByBranchDTO> RevenueByBranch { get; set; } = new();
        public List<RevenueByVehicleTypeDTO> RevenueByVehicleType { get; set; } = new();
        public PaymentMethodBreakdownDTO PaymentMethodBreakdown { get; set; } = new();
    }

    public class RevenueByPeriodDTO
    {
        public string Period { get; set; } = string.Empty; // 2024-01, 2024-W01, 2024-01-01
        public decimal Revenue { get; set; }
        public int RentalCount { get; set; }
        public decimal AverageRevenue { get; set; }
    }

    public class RevenueByBranchDTO
    {
        public string BranchId { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int RentalCount { get; set; }
        public double Percentage { get; set; }
    }

    public class RevenueByVehicleTypeDTO
    {
        public string VehicleTypeId { get; set; } = string.Empty;
        public string VehicleTypeName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int RentalCount { get; set; }
        public double Percentage { get; set; }
    }

    public class PaymentMethodBreakdownDTO
    {
        public decimal CashAmount { get; set; }
        public int CashCount { get; set; }
        public decimal CardAmount { get; set; }
        public int CardCount { get; set; }
        public decimal EWalletAmount { get; set; }
        public int EWalletCount { get; set; }
        public decimal BankTransferAmount { get; set; }
        public int BankTransferCount { get; set; }
    }

    // Response DTOs for Vehicle Utilization Analytics
    public class VehicleUtilizationDTO
    {
        public int TotalVehicles { get; set; }
        public int AvailableVehicles { get; set; }
        public int InUseVehicles { get; set; }
        public int MaintenanceVehicles { get; set; }
        public double OverallUtilizationRate { get; set; } // Percentage
        public double AverageRentalDurationHours { get; set; }
        public int TotalRentalHours { get; set; }
        public List<VehicleUtilizationByTypeDTO> UtilizationByType { get; set; } = new();
        public List<VehicleUtilizationDetailDTO> VehicleDetails { get; set; } = new();
        public List<TopPerformingVehicleDTO> TopPerformingVehicles { get; set; } = new();
    }

    public class VehicleUtilizationByTypeDTO
    {
        public string VehicleTypeId { get; set; } = string.Empty;
        public string VehicleTypeName { get; set; } = string.Empty;
        public int TotalVehicles { get; set; }
        public int AvailableVehicles { get; set; }
        public int InUseVehicles { get; set; }
        public int MaintenanceVehicles { get; set; }
        public double UtilizationRate { get; set; }
        public int TotalRentals { get; set; }
        public int TotalRentalHours { get; set; }
    }

    public class VehicleUtilizationDetailDTO
    {
        public string VehicleId { get; set; } = string.Empty;
        public string PlateNumber { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int RentalCount { get; set; }
        public int RentalHours { get; set; }
        public double UtilizationRate { get; set; }
        public decimal TotalRevenue { get; set; }
        public DateTime? LastRentalDate { get; set; }
    }

    public class TopPerformingVehicleDTO
    {
        public string VehicleId { get; set; } = string.Empty;
        public string PlateNumber { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public int RentalCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public double UtilizationRate { get; set; }
        public double AverageRating { get; set; }
    }

    // Combined Dashboard Analytics
    public class BusinessDashboardDTO
    {
        public DateTime GeneratedAt { get; set; }
        public AnalyticsFilterRequest Filters { get; set; } = new();
        public RevenueAnalyticsDTO RevenueAnalytics { get; set; } = new();
        public VehicleUtilizationDTO VehicleUtilization { get; set; } = new();
        public CustomerAnalyticsDTO CustomerAnalytics { get; set; } = new();
        public OperationalMetricsDTO OperationalMetrics { get; set; } = new();
    }

    // Customer Analytics
    public class CustomerAnalyticsDTO
    {
        public int TotalCustomers { get; set; }
        public int ActiveCustomers { get; set; }
        public int NewCustomersInPeriod { get; set; }
        public double CustomerRetentionRate { get; set; }
        public double AverageCustomerLifetimeValue { get; set; }
        public double AverageRentalsPerCustomer { get; set; }
        public List<TopCustomerDTO> TopCustomers { get; set; } = new();
        public List<CustomerSegmentDTO> CustomerSegments { get; set; } = new();
    }

    public class TopCustomerDTO
    {
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TotalRentals { get; set; }
        public decimal TotalSpent { get; set; }
        public double AverageRating { get; set; }
        public DateTime LastRentalDate { get; set; }
    }

    public class CustomerSegmentDTO
    {
        public string Segment { get; set; } = string.Empty; // New, Regular, VIP, Inactive
        public int CustomerCount { get; set; }
        public double Percentage { get; set; }
        public decimal AverageSpending { get; set; }
    }

    // Operational Metrics
    public class OperationalMetricsDTO
    {
        public double AverageCheckInTime { get; set; } // minutes
        public double AverageCheckOutTime { get; set; } // minutes
        public int TotalPenalties { get; set; }
        public decimal TotalPenaltyAmount { get; set; }
        public int MaintenanceIssuesReported { get; set; }
        public double AverageCustomerRating { get; set; }
        public int TotalFeedbacks { get; set; }
        public int PositiveFeedbacks { get; set; }
        public int NegativeFeedbacks { get; set; }
        public List<IssueBreakdownDTO> IssueBreakdown { get; set; } = new();
    }

    public class IssueBreakdownDTO
    {
        public string IssueType { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    // DTOs for data fetched from other microservices
    public class VehicleDataDTO
    {
        public string VehicleId { get; set; } = string.Empty;
        public string PlateNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string TypeId { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
    }

    public class VehicleTypeDataDTO
    {
        public string TypeId { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
    }

    public class RentalOrderDataDTO
    {
        public string RentalId { get; set; } = string.Empty;
        public string RenterId { get; set; } = string.Empty;
        public string VehicleId { get; set; } = string.Empty;
        public string BranchStartId { get; set; } = string.Empty;
        public string BranchEndId { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal EstimatedCost { get; set; }
        public decimal? ActualCost { get; set; }
    }

    public class PaymentDataDTO
    {
        public string PaymentId { get; set; } = string.Empty;
        public string RentalId { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime PaymentTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class UserDataDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    // Comparison Analytics
    public class PeriodComparisonDTO
    {
        public string CurrentPeriod { get; set; } = string.Empty;
        public string PreviousPeriod { get; set; } = string.Empty;
        public decimal CurrentRevenue { get; set; }
        public decimal PreviousRevenue { get; set; }
        public double RevenueGrowthPercentage { get; set; }
        public int CurrentRentals { get; set; }
        public int PreviousRentals { get; set; }
        public double RentalGrowthPercentage { get; set; }
        public double CurrentUtilizationRate { get; set; }
        public double PreviousUtilizationRate { get; set; }
        public double UtilizationChange { get; set; }
    }
}
