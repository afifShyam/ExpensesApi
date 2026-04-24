using ExpenseApi.Data;
using ExpenseApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseApi.Repositories;

public class ExpenseRepository(ExpenseDbContext context) : IExpenseRepository
{
    private readonly ExpenseDbContext _context = context;

    public Task<List<Expense>> GetAllAsync()
    {
        return _context.Expenses
            .AsNoTracking()
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public Task<Expense?> GetByIdAsync(int id)
    {
        return _context.Expenses
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Expense> CreateAsync(Expense expense)
    {
        expense.CreatedAt = DateTime.UtcNow;
        expense.UpdatedAt = null;

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        return expense;
    }

    public async Task<Expense?> UpdateAsync(int id, Expense expense)
    {
        var existingExpense = await _context.Expenses.FindAsync(id);

        if (existingExpense is null)
            return null;

        existingExpense.Title = expense.Title;
        existingExpense.Description = expense.Description;
        existingExpense.Amount = expense.Amount;
        existingExpense.Date = expense.Date;
        existingExpense.Category = expense.Category;
        existingExpense.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return existingExpense;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);

        if (expense is null)
            return false;

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();

        return true;
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _context.Expenses.AnyAsync(e => e.Id == id);
    }
}