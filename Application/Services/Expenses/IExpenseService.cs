using ExpenseApi.Common;
using ExpenseApi.Application.DTOs.Expenses;
using ExpenseApi.Domain.Entities;

namespace ExpenseApi.Application.Services.Expenses;

public interface IExpenseService
{
    Task<Result<IEnumerable<ExpenseResponseDto>>> GetAllAsync();

    Task<Result<ExpenseResponseDto>> GetByIdAsync(int id);

    Task<Result<ExpenseResponseDto>> CreateAsync(CreateExpenseDto dto);

    Task<Result<ExpenseResponseDto>> UpdateAsync(int id, UpdateExpenseDto dto);

    Task<Result<bool>> DeleteAsync(int id);
}
