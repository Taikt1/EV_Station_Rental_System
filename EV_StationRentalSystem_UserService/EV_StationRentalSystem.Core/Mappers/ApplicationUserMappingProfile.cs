

using AutoMapper;
using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.Entities;

namespace EV_StationRentalSystem.Core.Mappers
{
    public class ApplicationUserMappingProfile : Profile
    {

        public ApplicationUserMappingProfile()
        {

            CreateMap<ApplicationUser, AuthenticationResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Token, opt => opt.Ignore())
                .ForMember(dest => dest.Success, opt => opt.Ignore());


        }
    }
}
