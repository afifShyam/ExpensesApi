using ExpenseApi.Common;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApi.Configuration;

public static class ControllerConfiguration
{
    public static IMvcBuilder AddApiControllers(this IServiceCollection services)
    {
        return services
            .AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.RespectRequiredConstructorParameters = true)
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var details = context.ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key.ToCamelCasePath(),
                            x => x.Value!.Errors.First().ErrorMessage
                        );

                    var response = new ApiErrorResponse
                    {
                        Message = "Validation failed.",
                        Error = new ApiError
                        {
                            Code = "VALIDATION_ERROR",
                            TraceId = context.HttpContext.TraceIdentifier,
                            Details = details
                        }
                    };

                    return new BadRequestObjectResult(response);
                };
            });
    }
}