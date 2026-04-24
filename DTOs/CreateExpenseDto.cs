using System.ComponentModel.DataAnnotations;

namespace ExpenseApi.DTOs;

public class CreateExpenseDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;
}
