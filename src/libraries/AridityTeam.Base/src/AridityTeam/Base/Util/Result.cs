namespace AridityTeam.Base.Util;

public class Result<T>(T? value, bool isSuccess, string? errorMessage)
{
    public T? Value { get; } = value;
    public bool IsSuccess { get; } = isSuccess;
    public string? ErrorMessage { get; } = errorMessage;

    public static Result<T> Success(T value)
    {
        return new Result<T>(value, true, null);
    }

    public static Result<T> Failure(string errorMessage)
    {
        return new Result<T>(default, false, errorMessage);
    }
}