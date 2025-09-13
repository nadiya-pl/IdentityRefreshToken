using System.Text.Json.Serialization;

namespace Frontend.Models.Responses;

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public string TraceId { get; set; }
    public Dictionary<string, string> Errors { get; set; }
}
