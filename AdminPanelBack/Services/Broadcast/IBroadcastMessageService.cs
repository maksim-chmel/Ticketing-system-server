namespace AdminPanelBack.Services;

public interface IBroadcastMessageService
{
    public Task CreateBroadcastMessage(string message);
}