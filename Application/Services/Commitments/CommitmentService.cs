using ExpenseApi.Application.DTOs.Commitment;
using ExpenseApi.Application.Interfaces;
using ExpenseApi.Common;
using ExpenseApi.Domain.Entities;

namespace ExpenseApi.Application.Services.Commitments;

public class CommitmentService(ICommitmentRepository commitmentRepository) : ICommitmentService
{
    private readonly ICommitmentRepository _commitmentRepository = commitmentRepository;

    public async Task<Result<IEnumerable<CommitmentResponseDto>>> GetAllAsync()
    {
        var commitments = await _commitmentRepository.GetAllAsync();

        var response = commitments.Select(ToResponseDto);

        return Result<IEnumerable<CommitmentResponseDto>>.Success(response);
    }

    public async Task<Result<CommitmentResponseDto>> GetByIdAsync(int id)
    {
        var commitment = await _commitmentRepository.GetByIdAsync(id);

        if (commitment is null)
        {
            return Result<CommitmentResponseDto>.Failure(
                Error.NotFound("Commitment.NotFound", "Commitment not found.")
            );
        }

        return Result<CommitmentResponseDto>.Success(ToResponseDto(commitment));
    }

    public async Task<Result<CommitmentResponseDto>> CreateAsync(CreateCommitmentDto dto)
    {
        var validationError = ValidateCommitment(
            dto.Name,
            dto.Amount,
            dto.DueDay,

            dto.EndDate
        );

        if (validationError is not null)
        {
            return Result<CommitmentResponseDto>.Failure(validationError);
        }

        var commitment = new Commitment
        {
            Name = dto.Name.Trim(),
            Amount = dto.Amount,
            StartDate = dto.StartDate,
            DueDay = dto.DueDay,
            EndDate = dto.EndDate,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
        };

        await _commitmentRepository.AddAsync(commitment);

        return Result<CommitmentResponseDto>.Success(ToResponseDto(commitment));
    }

    public async Task<Result<CommitmentResponseDto>> UpdateAsync(int id, UpdateCommitmentDto dto)
    {
        var commitment = await _commitmentRepository.GetByIdAsync(id);

        if (commitment is null)
        {
            return Result<CommitmentResponseDto>.Failure(
                Error.NotFound("Commitment.NotFound", "Commitment not found.")
            );
        }

        var validationError = ValidateCommitment(
            dto.Name,
            dto.Amount,
            dto.DueDay,
            dto.EndDate
        );

        if (validationError is not null)
        {
            return Result<CommitmentResponseDto>.Failure(validationError);
        }

        commitment.Name = dto.Name.Trim();
        commitment.Amount = dto.Amount;

        commitment.DueDay = dto.DueDay;
        commitment.IsActive = dto.IsActive;
        commitment.EndDate = dto.EndDate;
        commitment.UpdatedAt = DateTime.UtcNow;

        await _commitmentRepository.UpdateAsync(commitment);

        return Result<CommitmentResponseDto>.Success(ToResponseDto(commitment));
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var commitment = await _commitmentRepository.GetByIdAsync(id);

        if (commitment is null)
        {
            return Result<bool>.Failure(
                Error.NotFound("Commitment.NotFound", "Commitment not found.")
            );
        }

        await _commitmentRepository.DeleteAsync(commitment);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAllAsync()
    {
        var bulkDelete = await _commitmentRepository.BulkDeleteAsync();

        return Result<bool>.Success(bulkDelete);
    }

    private static Error? ValidateCommitment(
        string name,
        decimal amount,
        int dueDay,
        DateTime? endDate)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("Commitment.InvalidName", "Name is required.");
        }

        if (amount <= 0)
        {
            return Error.Validation("Commitment.InvalidAmount", "Amount must be greater than zero.");
        }

        if (dueDay < 1 || dueDay > 31)
        {
            return Error.Validation("Commitment.InvalidDueDay", "Due day must be between 1 and 31.");
        }



        return null;
    }

    private static CommitmentResponseDto ToResponseDto(Commitment commitment)
    {
        return new CommitmentResponseDto
        {
            Id = commitment.Id,
            Name = commitment.Name,
            Amount = commitment.Amount,
            DueDay = commitment.DueDay,
            IsActive = commitment.IsActive,
            StartDate = commitment.StartDate,
            EndDate = commitment.EndDate
        };
    }
}