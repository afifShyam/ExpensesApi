using ExpenseApi.Common;
using ExpenseApi.DTOs;
using ExpenseApi.Models;
using ExpenseApi.Repositories;

namespace ExpenseApi.Services;

public class ExpenseService(IExpenseRepository expenseRepository) : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository = expenseRepository;

    public async Task<Result<IEnumerable<ExpenseResponseDto>>> GetAllAsync()
    {
        var expenses = await _expenseRepository.GetAllAsync();

        var response = expenses.Select(ToResponseDto);

        return Result<IEnumerable<ExpenseResponseDto>>.Success(response);
    }

    public async Task<Result<ExpenseResponseDto>> GetByIdAsync(int id)
    {
        var expense = await _expenseRepository.GetByIdAsync(id);

        if (expense is null)
        {
            return Result<ExpenseResponseDto>.Failure(
                new Error("Expense.NotFound", "Expense was not found.")
            );
        }

        return Result<ExpenseResponseDto>.Success(ToResponseDto(expense));
    }

    public async Task<Result<ExpenseResponseDto>> CreateAsync(CreateExpenseDto dto)
    {
        if (dto.Amount <= 0)
        {
            return Result<ExpenseResponseDto>.Failure(
                new Error("Expense.InvalidAmount", "Amount must be greater than zero.")
            );
        }

        var expense = new Expense
        {
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            Amount = dto.Amount,
            Date = DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc),
            Category = dto.Category.Trim()
        };

        var createdExpense = await _expenseRepository.CreateAsync(expense);

        return Result<ExpenseResponseDto>.Success(ToResponseDto(createdExpense));
    }

    public async Task<Result<ExpenseResponseDto>> UpdateAsync(int id, UpdateExpenseDto dto)
    {
        if (dto.Amount <= 0)
        {
            return Result<ExpenseResponseDto>.Failure(
                new Error("Expense.InvalidAmount", "Amount must be greater than zero.")
            );
        }

        var expense = new Expense
        {
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            Amount = dto.Amount,
            Date = DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc),
            Category = dto.Category.Trim()
        };

        var updatedExpense = await _expenseRepository.UpdateAsync(id, expense);

        if (updatedExpense is null)
        {
            return Result<ExpenseResponseDto>.Failure(
                new Error("Expense.NotFound", "Expense was not found.")
            );
        }

        return Result<ExpenseResponseDto>.Success(ToResponseDto(updatedExpense));
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var deleted = await _expenseRepository.DeleteAsync(id);

        if (!deleted)
        {
            return Result<bool>.Failure(
                new Error("Expense.NotFound", "Expense was not found.")
            );
        }

        return Result<bool>.Success(true);
    }

    private static ExpenseResponseDto ToResponseDto(Expense expense)
    {
        return new ExpenseResponseDto
        {
            Id = expense.Id,
            Title = expense.Title,
            Description = expense.Description,
            Amount = expense.Amount,
            Date = expense.Date,
            Category = expense.Category,
            CreatedAt = expense.CreatedAt,
            UpdatedAt = expense.UpdatedAt
        };
    }
}
