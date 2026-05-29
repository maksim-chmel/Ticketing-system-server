namespace AdminPanelBack.Middleware;
using Microsoft.AspNetCore.OutputCaching;

public class AuthorizedOutputCachePolicy : IOutputCachePolicy
{
    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        var request = context.HttpContext.Request;
        
        var isGetOrHead = HttpMethods.IsGet(request.Method) || HttpMethods.IsHead(request.Method);
        
        context.EnableOutputCaching = true;
        context.AllowCacheLookup = isGetOrHead;
        context.AllowCacheStorage = isGetOrHead;
        context.AllowLocking = true;

        return ValueTask.CompletedTask;
    }

    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellationToken) 
        => ValueTask.CompletedTask;

    public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        
        if (context.HttpContext.Response.StatusCode != StatusCodes.Status200OK)
        {
            context.AllowCacheStorage = false;
        }
        return ValueTask.CompletedTask;
    }
}