namespace ExpenseApi.DTOs;

public class ExpenseResponseDto
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string Title { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime Date { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}