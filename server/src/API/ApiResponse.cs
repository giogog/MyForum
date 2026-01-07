namespace API;

public record ApiResponse
{
    public string Message { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public object? Result { get; set; }
    public int StatusCode { get; set; }
    public ApiResponse()
    {

    }
    public ApiResponse(string message, bool success, object? result, int statuscode)
    {
        Message = message;
        IsSuccess = success;
        Result = result;
        StatusCode = statuscode;
    }
}

