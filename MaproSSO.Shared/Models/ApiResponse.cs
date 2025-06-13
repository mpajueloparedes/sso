using System.Text.Json.Serialization;

namespace MaproSSO.Shared.Models;

public class ApiResponse<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("errors")]
    public List<string> Errors { get; set; } = new();

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("requestId")]
    public string? RequestId { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Operation completed successfully")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string> { message }
        };
    }

    public static ApiResponse<T> ErrorResponse(List<string> errors)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = "One or more validation errors occurred",
            Errors = errors
        };
    }
}

//public class ApiResponse : ApiResponse<object>
//{
//    public static ApiResponse Success(string message = "Operation completed successfully")
//    {
//        return new ApiResponse
//        {
//            Success = true,
//            Message = message
//        };
//    }

//    public static new ApiResponse ErrorResponse(string message, List<string>? errors = null)
//    {
//        return new ApiResponse
//        {
//            Success = false,
//            Message = message,
//            Errors = errors ?? new List<string> { message }
//        };
//    }
//}