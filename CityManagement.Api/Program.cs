using Microsoft.EntityFrameworkCore;
using CityManagement.Api.Data;
using CityManagement.Api.Features.Cities;
using CityManagement.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "City Management API", Version = "v1" });
});

// Configure Entity Framework
var useInMemoryDatabase = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", true);
if (useInMemoryDatabase)
{
    builder.Services.AddDbContext<CityManagementContext>(options =>
        options.UseInMemoryDatabase("CityManagementDb"));
}
else
{
    builder.Services.AddDbContext<CityManagementContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// Register HTTP clients and services
builder.Services.AddHttpClient<IRestCountriesService, RestCountriesService>();
builder.Services.AddHttpClient<IOpenWeatherMapService, OpenWeatherMapService>();

// Add CORS for Swagger UI
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "City Management API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseCors();
app.UseHttpsRedirection();

// Map city endpoints
AddCity.MapEndpoint(app);
UpdateCity.MapEndpoint(app);
DeleteCity.MapEndpoint(app);
SearchCities.MapEndpoint(app);

// Ensure database is created for in-memory database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CityManagementContext>();
    context.Database.EnsureCreated();
}

app.Run();
