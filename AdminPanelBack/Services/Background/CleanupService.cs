using AdminPanelBack.DB;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AdminPanelBack.Services.Background;

public abstract class CleanupService<T>(IServiceScopeFactory scopeFactory,ILogger<CleanupService<T>> logger)
    : BackgroundService where T : class
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await dbContext.Set<T>()
                    .Where(GetPredicate())
                    .ExecuteDeleteAsync(stoppingToken);

                await Task.Delay(GetInterval(), stoppingToken);
            }
            catch (Exception e)
            {
               logger.LogError(e, "Error during cleanup"); 
            }
           
        }
    }

    public abstract Expression<Func<T, bool>> GetPredicate();
    protected virtual TimeSpan GetInterval() => TimeSpan.FromHours(24);
}
