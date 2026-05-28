using ExpenseApi.Application.Interfaces;
using ExpenseApi.Infrastructure.Data;
using ExpenseApi.Infrastructure.Repositories.Commitments;
using ExpenseApi.Infrastructure.Repositories.Expenses;
using Microsoft.EntityFrameworkCore;

namespace ExpenseApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ExpenseDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<ICommitmentRepository, CommitmentRepository>();

        return services;
    }
}