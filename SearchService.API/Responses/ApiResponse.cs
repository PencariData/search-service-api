namespace SearchService.API.Responses;

public record ApiResponse<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public string? Error { get; init; }
    public List<string>? Errors { get; init; } // for validation multiple errors
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public static ApiResponse<T> Ok(T data) => new() { IsSuccess = true, Data = data };
    public static ApiResponse<T> Fail(string error) => new() { IsSuccess = false, Error = error };
}