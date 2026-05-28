namespace ExpenseApi.Domain.Common;

public sealed class Result<T>
{
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public T? Value { get; }

    public Error? Error { get; }

    private Result(bool isSuccess, T? value, Error? error)
    {
        if (isSuccess && error is not null)
            throw new InvalidOperationException("Success result cannot have an error.");

        if (!isSuccess && error is null)
            throw new InvalidOperationException("Failure result must have an error.");

        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, null);
    }

    public static Result<T> Failure(Error error)
    {
        return new Result<T>(false, default, error);
    }

    public TResult When<TResult>(
        Func<T, TResult> success,
        Func<Error, TResult> failure)
    {
        return IsSuccess
            ? success(Value!)
            : failure(Error!);
    }

}
