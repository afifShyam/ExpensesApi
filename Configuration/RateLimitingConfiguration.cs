using System.Security.Claims;
using System.Threading.RateLimiting;
using ExpenseApi.Common;

namespace ExpenseApi.Configuration;

public static class RateLimitingConfiguration
{
    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (context, cancellationToken) =>
            {
                var httpContext = context.HttpContext;

                var response = new ApiErrorResponse
                {
                    Message = "Too many requests.",
                    Error = new ApiError
                    {
                        Code = "RATE_LIMIT_EXCEEDED",
                        TraceId = httpContext.TraceIdentifier
                    }
                };

                httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                await httpContext.Response.WriteAsJsonAsync(
                    response,
                    cancellationToken);
            };

            options.AddPolicy(Policies.ApiRateLimitPolicy, context =>
            {
                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var partitionKey = userId
                    ?? context.Connection.RemoteIpAddress?.ToString()
                    ?? "anonymous";

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    });
            });
        });

        return services;
    }
}