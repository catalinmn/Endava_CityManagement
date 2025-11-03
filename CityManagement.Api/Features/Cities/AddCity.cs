using Microsoft.EntityFrameworkCore;
using CityManagement.Api.Data;
using System.ComponentModel.DataAnnotations;

namespace CityManagement.Api.Features.Cities;

public static class AddCity
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/cities", HandleAsync)
            .WithName("AddCity")
            .WithSummary("Create a new city")
            .WithDescription("Creates a new city record in the database")
            .ProducesValidationProblem()
            .Produces<CityResponse>(201);
    }

    internal static async Task<IResult> HandleAsync(
        AddCityRequest request,
        CityManagementContext context,
        ILogger<Program> logger)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(request);
        
        if (!Validator.TryValidateObject(request, validationContext, validationResults, validateAllProperties: true))
        {
            var errors = validationResults.ToDictionary(
                vr => vr.MemberNames.FirstOrDefault() ?? "Unknown",
                vr => new[] { vr.ErrorMessage ?? "Validation error" }
            );
            return Results.ValidationProblem(errors);
        }

        var city = new City
        {
            Name = request.Name,
            State = request.State,
            Country = request.Country,
            TouristRating = request.TouristRating,
            DateEstablished = request.DateEstablished,
            EstimatedPopulation = request.EstimatedPopulation
        };

        try
        {
            context.Cities.Add(city);
            await context.SaveChangesAsync();

            logger.LogInformation("Created city {CityName} with ID {CityId}", city.Name, city.Id);

            var response = new CityResponse
            {
                Id = city.Id,
                Name = city.Name,
                State = city.State,
                Country = city.Country,
                TouristRating = city.TouristRating,
                DateEstablished = city.DateEstablished,
                EstimatedPopulation = city.EstimatedPopulation
            };

            return Results.Created($"/cities/{city.Id}", response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating city {CityName}", request.Name);
            return Results.Problem("An error occurred while creating the city");
        }
    }
}