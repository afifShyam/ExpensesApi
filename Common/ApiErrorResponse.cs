using System.Text.Json.Serialization;

namespace ExpenseApi.Common;

public class ApiErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public ApiError Error { get; set; } = default!;
}

public class ApiError
{
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("trace_id")]
    public string TraceId { get; set; } = string.Empty;

    public object? Details { get; set; }
}
