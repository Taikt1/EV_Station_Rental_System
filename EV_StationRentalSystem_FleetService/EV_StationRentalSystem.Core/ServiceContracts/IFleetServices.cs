using EV_StationRentalSystem.Core.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.ServiceContracts
{
    public interface IVehicleService
    {
        Task<VehicleResponse> CreateVehicleAsync(VehicleCreateRequest request);
        Task<VehicleResponse?> GetVehicleByIdAsync(Guid vehicleId);
        Task<IEnumerable<VehicleResponse>> GetAllVehiclesAsync();
        Task<IEnumerable<VehicleResponse>> GetVehiclesByTypeAsync(Guid typeId);
        Task<IEnumerable<VehicleResponse>> GetVehiclesByStatusAsync(string status);
        Task<VehicleResponse> UpdateVehicleAsync(Guid vehicleId, VehicleUpdateRequest request);
        Task<bool> DeleteVehicleAsync(Guid vehicleId);
        Task<VehicleStatusSummaryResponse> GetVehicleStatusSummaryAsync();
    }

    public interface ITypeVehicleService
    {
        Task<TypeVehicleResponse> CreateTypeVehicleAsync(TypeVehicleCreateRequest request);
        Task<TypeVehicleResponse?> GetTypeVehicleByIdAsync(Guid typeId);
        Task<IEnumerable<TypeVehicleResponse>> GetAllTypeVehiclesAsync();
        Task<TypeVehicleResponse> UpdateTypeVehicleAsync(Guid typeId, TypeVehicleUpdateRequest request);
        Task<bool> DeleteTypeVehicleAsync(Guid typeId);
    }

    public interface IBranchDestinationService
    {
        Task<BranchDestinationResponse> CreateBranchAsync(BranchDestinationCreateRequest request);
        Task<BranchDestinationResponse?> GetBranchByIdAsync(Guid branchId);
        Task<IEnumerable<BranchDestinationResponse>> GetAllBranchesAsync();
        Task<BranchDestinationResponse> UpdateBranchAsync(Guid branchId, BranchDestinationUpdateRequest request);
        Task<bool> DeleteBranchAsync(Guid branchId);
    }

    public interface IMaintenanceRecordService
    {
        Task<MaintenanceRecordResponse> CreateMaintenanceRecordAsync(MaintenanceRecordCreateRequest request);
        Task<MaintenanceRecordResponse?> GetMaintenanceRecordByIdAsync(Guid maintenanceId);
        Task<IEnumerable<MaintenanceRecordResponse>> GetMaintenanceByVehicleAsync(Guid vehicleId);
        Task<IEnumerable<MaintenanceRecordResponse>> GetUpcomingMaintenanceAsync();
        Task<MaintenanceRecordResponse> UpdateMaintenanceRecordAsync(Guid maintenanceId, MaintenanceRecordUpdateRequest request);
        Task<bool> DeleteMaintenanceRecordAsync(Guid maintenanceId);
    }

    public interface IVehicleRelocationService
    {
        Task<VehicleRelocationResponse> CreateRelocationAsync(VehicleRelocationCreateRequest request);
        Task<VehicleRelocationResponse?> GetRelocationByIdAsync(Guid relocationId);
        Task<IEnumerable<VehicleRelocationResponse>> GetRelocationsByVehicleAsync(Guid vehicleId);
        Task<IEnumerable<VehicleRelocationResponse>> GetRelocationsByStatusAsync(string status);
        Task<VehicleRelocationResponse> UpdateRelocationAsync(Guid relocationId, VehicleRelocationUpdateRequest request);
        Task<bool> DeleteRelocationAsync(Guid relocationId);
        Task<VehicleRelocationResponse> CompleteRelocationAsync(Guid relocationId);
    }
}

