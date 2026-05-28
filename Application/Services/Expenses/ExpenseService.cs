using ExpenseApi.Common;
using ExpenseApi.Application.DTOs.Expenses;
using ExpenseApi.Domain.Entities;
using ExpenseApi.Application.Interfaces;

namespace ExpenseApi.Application.Services.Expenses
{
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
                     Error.NotFound("Expense.NotFound", "Expense was not found.")
                 );
            }

            return Result<ExpenseResponseDto>.Success(ToResponseDto(expense));
        }

        public async Task<Result<ExpenseResponseDto>> CreateAsync(CreateExpenseDto dto)
        {
            if (dto.Amount <= 0)
            {
                return Result<ExpenseResponseDto>.Failure(
                    Error.BadRequest("Expense.InvalidAmount", "Amount must be greater than zero.")
                );
            }

            if (dto.CategoryId <= 0)
            {
                return Result<ExpenseResponseDto>.Failure(
                    Error.BadRequest("Expense.InvalidCategory", "CategoryId is required.")
                );
            }

            var categoryExists = await _expenseRepository.CategoryExistsAsync(dto.CategoryId);

            if (!categoryExists)
            {
                return Result<ExpenseResponseDto>.Failure(
                    Error.NotFound("Category.NotFound", "Category was not found.")
                );
            }

            var expense = new Expense
            {
                Title = dto.Title.Trim(),
                Amount = dto.Amount,
                Date = DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc),
                CategoryId = dto.CategoryId,
                Note = dto.Note?.Trim()
            };

            var createdExpense = await _expenseRepository.CreateAsync(expense);

            return Result<ExpenseResponseDto>.Success(ToResponseDto(createdExpense));
        }

        public async Task<Result<ExpenseResponseDto>> UpdateAsync(int id, UpdateExpenseDto dto)
        {
            if (dto.Amount <= 0)
            {
                return Result<ExpenseResponseDto>.Failure(
                     Error.BadRequest("Expense.InvalidAmount", "Amount must be greater than zero.")
                 );
            }

            if (dto.CategoryId <= 0)
            {
                return Result<ExpenseResponseDto>.Failure(
                    Error.BadRequest("Expense.InvalidCategory", "CategoryId is required.")
                );
            }

            var categoryExists = await _expenseRepository.CategoryExistsAsync(dto.CategoryId);

            if (!categoryExists)
            {
                return Result<ExpenseResponseDto>.Failure(
                     Error.NotFound("Category.NotFound", "Category was not found.")
                 );
            }

            var expense = new Expense
            {
                Title = dto.Title.Trim(),
                Amount = dto.Amount,
                Date = DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc),
                CategoryId = dto.CategoryId,
                Note = dto.Note?.Trim()
            };

            var updatedExpense = await _expenseRepository.UpdateAsync(id, expense);

            if (updatedExpense is null)
            {
                return Result<ExpenseResponseDto>.Failure(
                    Error.NotFound("Expense.NotFound", "Expense was not found.")
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
                    Error.NotFound("Expense.NotFound", "Expense was not found.")
                );
            }

            return Result<bool>.Success(true);
        }

        private static ExpenseResponseDto ToResponseDto(Expense expense)
        {
            return new ExpenseResponseDto
            {
                Id = expense.Id,
                CategoryId = expense.CategoryId,
                CategoryName = expense.Category?.Name,
                Title = expense.Title,
                Amount = expense.Amount,
                Date = expense.Date,
                Note = expense.Note,
                CreatedAt = expense.CreatedAt,
                UpdatedAt = expense.UpdatedAt
            };
        }
    }
}