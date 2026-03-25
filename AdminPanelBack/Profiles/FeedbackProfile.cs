using AdminPanelBack.Controllers;
using AdminPanelBack.DTO;
using AdminPanelBack.Models;
using AdminPanelBack.Services.Feedback;
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
        CreateMap<UsersMessageDto, Feedback>()
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => 0));
    }
}