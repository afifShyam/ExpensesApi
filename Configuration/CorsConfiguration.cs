using ExpenseApi.Common;

namespace ExpenseApi.Configuration;

public static class CorsConfiguration
{
    public static IServiceCollection AddApiCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(Policies.DevelopmentCorsPolicy, policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }
}