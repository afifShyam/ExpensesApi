namespace ExpenseApi.Domain.Entities;

public class CommitmentPayment
{
    public int Id { get; set; }

    public int CommitmentId { get; set; }

    public Commitment? Commitment { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public decimal AmountPaid { get; set; }

    public DateTime PaidAt { get; set; }

    public string? Note { get; set; }
}