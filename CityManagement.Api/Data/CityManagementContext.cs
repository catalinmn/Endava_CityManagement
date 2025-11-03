using Microsoft.EntityFrameworkCore;
using CityManagement.Api.Features.Cities;

namespace CityManagement.Api.Data;

public class CityManagementContext : DbContext
{
    public CityManagementContext(DbContextOptions<CityManagementContext> options) : base(options)
    {
    }

    public DbSet<City> Cities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.State).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TouristRating).IsRequired();
            entity.Property(e => e.DateEstablished).IsRequired();
            entity.Property(e => e.EstimatedPopulation).IsRequired();

            entity.HasIndex(e => e.Name);
        });
    }
}