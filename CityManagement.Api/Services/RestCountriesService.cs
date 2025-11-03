using System.Text.Json;

namespace CityManagement.Api.Services;

public interface IRestCountriesService
{
    Task<CountryInfo?> GetCountryInfoAsync(string countryName);
}

public class RestCountriesService : IRestCountriesService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RestCountriesService> _logger;

    public RestCountriesService(HttpClient httpClient, ILogger<RestCountriesService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<CountryInfo?> GetCountryInfoAsync(string countryName)
    {
        try
        {
            var response = await _httpClient.GetAsync($"https://restcountries.com/v3.1/name/{countryName}?fields=cca2,cca3,currencies");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get country info for {CountryName}. Status: {StatusCode}", countryName, response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var countries = JsonSerializer.Deserialize<CountryApiResponse[]>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var country = countries?.FirstOrDefault();
            if (country == null) return null;

            var currency = country.Currencies?.FirstOrDefault();
            return new CountryInfo
            {
                Country2Code = country.Cca2,
                Country3Code = country.Cca3,
                CurrencyCode = currency?.Key ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting country info for {CountryName}", countryName);
            return null;
        }
    }
}

public record CountryInfo
{
    public string Country2Code { get; init; } = string.Empty;
    public string Country3Code { get; init; } = string.Empty;
    public string CurrencyCode { get; init; } = string.Empty;
}

public record CountryApiResponse
{
    public string Cca2 { get; init; } = string.Empty;
    public string Cca3 { get; init; } = string.Empty;
    public Dictionary<string, object>? Currencies { get; init; }
}