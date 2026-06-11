
using AutoMapper;
using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.Entities;

namespace EV_StationRentalSystem.Core.Mappers
{
    public class WorkforceMappingProfile : Profile
    {
        public WorkforceMappingProfile()
        {
            // Shift mappings
            CreateMap<Shift, ShiftDTO>();
            CreateMap<CreateShiftRequest, Shift>();

            // Workday mappings - KHÔNG map Workday trong Assignment để tránh circular reference
            CreateMap<Workday, WorkdayDTO>()
                .ForMember(dest => dest.Assignments, opt => opt.MapFrom(src => src.StaffAssignments))
                .ForMember(dest => dest.StaffInfo, opt => opt.Ignore());
            CreateMap<CreateWorkdayRequest, Workday>();

            // StaffAssignment mappings - CHỈ map Shift, KHÔNG map Workday
            CreateMap<StaffAssignment, StaffAssignmentDTO>()
                .ForMember(dest => dest.Shift, opt => opt.MapFrom(src => src.Shift));
            CreateMap<CreateAssignmentRequest, StaffAssignment>();
        }
    }
}
