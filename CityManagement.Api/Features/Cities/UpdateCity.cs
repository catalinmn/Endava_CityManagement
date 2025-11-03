using Microsoft.EntityFrameworkCore;
using CityManagement.Api.Data;
using System.ComponentModel.DataAnnotations;

namespace CityManagement.Api.Features.Cities;

public static class UpdateCity
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/cities/{id:int}", HandleAsync)
            .WithName("UpdateCity")
            .WithSummary("Update an existing city")
            .WithDescription("Updates tourist rating, date established, and estimated population of an existing city")
            .ProducesValidationProblem()
            .Produces<CityResponse>(200)
            .Produces(404);
    }

    private static async Task<IResult> HandleAsync(
        int id,
        UpdateCityRequest request,
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

        try
        {
            var city = await context.Cities.FindAsync(id);
            if (city == null)
            {
                logger.LogWarning("City with ID {CityId} not found", id);
                return Results.NotFound();
            }

            city.TouristRating = request.TouristRating;
            city.DateEstablished = request.DateEstablished;
            city.EstimatedPopulation = request.EstimatedPopulation;

            await context.SaveChangesAsync();

            logger.LogInformation("Updated city {CityName} with ID {CityId}", city.Name, city.Id);

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

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating city with ID {CityId}", id);
            return Results.Problem("An error occurred while updating the city");
        }
    }
}