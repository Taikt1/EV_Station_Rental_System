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
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IMapper _mapper;

        public VehicleService(IVehicleRepository vehicleRepo, IMapper mapper)
        {
            _vehicleRepo = vehicleRepo;
            _mapper = mapper;
        }

        public async Task<VehicleResponse> CreateVehicleAsync(VehicleCreateRequest request)
        {
            var vehicle = _mapper.Map<Vehicle>(request);
            vehicle.VehicleId = Guid.NewGuid();
            vehicle.Status = "Available";

            var created = await _vehicleRepo.AddAsync(vehicle);
            var result = await _vehicleRepo.GetVehicleWithDetailsAsync(created.VehicleId);
            return _mapper.Map<VehicleResponse>(result);
        }

        public async Task<VehicleResponse?> GetVehicleByIdAsync(Guid vehicleId)
        {
            var vehicle = await _vehicleRepo.GetVehicleWithDetailsAsync(vehicleId);
            return vehicle == null ? null : _mapper.Map<VehicleResponse>(vehicle);
        }

        public async Task<IEnumerable<VehicleResponse>> GetAllVehiclesAsync()
        {
            var vehicles = await _vehicleRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<VehicleResponse>>(vehicles);
        }

        public async Task<IEnumerable<VehicleResponse>> GetVehiclesByTypeAsync(Guid typeId)
        {
            var vehicles = await _vehicleRepo.GetVehiclesByTypeAsync(typeId);
            return _mapper.Map<IEnumerable<VehicleResponse>>(vehicles);
        }

        public async Task<IEnumerable<VehicleResponse>> GetVehiclesByStatusAsync(string status)
        {
            var vehicles = await _vehicleRepo.GetVehiclesByStatusAsync(status);
            return _mapper.Map<IEnumerable<VehicleResponse>>(vehicles);
        }

        public async Task<VehicleResponse> UpdateVehicleAsync(Guid vehicleId, VehicleUpdateRequest request)
        {
            var vehicle = await _vehicleRepo.GetByIdAsync(vehicleId);
            if (vehicle == null) throw new KeyNotFoundException("Vehicle not found");

            if (request.PlateNumber != null) vehicle.PlateNumber = request.PlateNumber;
            if (request.ChassisNumber != null) vehicle.ChassisNumber = request.ChassisNumber;
            if (request.BatteryCapacity.HasValue) vehicle.BatteryCapacity = request.BatteryCapacity.Value;
            if (request.Status != null) vehicle.Status = request.Status;
            if (request.TypeId.HasValue) vehicle.TypeId = request.TypeId.Value;
            if (request.ManufactureYear.HasValue) vehicle.ManufactureYear = request.ManufactureYear.Value;
            if (request.Color != null) vehicle.Color = request.Color;
            if (request.QRCode != null) vehicle.QRCode = request.QRCode;

            await _vehicleRepo.UpdateAsync(vehicle);
            var updated = await _vehicleRepo.GetVehicleWithDetailsAsync(vehicleId);
            return _mapper.Map<VehicleResponse>(updated);
        }

        public async Task<bool> DeleteVehicleAsync(Guid vehicleId)
        {
            var vehicle = await _vehicleRepo.GetByIdAsync(vehicleId);
            if (vehicle == null) return false;

            await _vehicleRepo.DeleteAsync(vehicle);
            return true;
        }

        public async Task<VehicleStatusSummaryResponse> GetVehicleStatusSummaryAsync()
        {
            var allVehicles = await _vehicleRepo.GetAllAsync();
            var vehicleList = allVehicles.ToList();

            return new VehicleStatusSummaryResponse
            {
                TotalVehicles = vehicleList.Count,
                AvailableCount = vehicleList.Count(v => v.Status == "Available"),
                InUseCount = vehicleList.Count(v => v.Status == "In-use"),
                MaintenanceCount = vehicleList.Count(v => v.Status == "Maintenance")
            };
        }
    }
}

