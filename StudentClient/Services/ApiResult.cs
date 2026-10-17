namespace StudentClient.Services;

public class ApiResult<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string Message { get; init; } = string.Empty;
    public int StatusCode { get; init; }

    public static ApiResult<T> Ok(T? data, string message, int statusCode)
    {
        return new ApiResult<T>
        {
            Success = true,
            Data = data,
            Message = message,
            StatusCode = statusCode
        };
    }

    public static ApiResult<T> Fail(string message, int statusCode = 0)
    {
        return new ApiResult<T>
        {
            Success = false,
            Message = message,
            StatusCode = statusCode
        };
    }
}
