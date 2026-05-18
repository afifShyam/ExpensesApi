using System.ComponentModel.DataAnnotations;

namespace ExpenseApi.DTOs;

public class UpdateExpenseDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    public DateTime Date { get; set; }

    public int CategoryId { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }
}