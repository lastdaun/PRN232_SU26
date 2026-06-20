using System.Text.Json.Serialization;

namespace PRN232.LMS.API.Models.Responses;

public class ApiResponse<T>
{
    private static readonly object EmptyData = new { };

    public bool Success { get; set; }
    public string Message { get; set; } = null!;

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public T? Data { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public object? Errors { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "Request processed successfully")
        => new() { Success = true, Message = message, Data = data, Errors = null };

    public static ApiResponse<object> OkEmpty(string message = "Request processed successfully")
        => new() { Success = true, Message = message, Data = EmptyData, Errors = null };

    public static ApiResponse<T> Fail(string message, object? errors = null)
        => new() { Success = false, Message = message, Data = default, Errors = errors };
}
