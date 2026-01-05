using System.Collections.Concurrent;
using System.Net;

namespace API.Middleware;

/// <summary>
/// Simple rate limiting middleware for demonstration purposes
/// For production, consider using AspNetCoreRateLimit package or built-in .NET 7+ rate limiting
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private static readonly ConcurrentDictionary<string, RequestCounter> _requests = new();
    private readonly int _permitLimit;
    private readonly int _windowSeconds;

    public RateLimitingMiddleware(
        RequestDelegate next,
        ILogger<RateLimitingMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _permitLimit = configuration.GetValue<int>("ApiSettings:RateLimiting:PermitLimit", 100);
        _windowSeconds = configuration.GetValue<int>("ApiSettings:RateLimiting:Window", 60);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        
        // Skip rate limiting for health checks
        if (endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Routing.RouteEndpoint>()?.RoutePattern.RawText == "/health")
        {
            await _next(context);
            return;
        }

        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var key = $"{ipAddress}_{context.Request.Path}";

        var counter = _requests.GetOrAdd(key, _ => new RequestCounter());

        bool rateLimitExceeded = false;
        
        lock (counter)
        {
            var now = DateTime.UtcNow;
            
            // Reset counter if window has passed
            if ((now - counter.WindowStart).TotalSeconds > _windowSeconds)
            {
                counter.RequestCount = 0;
                counter.WindowStart = now;
            }

            counter.RequestCount++;

            if (counter.RequestCount > _permitLimit)
            {
                rateLimitExceeded = true;
            }
        }

        if (rateLimitExceeded)
        {
            _logger.LogWarning("Rate limit exceeded for {IpAddress} on {Path}", ipAddress, context.Request.Path);
            
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            context.Response.ContentType = "application/json";
            
            var response = new ApiResponse(
                $"Rate limit exceeded. Maximum {_permitLimit} requests per {_windowSeconds} seconds.",
                false,
                null,
                (int)HttpStatusCode.TooManyRequests
            );
            
            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        await _next(context);
    }

    private class RequestCounter
    {
        public int RequestCount { get; set; }
        public DateTime WindowStart { get; set; } = DateTime.UtcNow;
    }
}
