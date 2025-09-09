namespace SearchService.API.Responses;

public record ApiResponse<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public string? Error { get; init; }
    public List<string>? Errors { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string? TraceId { get; init; } // for debugging correlation

    // Success
    public static ApiResponse<T> Ok(T data) => new()
    {
        IsSuccess = true,
        Data = data
    };

    // Failure (single error)
    public static ApiResponse<T> Fail(string error, string? traceId = null) => new()
    {
        IsSuccess = false,
        Error = error,
        TraceId = traceId
    };

    // Failure (multiple errors)
    public static ApiResponse<T> Fail(IEnumerable<string> errors, string error = "Validation failed", string? traceId = null) => new()
    {
        IsSuccess = false,
        Error = error,
        Errors = errors.ToList(),
        TraceId = traceId
    };

    // Failure from exception
    public static ApiResponse<T> FromException(Exception ex, string? traceId = null, string? friendlyMessage = null) => new()
    {
        IsSuccess = false,
        Error = friendlyMessage ?? ex.Message,
        Errors = [ex.GetType().Name],
        TraceId = traceId
    };
}