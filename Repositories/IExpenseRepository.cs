using ExpenseApi.Models;

namespace ExpenseApi.Repositories;

public interface IExpenseRepository
{
    Task<List<Expense>> GetAllAsync();
    Task<Expense?> GetByIdAsync(int id);
    Task<Expense> CreateAsync(Expense expense);
    Task<Expense?> UpdateAsync(int id, Expense expense);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);

    Task<bool> CategoryExistsAsync(int categoryId);
}
