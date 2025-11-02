


using AutoMapper;
using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.Entities;
using System.Linq;

namespace EV_StationRentalSystem.Core.Mappers
{
    public class FleetMappingProfile : Profile
    {
        public FleetMappingProfile()
        {
            // Vehicle mappings
            CreateMap<VehicleCreateRequest, Vehicle>();
            CreateMap<Vehicle, VehicleResponse>()
                .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.TypeVehicle != null ? src.TypeVehicle.TypeName : null));

            // TypeVehicle mappings
            CreateMap<TypeVehicleCreateRequest, TypeVehicle>();
            CreateMap<TypeVehicle, TypeVehicleResponse>()
                .ForMember(dest => dest.VehicleCount, opt => opt.MapFrom(src => src.Vehicles != null ? src.Vehicles.Count : 0));

            // BranchDestination mappings
            CreateMap<BranchDestinationCreateRequest, BranchDestination>();
            CreateMap<BranchDestination, BranchDestinationResponse>()
                .ForMember(dest => dest.TotalVehicles, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.AvailableVehicles, opt => opt.MapFrom(src => 0));

            // MaintenanceRecord mappings
            CreateMap<MaintenanceRecordCreateRequest, MaintenanceRecord>();
            CreateMap<MaintenanceRecord, MaintenanceRecordResponse>()
                .ForMember(dest => dest.VehiclePlateNumber, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.PlateNumber : null));

            // VehicleRelocation mappings
            CreateMap<VehicleRelocationCreateRequest, VehicleRelocation>();
            CreateMap<VehicleRelocation, VehicleRelocationResponse>()
                .ForMember(dest => dest.VehiclePlateNumber, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.PlateNumber : null))
                .ForMember(dest => dest.ToBranchName, opt => opt.MapFrom(src => src.ToBranch != null ? src.ToBranch.BranchName : null));
        }
    }
}
