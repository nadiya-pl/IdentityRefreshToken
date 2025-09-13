using System.Text.Json.Serialization;

namespace Frontend.Models.Responses;

public class ProtectedResponse
{
    public bool Success { get; set; }
    public string Data { get; set; }
    public string Message { get; set; }
    public string TraceId { get; set; }
}
