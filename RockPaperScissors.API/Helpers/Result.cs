namespace api.Helpers;

public class Result<T>
{
    private Result(bool isSuccess, int? statusCode, string? errorMessage, T? data)
    {
        ErrorMessage = errorMessage;
        Data = data;
        IsSuccess = isSuccess;
        StatusCode = statusCode;
    }

    public bool IsSuccess { get; }
    public int? StatusCode { get; }
    public string? ErrorMessage { get; }
    public T? Data { get; }

    public static Result<T> Success(T data) => new(true, null, null, data);

    public static Result<T> Failure(int statusCode, string errorMessage) => new(false, statusCode, errorMessage, default);
}
