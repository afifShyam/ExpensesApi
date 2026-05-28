using ExpenseApi.Common;
using ExpenseApi.Application.DTOs.Commitment;
using ExpenseApi.Domain.Entities;

namespace ExpenseApi.Application.Services.Commitments;

public interface ICommitmentService
{
    Task<Result<IEnumerable<CommitmentResponseDto>>> GetAllAsync();

    Task<Result<CommitmentResponseDto>> GetByIdAsync(int id);

    Task<Result<CommitmentResponseDto>> CreateAsync(CreateCommitmentDto dto);

    Task<Result<CommitmentResponseDto>> UpdateAsync(int id, UpdateCommitmentDto dto);

    Task<Result<bool>> DeleteAsync(int id);

    Task<Result<bool>> DeleteAllAsync();
}