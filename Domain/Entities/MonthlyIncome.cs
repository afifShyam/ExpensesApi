namespace ExpenseApi.Domain.Entities;

public class MonthlyIncome
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}