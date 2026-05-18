namespace ExpenseApi.Common;

public sealed class Error(string code, string message, object? details = null)
{
    public string Code { get; } = code;

    public string Message { get; } = message;

    public object? Details { get; } = details;
}