using System.ComponentModel.DataAnnotations;

namespace CityManagement.Api.Features.Cities;

public record AddCityRequest
{
    [Required]
    public string Name { get; init; } = string.Empty;
    
    [Required]
    public string State { get; init; } = string.Empty;
    
    [Required]
    public string Country { get; init; } = string.Empty;
    
    [Required]
    [Range(1, 5)]
    public int TouristRating { get; init; }
    
    [Required]
    public DateTime DateEstablished { get; init; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int EstimatedPopulation { get; init; }
}

public record UpdateCityRequest
{
    [Required]
    [Range(1, 5)]
    public int TouristRating { get; init; }
    
    [Required]
    public DateTime DateEstablished { get; init; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int EstimatedPopulation { get; init; }
}

public record CityResponse
{
    public int? Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? State { get; init; }
    public string Country { get; init; } = string.Empty;
    public int? TouristRating { get; init; }
    public DateTime? DateEstablished { get; init; }
    public int? EstimatedPopulation { get; init; }
    public string? Country2Code { get; init; }
    public string? Country3Code { get; init; }
    public string? CurrencyCode { get; init; }
    public object? Weather { get; init; }
}