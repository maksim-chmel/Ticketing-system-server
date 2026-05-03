using AdminPanelBack.DTO;
using AdminPanelBack.Models.Statistics;
using AutoMapper;

namespace AdminPanelBack.Profiles;

public class StatisticProfile:Profile
{
    public StatisticProfile()
    {
        CreateMap<StatusDistributionItem, StatusDistributionDto>()
            .ForMember(dest => dest.Name, opt
                => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Value, opt 
                => opt.MapFrom(src => src.Count));
        
        CreateMap<RequestsOverTimeItem, TimeDisrtibutionDto>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Count));
    }
}