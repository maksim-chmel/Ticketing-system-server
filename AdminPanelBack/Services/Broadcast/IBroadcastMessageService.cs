namespace AdminPanelBack.Services.Broadcast;

public interface IBroadcastMessageService
{
    public Task CreateBroadcastMessage(string message);
}