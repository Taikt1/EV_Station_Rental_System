using AutoMapper;
using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.Entities;
using EV_StationRentalSystem.Core.RepositoryContracts;
using EV_StationRentalSystem.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Services
{
    public class TypeVehicleService : ITypeVehicleService
    {
        private readonly ITypeVehicleRepository _typeVehicleRepo;
        private readonly IMapper _mapper;

        public TypeVehicleService(ITypeVehicleRepository typeVehicleRepo, IMapper mapper)
        {
            _typeVehicleRepo = typeVehicleRepo;
            _mapper = mapper;
        }

        public async Task<TypeVehicleResponse> CreateTypeVehicleAsync(TypeVehicleCreateRequest request)
        {
            var typeVehicle = _mapper.Map<TypeVehicle>(request);
            typeVehicle.TypeId = Guid.NewGuid();

            var created = await _typeVehicleRepo.AddAsync(typeVehicle);
            return _mapper.Map<TypeVehicleResponse>(created);
        }

        public async Task<TypeVehicleResponse?> GetTypeVehicleByIdAsync(Guid typeId)
        {
            var typeVehicle = await _typeVehicleRepo.GetTypeWithVehiclesAsync(typeId);
            return typeVehicle == null ? null : _mapper.Map<TypeVehicleResponse>(typeVehicle);
        }

        public async Task<IEnumerable<TypeVehicleResponse>> GetAllTypeVehiclesAsync()
        {
            var types = await _typeVehicleRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<TypeVehicleResponse>>(types);
        }

        public async Task<TypeVehicleResponse> UpdateTypeVehicleAsync(Guid typeId, TypeVehicleUpdateRequest request)
        {
            var typeVehicle = await _typeVehicleRepo.GetByIdAsync(typeId);
            if (typeVehicle == null) throw new KeyNotFoundException("Type Vehicle not found");

            if (request.TypeName != null) typeVehicle.TypeName = request.TypeName;
            if (request.Brand != null) typeVehicle.Brand = request.Brand;
            if (request.Model != null) typeVehicle.Model = request.Model;
            if (request.DefaultBattery.HasValue) typeVehicle.DefaultBattery = request.DefaultBattery.Value;
            if (request.BasePrice.HasValue) typeVehicle.BasePrice = request.BasePrice.Value;
            if (request.Description != null) typeVehicle.Description = request.Description;

            await _typeVehicleRepo.UpdateAsync(typeVehicle);
            return _mapper.Map<TypeVehicleResponse>(typeVehicle);
        }

        public async Task<bool> DeleteTypeVehicleAsync(Guid typeId)
        {
            var typeVehicle = await _typeVehicleRepo.GetByIdAsync(typeId);
            if (typeVehicle == null) return false;

            await _typeVehicleRepo.DeleteAsync(typeVehicle);
            return true;
        }
    }

    public class BranchDestinationService : IBranchDestinationService
    {
        private readonly IBranchDestinationRepository _branchRepo;
        private readonly IMapper _mapper;

        public BranchDestinationService(IBranchDestinationRepository branchRepo, IMapper mapper)
        {
            _branchRepo = branchRepo;
            _mapper = mapper;
        }

        public async Task<BranchDestinationResponse> CreateBranchAsync(BranchDestinationCreateRequest request)
        {
            var branch = _mapper.Map<BranchDestination>(request);
            branch.BranchId = Guid.NewGuid();
            branch.Status = "Active";

            var created = await _branchRepo.AddAsync(branch);
            return _mapper.Map<BranchDestinationResponse>(created);
        }

        public async Task<BranchDestinationResponse?> GetBranchByIdAsync(Guid branchId)
        {
            var branch = await _branchRepo.GetBranchWithDetailsAsync(branchId);
            return branch == null ? null : _mapper.Map<BranchDestinationResponse>(branch);
        }

        public async Task<IEnumerable<BranchDestinationResponse>> GetAllBranchesAsync()
        {
            var branches = await _branchRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<BranchDestinationResponse>>(branches);
        }

        public async Task<BranchDestinationResponse> UpdateBranchAsync(Guid branchId, BranchDestinationUpdateRequest request)
        {
            var branch = await _branchRepo.GetByIdAsync(branchId);
            if (branch == null) throw new KeyNotFoundException("Branch not found");

            if (request.BranchName != null) branch.BranchName = request.BranchName;
            if (request.Address != null) branch.Address = request.Address;
            if (request.City != null) branch.City = request.City;
            if (request.Latitude.HasValue) branch.Latitude = request.Latitude.Value;
            if (request.Longitude.HasValue) branch.Longitude = request.Longitude.Value;
            if (request.ContactNumber != null) branch.ContactNumber = request.ContactNumber;
            if (request.WorkingHours != null) branch.WorkingHours = request.WorkingHours;
            if (request.Status != null) branch.Status = request.Status;

            await _branchRepo.UpdateAsync(branch);
            return _mapper.Map<BranchDestinationResponse>(branch);
        }

        public async Task<bool> DeleteBranchAsync(Guid branchId)
        {
            var branch = await _branchRepo.GetByIdAsync(branchId);
            if (branch == null) return false;

            await _branchRepo.DeleteAsync(branch);
            return true;
        }
    }

    public class MaintenanceRecordService : IMaintenanceRecordService
    {
        private readonly IMaintenanceRecordRepository _maintenanceRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IMapper _mapper;

        public MaintenanceRecordService(IMaintenanceRecordRepository maintenanceRepo, IVehicleRepository vehicleRepo, IMapper mapper)
        {
            _maintenanceRepo = maintenanceRepo;
            _vehicleRepo = vehicleRepo;
            _mapper = mapper;
        }

        public async Task<MaintenanceRecordResponse> CreateMaintenanceRecordAsync(MaintenanceRecordCreateRequest request)
        {
            var maintenance = _mapper.Map<MaintenanceRecord>(request);
            maintenance.MaintenanceId = Guid.NewGuid();

            var created = await _maintenanceRepo.AddAsync(maintenance);
            
            // Update vehicle status to Maintenance
            var vehicle = await _vehicleRepo.GetByIdAsync(request.VehicleId);
            if (vehicle != null)
            {
                vehicle.Status = "Maintenance";
                await _vehicleRepo.UpdateAsync(vehicle);
            }

            var result = await _maintenanceRepo.GetByIdAsync(created.MaintenanceId);
            return _mapper.Map<MaintenanceRecordResponse>(result);
        }

        public async Task<MaintenanceRecordResponse?> GetMaintenanceRecordByIdAsync(Guid maintenanceId)
        {
            var maintenance = await _maintenanceRepo.GetByIdAsync(maintenanceId);
            return maintenance == null ? null : _mapper.Map<MaintenanceRecordResponse>(maintenance);
        }

        public async Task<IEnumerable<MaintenanceRecordResponse>> GetMaintenanceByVehicleAsync(Guid vehicleId)
        {
            var records = await _maintenanceRepo.GetMaintenanceByVehicleAsync(vehicleId);
            return _mapper.Map<IEnumerable<MaintenanceRecordResponse>>(records);
        }

        public async Task<IEnumerable<MaintenanceRecordResponse>> GetUpcomingMaintenanceAsync()
        {
            var records = await _maintenanceRepo.GetUpcomingMaintenanceAsync();
            return _mapper.Map<IEnumerable<MaintenanceRecordResponse>>(records);
        }

        public async Task<MaintenanceRecordResponse> UpdateMaintenanceRecordAsync(Guid maintenanceId, MaintenanceRecordUpdateRequest request)
        {
            var maintenance = await _maintenanceRepo.GetByIdAsync(maintenanceId);
            if (maintenance == null) throw new KeyNotFoundException("Maintenance record not found");

            if (request.MaintenanceDate.HasValue) maintenance.MaintenanceDate = request.MaintenanceDate.Value;
            if (request.Description != null) maintenance.Description = request.Description;
            if (request.Cost.HasValue) maintenance.Cost = request.Cost.Value;
            if (request.PerformedBy.HasValue) maintenance.PerformedBy = request.PerformedBy.Value;
            if (request.NextMaintenanceDate.HasValue) maintenance.NextMaintenanceDate = request.NextMaintenanceDate;

            await _maintenanceRepo.UpdateAsync(maintenance);
            return _mapper.Map<MaintenanceRecordResponse>(maintenance);
        }

        public async Task<bool> DeleteMaintenanceRecordAsync(Guid maintenanceId)
        {
            var maintenance = await _maintenanceRepo.GetByIdAsync(maintenanceId);
            if (maintenance == null) return false;

            await _maintenanceRepo.DeleteAsync(maintenance);
            return true;
        }
    }

    public class VehicleRelocationService : IVehicleRelocationService
    {
        private readonly IVehicleRelocationRepository _relocationRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IMapper _mapper;

        public VehicleRelocationService(IVehicleRelocationRepository relocationRepo, IVehicleRepository vehicleRepo, IMapper mapper)
        {
            _relocationRepo = relocationRepo;
            _vehicleRepo = vehicleRepo;
            _mapper = mapper;
        }

        public async Task<VehicleRelocationResponse> CreateRelocationAsync(VehicleRelocationCreateRequest request)
        {
            var relocation = _mapper.Map<VehicleRelocation>(request);
            relocation.RelocationId = Guid.NewGuid();
            relocation.Status = "Planned";

            var created = await _relocationRepo.AddAsync(relocation);
            var result = await _relocationRepo.GetRelocationWithDetailsAsync(created.RelocationId);
            return _mapper.Map<VehicleRelocationResponse>(result);
        }

        public async Task<VehicleRelocationResponse?> GetRelocationByIdAsync(Guid relocationId)
        {
            var relocation = await _relocationRepo.GetRelocationWithDetailsAsync(relocationId);
            return relocation == null ? null : _mapper.Map<VehicleRelocationResponse>(relocation);
        }

        public async Task<IEnumerable<VehicleRelocationResponse>> GetRelocationsByVehicleAsync(Guid vehicleId)
        {
            var relocations = await _relocationRepo.GetRelocationsByVehicleAsync(vehicleId);
            return _mapper.Map<IEnumerable<VehicleRelocationResponse>>(relocations);
        }

        public async Task<IEnumerable<VehicleRelocationResponse>> GetRelocationsByStatusAsync(string status)
        {
            var relocations = await _relocationRepo.GetRelocationsByStatusAsync(status);
            return _mapper.Map<IEnumerable<VehicleRelocationResponse>>(relocations);
        }

        public async Task<VehicleRelocationResponse> UpdateRelocationAsync(Guid relocationId, VehicleRelocationUpdateRequest request)
        {
            var relocation = await _relocationRepo.GetByIdAsync(relocationId);
            if (relocation == null) throw new KeyNotFoundException("Relocation not found");

            if (request.ArrivalTime.HasValue) relocation.ArrivalTime = request.ArrivalTime;
            if (request.Status != null) relocation.Status = request.Status;
            if (request.Reason != null) relocation.Reason = request.Reason;

            await _relocationRepo.UpdateAsync(relocation);
            var result = await _relocationRepo.GetRelocationWithDetailsAsync(relocationId);
            return _mapper.Map<VehicleRelocationResponse>(result);
        }

        public async Task<bool> DeleteRelocationAsync(Guid relocationId)
        {
            var relocation = await _relocationRepo.GetByIdAsync(relocationId);
            if (relocation == null) return false;

            await _relocationRepo.DeleteAsync(relocation);
            return true;
        }

        public async Task<VehicleRelocationResponse> CompleteRelocationAsync(Guid relocationId)
        {
            var relocation = await _relocationRepo.GetByIdAsync(relocationId);
            if (relocation == null) throw new KeyNotFoundException("Relocation not found");

            relocation.Status = "Completed";
            relocation.ArrivalTime = DateTime.UtcNow;

            await _relocationRepo.UpdateAsync(relocation);
            
            var result = await _relocationRepo.GetRelocationWithDetailsAsync(relocationId);
            return _mapper.Map<VehicleRelocationResponse>(result);
        }
    }
}

