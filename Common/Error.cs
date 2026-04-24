namespace ExpenseApi.Common;

public sealed record Error(string Code, string Message, object? Details = null);
