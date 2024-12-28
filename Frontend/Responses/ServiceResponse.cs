namespace Frontend.Responses;

public class ServiceResponse
{
    public bool Success { get; set; }
    public object? Data { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}