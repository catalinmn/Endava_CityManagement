using System.Text.Json;

namespace CityManagement.Api.Services;

public interface IOpenWeatherMapService
{
    Task<object?> GetWeatherAsync(string cityName);
}

public class OpenWeatherMapService : IOpenWeatherMapService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenWeatherMapService> _logger;
    private readonly string _apiKey;

    public OpenWeatherMapService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenWeatherMapService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["OpenWeatherMap:ApiKey"] ?? string.Empty;
    }

    public async Task<object?> GetWeatherAsync(string cityName)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            _logger.LogWarning("OpenWeatherMap API key not configured");
            return new { error = "Weather service not configured" };
        }

        try
        {
            var response = await _httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={_apiKey}&units=metric");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get weather for {CityName}. Status: {StatusCode}", cityName, response.StatusCode);
                return new { error = "Weather data not available" };
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting weather for {CityName}", cityName);
            return new { error = "Weather service unavailable" };
        }
    }
}