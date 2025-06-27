using AdminPanelBack.DTO;
using AdminPanelBack.Models;
using AutoMapper;

namespace AdminPanelBack.Profiles;

public class FeedbackProfile : Profile
{
    public FeedbackProfile()
    {
        CreateMap<Feedback, FeedbackDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.CreatedDate));;
    }
}