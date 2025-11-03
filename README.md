# City Management REST API

A minimal standalone C# REST API built with .NET 9, Entity Framework, and vertical-slice architecture. Supports CRUD operations for local city records and integrates with external APIs for enhanced search functionality.

## Features

### Core Operations
- **Add City**: POST /cities - Create new city records
- **Update City**: PUT /cities/{id} - Update tourist rating, date established, and estimated population
- **Delete City**: DELETE /cities/{id} - Remove city records
- **Search Cities**: GET /cities/search?name={value} - Search with local + external data merging

### Data Entity: City
- `id`: Generated primary key (int)
- `name`: City name (string, required)
- `state`: State/province (string, required)
- `country`: Country name (string, required)
- `touristRating`: Rating 1-5 (int, required)
- `dateEstablished`: Establishment date (DateTime, required)
- `estimatedPopulation`: Population count (int ≥ 0, required)

### External API Integration
- **REST Countries API**: Provides country codes and currency information
- **OpenWeatherMap API**: Provides weather data (requires API key configuration)

### Architecture
- **Vertical Slice Architecture**: Each feature is self-contained
- **Entity Framework**: Configurable for In-Memory or SQL Server
- **Built-in Testing**: Swagger UI for manual endpoint testing
- **Validation**: Request validation with detailed error responses

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Optional: SQL Server (for non in-memory database)
- Optional: OpenWeatherMap API key

### Quick Start

1. **Clone and build:**
   ```bash
   git clone <repository>
   cd CityManagement
   dotnet build
   ```

2. **Run with in-memory database:**
   ```bash
   cd CityManagement.Api
   $env:UseInMemoryDatabase='true'
   dotnet run --urls="http://localhost:5025"
   ```

3. **Open Swagger UI:**
   Navigate to http://localhost:5025 in your browser

### Configuration

#### Database Options

**In-Memory Database (Default):**
```json
{
  "UseInMemoryDatabase": true
}
```

**SQL Server:**
```json
{
  "UseInMemoryDatabase": false,
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CityManagementDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

#### External APIs

**OpenWeatherMap (Optional):**
```json
{
  "OpenWeatherMap": {
    "ApiKey": "your-api-key-here"
  }
}
```

## API Endpoints

### 1. Add City
**POST** `/cities`

**Request Body:**
```json
{
  "name": "New York",
  "state": "New York",
  "country": "United States",
  "touristRating": 5,
  "dateEstablished": "1624-01-01",
  "estimatedPopulation": 8000000
}
```

**Response (201):**
```json
{
  "id": 1,
  "name": "New York",
  "state": "New York",
  "country": "United States",
  "touristRating": 5,
  "dateEstablished": "1624-01-01T00:00:00",
  "estimatedPopulation": 8000000
}
```

### 2. Update City
**PUT** `/cities/{id}`

**Request Body:**
```json
{
  "touristRating": 4,
  "dateEstablished": "1624-01-01",
  "estimatedPopulation": 8100000
}
```

### 3. Delete City
**DELETE** `/cities/{id}`

**Response:** 204 No Content

### 4. Search Cities
**GET** `/cities/search?name=london`

**Behavior:**
- If local matches found: Returns local data supplemented with external APIs
- If no local matches: Returns external-only data (weather + country info)

**Response:**
```json
[
  {
    "id": 1,
    "name": "London",
    "state": "England",
    "country": "United Kingdom",
    "touristRating": 5,
    "dateEstablished": "1066-01-01T00:00:00",
    "estimatedPopulation": 9000000,
    "country2Code": "GB",
    "country3Code": "GBR",
    "currencyCode": "GBP",
    "weather": { /* OpenWeatherMap response */ }
  }
]
```

## Testing

### Unit Tests
Run the included unit tests:
```bash
cd CityManagement.Tests
dotnet test
```

**Minimum Required Test:** `AddCity_ValidRequest_ShouldCreateCityAndReturnCorrectResponse` validates the Add City operation.

### Manual Testing
Use the built-in Swagger UI at http://localhost:5025 to test all endpoints interactively.

## Project Structure

```
CityManagement/
├── CityManagement.Api/
│   ├── Features/Cities/           # Vertical slice for city operations
│   │   ├── City.cs               # Entity model
│   │   ├── CityDtos.cs           # Request/Response DTOs
│   │   ├── AddCity.cs            # POST endpoint
│   │   ├── UpdateCity.cs         # PUT endpoint
│   │   ├── DeleteCity.cs         # DELETE endpoint
│   │   └── SearchCities.cs       # GET search endpoint
│   ├── Data/
│   │   └── CityManagementContext.cs  # EF DbContext
│   ├── Services/                 # External API services
│   │   ├── RestCountriesService.cs
│   │   └── OpenWeatherMapService.cs
│   └── Program.cs                # Application startup
└── CityManagement.Tests/
    └── UnitTest1.cs              # Unit tests for Add City
```

## Technical Details

- **Framework**: .NET 9 Minimal APIs
- **Database**: Entity Framework Core with In-Memory or SQL Server providers
- **Validation**: Data Annotations with custom validation logic
- **HTTP Client**: Named HTTP clients for external API integration
- **Documentation**: Swagger/OpenAPI with SwaggerUI
- **Testing**: xUnit with Entity Framework In-Memory provider
- **Error Handling**: Structured error responses with validation details
- **Logging**: Built-in .NET logging throughout the application

## Limitations & Notes

- External API calls may fail if services are unavailable
- OpenWeatherMap requires API key for weather data
- In-memory database data is lost on application restart
- No authentication/authorization implemented (minimal scope)
- Search functionality treats the name parameter as a city name for external API calls