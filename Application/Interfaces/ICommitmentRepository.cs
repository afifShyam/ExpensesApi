using ExpenseApi.Domain.Entities;

namespace ExpenseApi.Application.Interfaces;

public interface ICommitmentRepository
{
    Task<List<Commitment>> GetAllAsync();
    Task<Commitment?> GetByIdAsync(int id);
    Task AddAsync(Commitment commitment);
    Task UpdateAsync(Commitment commitment);
    Task DeleteAsync(Commitment commitment);
    Task<bool> BulkDeleteAsync();
    Task<bool> ExistsAsync(int id);
}