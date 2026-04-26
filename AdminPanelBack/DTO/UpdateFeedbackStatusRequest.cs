using AdminPanelBack.Models;

namespace AdminPanelBack.DTO;

public sealed class UpdateFeedbackStatusRequest
{
    public FeedbackStatus Status { get; set; }
}
