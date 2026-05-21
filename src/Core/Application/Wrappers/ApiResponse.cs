namespace Application.Wrappers;

/// <summary>Standardized API response wrapper for all endpoints.</summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    /// <summary>Creates a successful response with data and an optional message.</summary>
    public static ApiResponse<T> Ok(T data, string message = "Success") =>
        new() { Success = true, Message = message, Data = data };

    /// <summary>Creates a failed response with an error message and optional error list.</summary>
    public static ApiResponse<T> Fail(string message, List<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors };
}
