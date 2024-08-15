using AutoMapper;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs.Business;
using Celebratix.Common.Models.DTOs.Business.Channel;
using Celebratix.Common.Models.DTOs.Business.Events;

namespace Celebratix.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EventTicketCreateRequest, EventTicketType>()
                .ForMember(dest => dest.MaxTicketsAvailable, opt => opt.MapFrom(src => src.Capacity))
                .ForMember(a => a.Image,
                    opt => opt.Ignore());
            CreateMap<BusinessTrackerCreateRequest, Tracking>();
            CreateMap<TransactionCreateBusinessRequest, Transaction>();
            CreateMap<ChannelRequest, Channel>()
                .ForMember(a => a.CustomBackground,
                    opt => opt.Ignore());
        }
    }
}