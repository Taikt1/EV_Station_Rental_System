


using AutoMapper;
using EV_StationRentalSystem.Core.DTO.Request;
using EV_StationRentalSystem.Core.DTO.Response;
using EV_StationRentalSystem.Core.Entities;

namespace EV_StationRentalSystem.Core.Mappers
{
    public class RentalPaymentMappingProfile : Profile
    {

        public RentalPaymentMappingProfile()
        {
            //---------------------------------RENTAL ORDER----------------------------------------
            CreateMap<RentalOrder, RentalOrderResponse>()
                .ForMember(dest => dest.DetailCount, opt => opt.MapFrom(src => src.RentalOrderDetails.Count));

            CreateMap<RentalOrderDetail, RentalOrderDetailInfoResponse>();

            //--------------------------------RENTAL CONTRACT--------------------------------------
            CreateMap<RentalContract, RentalContractResponse>();
            CreateMap<CreateRentalContractRequest, RentalContract>();

            //-----------------------------------CHECK IN------------------------------------------
            CreateMap<Checkin, CheckinResponse>()
                .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.PhotoProofs))
                .ReverseMap();

            CreateMap<CreateCheckinRequest, Checkin>();
            CreateMap<PhotoProof, PhotoProofResponse>();


            //----------------------------------CHECK OUT------------------------------------------
            CreateMap<Checkout, CheckoutResponse>()
            .ForMember(dest => dest.Photos,
                       opt => opt.MapFrom(src => src.PhotoProofs)).ReverseMap();
            CreateMap<CreateCheckoutRequest, Checkout>();

            CreateMap<PenaltyRecord, PenaltyResponse>();
            CreateMap<CreatePenaltyRequest, PenaltyRecord>();

        }
    }
}
