using ExpenseApi.Application.Services.Commitments;
using ExpenseApi.Application.Services.Expenses;
using ExpenseApi.Common.Mapping;

namespace ExpenseApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>

            cfg.AddProfile<MappingProfile>()
        );

        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<ICommitmentService, CommitmentService>();

        return services;
    }
}