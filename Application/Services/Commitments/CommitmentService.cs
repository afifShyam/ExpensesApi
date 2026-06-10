using ExpenseApi.Application.DTOs.Commitment;
using ExpenseApi.Application.Interfaces;
using ExpenseApi.Common;
using ExpenseApi.Domain.Entities;
using ExpenseApi.Common.Mapping;
using AutoMapper;

namespace ExpenseApi.Application.Services.Commitments;

public sealed class CommitmentService(
    ICommitmentRepository commitmentRepository,
    ILogger<CommitmentService> logger,
    IMapper mapper
) : ICommitmentService
{

    public async Task<Result<IEnumerable<CommitmentResponseDto>>> GetAllAsync()
    {
        var commitments = await commitmentRepository.GetAllAsync();

        var response = commitments.Select(
            mapper.Map<CommitmentResponseDto>
        );

        return Result<IEnumerable<CommitmentResponseDto>>.Success(response);
    }

    public async Task<Result<CommitmentResponseDto>> GetByIdAsync(int id)
    {
        var commitment = await commitmentRepository.GetByIdAsync(id);

        if (commitment is null)
        {
            logger.LogWarning("Commitment not found with id: {CommitmentId}", id);
            return Result<CommitmentResponseDto>.Failure(
                Error.NotFound("Commitment.NotFound", "Commitment not found.")
            );
        }

        return Result<CommitmentResponseDto>.Success(mapper.Map<CommitmentResponseDto>(commitment));
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
            logger.LogWarning("Validation failed for commitment creation: {ValidationError}", validationError.Code);
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

        await commitmentRepository.AddAsync(commitment);

        return Result<CommitmentResponseDto>.Success(mapper.Map<CommitmentResponseDto>(commitment));
    }

    public async Task<Result<CommitmentResponseDto>> UpdateAsync(int id, UpdateCommitmentDto dto)
    {
        var commitment = await commitmentRepository.GetByIdAsync(id);

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

        await commitmentRepository.UpdateAsync(commitment);

        return Result<CommitmentResponseDto>.Success(mapper.Map<CommitmentResponseDto>(commitment));
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var commitment = await commitmentRepository.GetByIdAsync(id);

        if (commitment is null)
        {
            return Result<bool>.Failure(
                Error.NotFound("Commitment.NotFound", "Commitment not found.")
            );
        }

        await commitmentRepository.DeleteAsync(commitment);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAllAsync()
    {
        var bulkDelete = await commitmentRepository.BulkDeleteAsync();

        return Result<bool>.Success(bulkDelete);
    }

    private static Error? ValidateCommitment(
        string name,
        decimal amount,
        int dueDay,
        DateTime? endDate)
    {
        return (name, amount, dueDay, endDate) switch
        {
            (_, _, _, _) when string.IsNullOrWhiteSpace(name) => Error.Validation("Commitment.InvalidName", "Name is required."),
            (_, _, _, _) when amount <= 0 => Error.Validation("Commitment.InvalidAmount", "Amount must be greater than zero."),
            (_, _, _, _) when dueDay < 1 || dueDay > 31 => Error.Validation("Commitment.InvalidDueDay", "Due day must be between 1 and 31."),
            _ => null
        };

    }

}
