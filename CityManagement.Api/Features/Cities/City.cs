using System.ComponentModel.DataAnnotations;

namespace CityManagement.Api.Features.Cities;

public class City
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string State { get; set; } = string.Empty;
    
    [Required]
    public string Country { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 5)]
    public int TouristRating { get; set; }
    
    [Required]
    public DateTime DateEstablished { get; set; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int EstimatedPopulation { get; set; }
}