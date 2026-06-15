using Microsoft.AspNetCore.SignalR;

namespace AdminPanelBack.Hubs;

public class FeedbackHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var apiKey = Context.GetHttpContext()?.Request.Query["access_token"].ToString();
        var configuredKey = Context.GetHttpContext()?.RequestServices
            .GetRequiredService<IConfiguration>()["API_KEY"];

        if (string.IsNullOrWhiteSpace(apiKey) || apiKey != configuredKey)
        {
            Context.Abort();
            return;
        }

        await base.OnConnectedAsync();
    }
}
