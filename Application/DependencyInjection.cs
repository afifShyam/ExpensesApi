using ExpenseApi.Application.Interfaces;
using ExpenseApi.Application.Services.Commitments;
using ExpenseApi.Application.Services.Expenses;

namespace ExpenseApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<ICommitmentService, CommitmentService>();

        return services;
    }
}