using EV_StationRentalSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.RepositoryContracts
{
    public interface IVehicleRepository : IGenericRepository<Vehicle>
    {
        Task<IEnumerable<Vehicle>> GetVehiclesByTypeAsync(Guid typeId);
        Task<IEnumerable<Vehicle>> GetVehiclesByStatusAsync(string status);
        Task<Vehicle?> GetVehicleWithDetailsAsync(Guid vehicleId);
    }

    public interface ITypeVehicleRepository : IGenericRepository<TypeVehicle>
    {
        Task<TypeVehicle?> GetTypeWithVehiclesAsync(Guid typeId);
    }

    public interface IBranchDestinationRepository : IGenericRepository<BranchDestination>
    {
        Task<BranchDestination?> GetBranchWithDetailsAsync(Guid branchId);
    }

    public interface IMaintenanceRecordRepository : IGenericRepository<MaintenanceRecord>
    {
        Task<IEnumerable<MaintenanceRecord>> GetMaintenanceByVehicleAsync(Guid vehicleId);
        Task<IEnumerable<MaintenanceRecord>> GetUpcomingMaintenanceAsync();
    }

    public interface IVehicleRelocationRepository : IGenericRepository<VehicleRelocation>
    {
        Task<IEnumerable<VehicleRelocation>> GetRelocationsByVehicleAsync(Guid vehicleId);
        Task<IEnumerable<VehicleRelocation>> GetRelocationsByStatusAsync(string status);
        Task<VehicleRelocation?> GetRelocationWithDetailsAsync(Guid relocationId);
    }

    public interface IVehicleAvailabilityRepository : IGenericRepository<VehicleAvailabilityByBranch>
    {
        Task<VehicleAvailabilityByBranch?> GetByBranchAndDateAsync(Guid branchId, DateTime date);
        Task UpdateAvailabilityAsync(Guid branchId);
    }
}

