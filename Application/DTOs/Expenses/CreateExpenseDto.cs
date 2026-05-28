using System.ComponentModel.DataAnnotations;

namespace ExpenseApi.Application.DTOs.Expenses;

public class CreateExpenseDto
{

    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public required string Title { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    public DateTime Date { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Category ID must be greater than 0")]
    public required int CategoryId { get; set; }

    [MaxLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
    public string? Note { get; set; }
}