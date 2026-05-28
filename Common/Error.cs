namespace ExpenseApi.Common;

public sealed class Error
{
    public string Code { get; }
    public string Message { get; }
    public string? Details { get; }
    public int StatusCode { get; }

    private Error(string code, string message, int statusCode, string? details = null)
    {
        Code = code;
        Message = message;
        StatusCode = statusCode;
        Details = details;
    }

    public static Error NotFound(string code, string message, string? details = null)
        => new(code, message, StatusCodes.Status404NotFound, details);

    public static Error BadRequest(string code, string message, string? details = null)
        => new(code, message, StatusCodes.Status400BadRequest, details);

    public static Error Unauthorized(string code, string message, string? details = null)
        => new(code, message, StatusCodes.Status401Unauthorized, details);

    public static Error Conflict(string code, string message, string? details = null)
        => new(code, message, StatusCodes.Status409Conflict, details);

    public static Error Validation(string message, string? details = null)
        => new("VALIDATION_ERROR", message, StatusCodes.Status400BadRequest, details);
}