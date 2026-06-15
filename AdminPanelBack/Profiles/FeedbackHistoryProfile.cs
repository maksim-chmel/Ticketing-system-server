using AdminPanelBack.DTO;
using AdminPanelBack.Models;
using AutoMapper;

namespace AdminPanelBack.Profiles;

public class FeedbackHistoryProfile : Profile
{
    public FeedbackHistoryProfile()
    {
        CreateMap<FeedbackHistory, FeedbackHistoryDto>();
    }
}
