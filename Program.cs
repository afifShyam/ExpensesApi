using ExpenseApi.Data;
using ExpenseApi.Middleware;
using ExpenseApi.Repositories;
using ExpenseApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var details = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    x => ToCamelCase(x.Key),
                    x => x.Value!.Errors.First().ErrorMessage
                );

            var response = new ExpenseApi.Common.ApiErrorResponse
            {
                Message = "Validation failed.",
                Error = new ExpenseApi.Common.ApiError
                {
                    Code = "VALIDATION_ERROR",
                    TraceId = context.HttpContext.TraceIdentifier,
                    Details = details
                }
            };

            return new BadRequestObjectResult(response);
        };
    });

builder.Services.AddOpenApi();

builder.Services.AddDbContext<ExpenseDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

app.Run();

static string ToCamelCase(string value)
{
    if (string.IsNullOrWhiteSpace(value))
        return value;

    return char.ToLowerInvariant(value[0]) + value[1..];
}
