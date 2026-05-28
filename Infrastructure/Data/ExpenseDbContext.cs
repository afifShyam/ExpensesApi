using ExpenseApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseApi.Infrastructure.Data;

public class ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : DbContext(options)
{
    public DbSet<Expense> Expenses => Set<Expense>();

    public DbSet<MonthlyIncome> MonthlyIncomes => Set<MonthlyIncome>();

    public DbSet<Commitment> Commitments => Set<Commitment>();

    public DbSet<CommitmentPayment> CommitmentPayments => Set<CommitmentPayment>();

    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(e => e.Date)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Expenses)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(c => c.Type)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(c => c.CreatedAt)
                .IsRequired();

            entity.Property(c => c.UpdatedAt);
        });

        modelBuilder.Entity<MonthlyIncome>(entity =>
        {
            entity.HasKey(m => m.Id);

            entity.Property(m => m.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(m => m.Month)
                .IsRequired();

            entity.Property(m => m.Year)
                .IsRequired();

            entity.Property(m => m.CreatedAt)
                .IsRequired();

            entity.Property(m => m.UpdatedAt);
        });

        modelBuilder.Entity<Commitment>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(c => c.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(c => c.DueDay)
                .IsRequired();

            entity.Property(c => c.IsActive)
                .IsRequired();

            entity.Property(c => c.StartDate)
                .IsRequired();

            entity.Property(c => c.EndDate);

            entity.Property(c => c.CreatedAt)
                .IsRequired();

            entity.Property(c => c.UpdatedAt);
        });

        modelBuilder.Entity<CommitmentPayment>(entity =>
        {
            entity.HasKey(cp => cp.Id);

            entity.Property(cp => cp.Month)
                .IsRequired();

            entity.Property(cp => cp.Year)
                .IsRequired();

            entity.Property(cp => cp.AmountPaid)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(cp => cp.PaidAt)
                .IsRequired();

            entity.Property(cp => cp.Note)
                .HasMaxLength(500);

            entity.HasOne(cp => cp.Commitment)
                .WithMany(c => c.Payments)
                .HasForeignKey(cp => cp.CommitmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}