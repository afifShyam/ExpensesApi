using ExpenseApi.Common;
using ExpenseApi.DTOs;

namespace ExpenseApi.Services;

public interface IExpenseService
{
    Task<Result<IEnumerable<ExpenseResponseDto>>> GetAllAsync();

    Task<Result<ExpenseResponseDto>> GetByIdAsync(int id);

    Task<Result<ExpenseResponseDto>> CreateAsync(CreateExpenseDto dto);

    Task<Result<ExpenseResponseDto>> UpdateAsync(int id, UpdateExpenseDto dto);

    Task<Result<bool>> DeleteAsync(int id);
}
