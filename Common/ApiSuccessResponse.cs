namespace ExpenseApi.Common;

public class ApiSuccessResponse<T>
{
    public string Message { get; set; } = string.Empty;
    public T Data { get; set; } = default!;
}
