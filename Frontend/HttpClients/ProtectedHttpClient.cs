using Frontend.Models.Responses;
using Frontend.TokenManager;

namespace Frontend.HttpClients;

public class ProtectedHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ITokenManager _tokenProvider;
    private readonly ILogger<ProtectedHttpClient> _logger;

    public ProtectedHttpClient(HttpClient httpClient, ITokenManager tokenProvider, ILogger<ProtectedHttpClient> logger)
    {
        _httpClient = httpClient;
        _tokenProvider = tokenProvider;
        _logger = logger;
    }

    public async Task<ProtectedResponse> GetProtectedData()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/protected");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenProvider.GetAccessToken());

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            return await HandleResponse(response);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "[ProtectedHttpClient] Request failed");
            return null;
        }
    }

    private async Task<ProtectedResponse> HandleResponse(HttpResponseMessage response)
    {
        var result = new ProtectedResponse();

        try
        {
            result = await response.Content.ReadFromJsonAsync<ProtectedResponse>();

            if (result == null)
            {
                result = new ProtectedResponse();
                result.Success = false;
                result.Message = "Ошибка десериализации объекта.";
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = $"Ошибка при обработке ответа: {ex.Message}";
        }

        return result;
    }

}
