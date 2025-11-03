using Microsoft.EntityFrameworkCore;
using CityManagement.Api.Data;

namespace CityManagement.Api.Features.Cities;

public static class DeleteCity
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/cities/{id:int}", HandleAsync)
            .WithName("DeleteCity")
            .WithSummary("Delete a city")
            .WithDescription("Removes a city record from the database")
            .Produces(204)
            .Produces(404);
    }

    private static async Task<IResult> HandleAsync(
        int id,
        CityManagementContext context,
        ILogger<Program> logger)
    {
        try
        {
            var city = await context.Cities.FindAsync(id);
            if (city == null)
            {
                logger.LogWarning("City with ID {CityId} not found", id);
                return Results.NotFound();
            }

            context.Cities.Remove(city);
            await context.SaveChangesAsync();

            logger.LogInformation("Deleted city {CityName} with ID {CityId}", city.Name, city.Id);

            return Results.NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting city with ID {CityId}", id);
            return Results.Problem("An error occurred while deleting the city");
        }
    }
}