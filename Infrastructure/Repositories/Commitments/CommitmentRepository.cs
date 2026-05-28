using ExpenseApi.Infrastructure.Data;
using ExpenseApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ExpenseApi.Application.Interfaces;

namespace ExpenseApi.Infrastructure.Repositories.Commitments;

public class CommitmentRepository(ExpenseDbContext context) : ICommitmentRepository
{
    private readonly ExpenseDbContext _context = context;

    public Task<List<Commitment>> GetAllAsync()
    {
        return _context.Commitments
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public Task<Commitment?> GetByIdAsync(int id)
    {
        return _context.Commitments
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public Task AddAsync(Commitment commitment)
    {
        _context.Commitments.Add(commitment);
        return _context.SaveChangesAsync();
    }

    public Task UpdateAsync(Commitment commitment)
    {
        _context.Commitments.Update(commitment);
        return _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Commitment commitment)
    {
        _context.Commitments.Remove(commitment);
        return _context.SaveChangesAsync();
    }

    public async Task<bool> BulkDeleteAsync()
    {
        var deletedCount = await _context.Commitments.ExecuteDeleteAsync();

        await _context.SaveChangesAsync();

        return deletedCount > 0;
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _context.Commitments.AnyAsync(c => c.Id == id);
    }
}