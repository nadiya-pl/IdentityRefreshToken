using Frontend.Models.Dto;
using Frontend.Models.Responses;
using System.Text;
using System.Text.Json;

namespace Frontend.HttpClients;

public class AuthHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthHttpClient> _logger;
    public AuthHttpClient(HttpClient httpClient, ILogger<AuthHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ApiResponse> Register(RegisterDto dto)
    {
        try
        {
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/user/register", content);

            return await HandleResponse(response);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "[AuthHttpClient] Request failed in Register");

            var result = new ApiResponse();
            result.Success = false;
            result.Message = $"Request error [Register]: {ex.Message}";
            return result;
        }
    }

    public async Task<ApiResponse> Login(LoginDto dto)
    {
        try
        {
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/user/login", content);

            return await HandleResponse(response);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "[AuthHttpClient] Request failed in Login");

            var result = new ApiResponse();
            result.Success = false;
            result.Message = $"Request error [Login]: {ex.Message}";
            return result;
        }
    }

    public async Task<ApiResponse> RefreshTokens(TokensDto dto)
    {        
        try
        {
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/user/refresh", content);

            return await HandleResponse(response);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "[AuthHttpClient] Request failed in RefreshTokens");

            var result = new ApiResponse();
            result.Success = false;
            result.Message = $"Request error [RefreshTokens]: {ex.Message}";
            return result;
        }
    }

    private async Task<ApiResponse> HandleResponse(HttpResponseMessage response)
    {
        var result = new ApiResponse();

        try
        {
            result = await response.Content.ReadFromJsonAsync<ApiResponse>();

            if(result == null)
            {
                result = new ApiResponse();
                result.Success = false;
                result.Message = "Failed to deserialize response.";
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = $"Error while processing response: {ex.Message}";
        }

        return result;
    }

}
