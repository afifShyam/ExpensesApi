using System.ComponentModel.DataAnnotations;

namespace ExpenseApi.Application.DTOs.Commitment;

public class CreateCommitmentDto
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public required string Name { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Range(1, 31, ErrorMessage = "Due day must be between 1 and 31")]
    public int DueDay { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    public DateTime? EndDate { get; set; }
}