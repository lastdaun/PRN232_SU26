namespace PRN232.LMS.Services.Models.Common;

/// <summary>Standard API response wrapper.</summary>
/// <typeparam name="T">Payload type.</typeparam>
public sealed class ApiResponse<T>
{
    /// <summary>Indicates whether the request succeeded.</summary>
    public bool Success { get; init; }

    /// <summary>Human-readable status message.</summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>Response payload when successful.</summary>
    public T? Data { get; init; }

    /// <summary>Validation or error details when unsuccessful.</summary>
    public object? Errors { get; init; }

    public static ApiResponse<T> Ok(T data, string message = "Request processed successfully")
        => new() { Success = true, Message = message, Data = data, Errors = null };

    public static ApiResponse<T> Fail(string message, object? errors = null)
        => new() { Success = false, Message = message, Data = default, Errors = errors };
}
