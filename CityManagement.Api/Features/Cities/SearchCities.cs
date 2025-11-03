using Microsoft.EntityFrameworkCore;
using CityManagement.Api.Data;
using CityManagement.Api.Services;

namespace CityManagement.Api.Features.Cities;

public static class SearchCities
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/cities/search", HandleAsync)
            .WithName("SearchCities")
            .WithSummary("Search for cities")
            .WithDescription("Search for cities by name, returns local data supplemented with external data or external-only data if no local matches")
            .Produces<IEnumerable<CityResponse>>(200);
    }

    private static async Task<IResult> HandleAsync(
        string name,
        CityManagementContext context,
        IRestCountriesService restCountriesService,
        IOpenWeatherMapService weatherService,
        ILogger<Program> logger)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Results.BadRequest("Name parameter is required");
        }

        try
        {
            // Search for local matches (case-insensitive)
            var localCities = await context.Cities
                .Where(c => EF.Functions.Like(c.Name.ToLower(), $"%{name.ToLower()}%"))
                .ToListAsync();

            var responses = new List<CityResponse>();

            if (localCities.Any())
            {
                // Supplement local data with external data
                foreach (var city in localCities)
                {
                    var countryInfo = await restCountriesService.GetCountryInfoAsync(city.Country);
                    var weather = await weatherService.GetWeatherAsync(city.Name);

                    responses.Add(new CityResponse
                    {
                        Id = city.Id,
                        Name = city.Name,
                        State = city.State,
                        Country = city.Country,
                        TouristRating = city.TouristRating,
                        DateEstablished = city.DateEstablished,
                        EstimatedPopulation = city.EstimatedPopulation,
                        Country2Code = countryInfo?.Country2Code,
                        Country3Code = countryInfo?.Country3Code,
                        CurrencyCode = countryInfo?.CurrencyCode,
                        Weather = weather
                    });
                }

                logger.LogInformation("Found {Count} local cities matching '{Name}'", localCities.Count, name);
            }
            else
            {
                // No local matches, try to get external data for the search term
                // We'll treat the search term as a city name and try to get weather data
                var weather = await weatherService.GetWeatherAsync(name);
                
                if (weather != null)
                {
                    // Try to extract country from weather data or use a default
                    responses.Add(new CityResponse
                    {
                        Id = null,
                        Name = name,
                        State = null,
                        Country = "Unknown", // Would need to extract from weather API response
                        TouristRating = null,
                        DateEstablished = null,
                        EstimatedPopulation = null,
                        Country2Code = null,
                        Country3Code = null,
                        CurrencyCode = null,
                        Weather = weather
                    });
                }

                logger.LogInformation("No local cities found for '{Name}', returned external data only", name);
            }

            return Results.Ok(responses);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching for cities with name '{Name}'", name);
            return Results.Problem("An error occurred while searching for cities");
        }
    }
}