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
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(FleetDbContext context) : base(context) { }

        public async Task<IEnumerable<Vehicle>> GetVehiclesByTypeAsync(Guid typeId)
        {
            return await _dbSet
                .Where(v => v.TypeId == typeId)
                .Include(v => v.TypeVehicle)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesByStatusAsync(string status)
        {
            return await _dbSet
                .Where(v => v.Status == status)
                .Include(v => v.TypeVehicle)
                .ToListAsync();
        }

        public async Task<Vehicle?> GetVehicleWithDetailsAsync(Guid vehicleId)
        {
            return await _dbSet
                .Include(v => v.TypeVehicle)
                .Include(v => v.MaintenanceRecords)
                .Include(v => v.VehicleRelocations)
                .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);
        }
    }

    public class TypeVehicleRepository : GenericRepository<TypeVehicle>, ITypeVehicleRepository
    {
        public TypeVehicleRepository(FleetDbContext context) : base(context) { }

        public async Task<TypeVehicle?> GetTypeWithVehiclesAsync(Guid typeId)
        {
            return await _dbSet
                .Include(t => t.Vehicles)
                .FirstOrDefaultAsync(t => t.TypeId == typeId);
        }
    }

    public class BranchDestinationRepository : GenericRepository<BranchDestination>, IBranchDestinationRepository
    {
        public BranchDestinationRepository(FleetDbContext context) : base(context) { }

        public async Task<BranchDestination?> GetBranchWithDetailsAsync(Guid branchId)
        {
            return await _dbSet
                .Include(b => b.VehicleAvailabilities)
                .FirstOrDefaultAsync(b => b.BranchId == branchId);
        }
    }

    public class MaintenanceRecordRepository : GenericRepository<MaintenanceRecord>, IMaintenanceRecordRepository
    {
        public MaintenanceRecordRepository(FleetDbContext context) : base(context) { }

        public async Task<IEnumerable<MaintenanceRecord>> GetMaintenanceByVehicleAsync(Guid vehicleId)
        {
            return await _dbSet
                .Where(m => m.VehicleId == vehicleId)
                .Include(m => m.Vehicle)
                .OrderByDescending(m => m.MaintenanceDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MaintenanceRecord>> GetUpcomingMaintenanceAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _dbSet
                .Where(m => m.NextMaintenanceDate.HasValue && m.NextMaintenanceDate.Value >= today)
                .Include(m => m.Vehicle)
                .OrderBy(m => m.NextMaintenanceDate)
                .ToListAsync();
        }
    }

    public class VehicleRelocationRepository : GenericRepository<VehicleRelocation>, IVehicleRelocationRepository
    {
        public VehicleRelocationRepository(FleetDbContext context) : base(context) { }

        public async Task<IEnumerable<VehicleRelocation>> GetRelocationsByVehicleAsync(Guid vehicleId)
        {
            return await _dbSet
                .Where(r => r.VehicleId == vehicleId)
                .Include(r => r.Vehicle)
                .Include(r => r.ToBranch)
                .OrderByDescending(r => r.RelocationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<VehicleRelocation>> GetRelocationsByStatusAsync(string status)
        {
            return await _dbSet
                .Where(r => r.Status == status)
                .Include(r => r.Vehicle)
                .Include(r => r.ToBranch)
                .ToListAsync();
        }

        public async Task<VehicleRelocation?> GetRelocationWithDetailsAsync(Guid relocationId)
        {
            return await _dbSet
                .Include(r => r.Vehicle)
                .Include(r => r.ToBranch)
                .FirstOrDefaultAsync(r => r.RelocationId == relocationId);
        }
    }

    public class VehicleAvailabilityRepository : GenericRepository<VehicleAvailabilityByBranch>, IVehicleAvailabilityRepository
    {
        public VehicleAvailabilityRepository(FleetDbContext context) : base(context) { }

        public async Task<VehicleAvailabilityByBranch?> GetByBranchAndDateAsync(Guid branchId, DateTime date)
        {
            return await _dbSet
                .FirstOrDefaultAsync(a => a.BranchId == branchId && a.Date.Date == date.Date);
        }

        public async Task UpdateAvailabilityAsync(Guid branchId)
        {
            // This would require access to Vehicle table to count statuses
            // Will be implemented in the service layer instead
            await Task.CompletedTask;
        }
    }
}

