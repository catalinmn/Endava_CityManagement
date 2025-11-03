using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CityManagement.Api.Data;
using CityManagement.Api.Features.Cities;

namespace CityManagement.Tests;

public class AddCityTests
{
    private CityManagementContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<CityManagementContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new CityManagementContext(options);
    }

    [Fact]
    public async Task AddCity_ValidRequest_ShouldCreateCityAndReturnCorrectResponse()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var logger = new LoggerFactory().CreateLogger<Program>();

        var request = new AddCityRequest
        {
            Name = "Test City",
            State = "Test State",
            Country = "Test Country",
            TouristRating = 4,
            DateEstablished = new DateTime(2000, 1, 1),
            EstimatedPopulation = 100000
        };

        // Act
        var result = await AddCity.HandleAsync(request, context, logger);

        // Assert
        Assert.NotNull(result);
        
        // Verify city was saved to database
        var savedCity = await context.Cities.FirstOrDefaultAsync();
        Assert.NotNull(savedCity);
        Assert.Equal("Test City", savedCity.Name);
        Assert.Equal("Test State", savedCity.State);
        Assert.Equal("Test Country", savedCity.Country);
        Assert.Equal(4, savedCity.TouristRating);
        Assert.Equal(new DateTime(2000, 1, 1), savedCity.DateEstablished);
        Assert.Equal(100000, savedCity.EstimatedPopulation);
        Assert.True(savedCity.Id > 0);
    }

    [Fact]
    public async Task AddCity_InvalidTouristRating_ShouldReturnValidationProblem()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var logger = new LoggerFactory().CreateLogger<Program>();

        var request = new AddCityRequest
        {
            Name = "Test City",
            State = "Test State",
            Country = "Test Country",
            TouristRating = 6, // Invalid rating (should be 1-5)
            DateEstablished = new DateTime(2000, 1, 1),
            EstimatedPopulation = 100000
        };

        // Act
        var result = await AddCity.HandleAsync(request, context, logger);

        // Assert
        Assert.NotNull(result);
        
        // Verify no city was saved to database
        var cityCount = await context.Cities.CountAsync();
        Assert.Equal(0, cityCount);
    }

    [Fact]
    public async Task AddCity_NegativePopulation_ShouldReturnValidationProblem()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var logger = new LoggerFactory().CreateLogger<Program>();

        var request = new AddCityRequest
        {
            Name = "Test City",
            State = "Test State",
            Country = "Test Country",
            TouristRating = 3,
            DateEstablished = new DateTime(2000, 1, 1),
            EstimatedPopulation = -1 // Invalid population
        };

        // Act
        var result = await AddCity.HandleAsync(request, context, logger);

        // Assert
        Assert.NotNull(result);
        
        // Verify no city was saved to database
        var cityCount = await context.Cities.CountAsync();
        Assert.Equal(0, cityCount);
    }
}
