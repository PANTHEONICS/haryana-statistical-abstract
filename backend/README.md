# Haryana Statistical Abstract - Backend API

.NET Core 8.0 Web API for managing census population data from Statistical Abstract of Haryana 2023-24, Table 3.2.

## Features

- ✅ CRUD operations for Census Population data
- ✅ RESTful API design
- ✅ Middleware for error handling and request logging
- ✅ Entity Framework Core with In-Memory database (development)
- ✅ Swagger/OpenAPI documentation
- ✅ CORS support for frontend integration
- ✅ Comprehensive unit tests
- ✅ Seed data from mock-data folder

## Technology Stack

- **.NET 8.0** - Framework
- **Entity Framework Core 8.0** - ORM
- **In-Memory Database** - For development/testing
- **Swagger/OpenAPI** - API documentation
- **Serilog** - Logging
- **xUnit** - Unit testing
- **FluentAssertions** - Test assertions
- **Moq** - Mocking framework

## Project Structure

```
backend/
├── HaryanaStatAbstract.API/
│   ├── Controllers/          # API Controllers
│   ├── Data/                 # DbContext and Seed Data
│   ├── Middleware/           # Custom middleware
│   ├── Models/               # Entity models and DTOs
│   ├── Services/             # Business logic services
│   ├── Program.cs            # Application entry point
│   └── appsettings.json      # Configuration
├── HaryanaStatAbstract.API.Tests/
│   ├── Controllers/          # Controller tests
│   └── Services/             # Service tests
└── README.md                 # This file
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 / VS Code / JetBrains Rider

### Installation

1. **Navigate to the backend directory:**
   ```bash
   cd backend/HaryanaStatAbstract.API
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Run the application:**
   ```bash
   dotnet run
   ```

4. **Access Swagger UI:**
   - Navigate to `http://localhost:5000` or `https://localhost:5001`
   - Swagger UI is available at the root URL

### Running Tests

```bash
cd backend/HaryanaStatAbstract.API.Tests
dotnet test
```

## API Endpoints

### Base URL
```
http://localhost:5000/api/v1/CensusPopulation
```

### Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/CensusPopulation` | Get all census records |
| GET | `/api/v1/CensusPopulation/{year}` | Get record by year |
| GET | `/api/v1/CensusPopulation/range?startYear={start}&endYear={end}` | Get records by year range |
| POST | `/api/v1/CensusPopulation` | Create new record |
| PUT | `/api/v1/CensusPopulation/{year}` | Update existing record |
| DELETE | `/api/v1/CensusPopulation/{year}` | Delete record |

### Example Requests

#### Get All Records
```bash
GET http://localhost:5000/api/v1/CensusPopulation
```

#### Get Record by Year
```bash
GET http://localhost:5000/api/v1/CensusPopulation/2011
```

#### Create New Record
```bash
POST http://localhost:5000/api/v1/CensusPopulation
Content-Type: application/json

{
  "year": 2021,
  "totalPopulation": 30000000,
  "malePopulation": 15000000,
  "femalePopulation": 15000000,
  "sexRatio": 1000,
  "sourceDocument": "Statistical Abstract of Haryana 2023-24",
  "sourceTable": "Table 3.2"
}
```

#### Update Record
```bash
PUT http://localhost:5000/api/v1/CensusPopulation/2011
Content-Type: application/json

{
  "totalPopulation": 26000000,
  "malePopulation": 13000000,
  "femalePopulation": 13000000,
  "sexRatio": 1000
}
```

#### Delete Record
```bash
DELETE http://localhost:5000/api/v1/CensusPopulation/2011
```

## Middleware

### Error Handling Middleware
Global exception handling that returns standardized error responses:
- Catches all unhandled exceptions
- Returns appropriate HTTP status codes
- Logs errors using Serilog

### Request Logging Middleware
Logs all incoming requests and outgoing responses:
- Request method and path
- Remote IP address
- Response status code
- Request duration

## Data Models

### CensusPopulation Entity
- `Year` (int) - Primary key
- `TotalPopulation` (long) - Total population count
- `VariationInPopulation` (long?) - Change from previous census
- `DecennialPercentageIncrease` (decimal?) - Growth rate percentage
- `MalePopulation` (long) - Male population count
- `FemalePopulation` (long) - Female population count
- `SexRatio` (int) - Females per 1000 males
- `SourceDocument` (string?) - Source document name
- `SourceTable` (string?) - Source table reference
- `SourceReference` (string?) - Source reference

### Validation Rules
- Total population must equal sum of male and female population
- Year must be between 1900 and 2100
- All population counts must be greater than 0
- Sex ratio must be between 0 and 2000

## Database Configuration

### Development
Uses **In-Memory Database** for easy testing. Data is automatically seeded on application startup.

### Production
Configure SQL Server connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YourServer;Database=HaryanaStatAbstractDb;Trusted_Connection=True;"
  }
}
```

Update `Program.cs` to use SQL Server:
```csharp
options.UseSqlServer(connectionString);
```

## CORS Configuration

CORS is configured to allow requests from:
- `http://localhost:5173` (Vite default)
- `http://localhost:3000` (React default)

To add more origins, update `Program.cs`:
```csharp
policy.WithOrigins("http://localhost:5173", "https://yourdomain.com")
```

## Logging

Logs are written to:
- Console (stdout)
- File: `logs/haryana-stat-abstract-YYYYMMDD.log`

Configure log levels in `appsettings.json`:
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning"
      }
    }
  }
}
```

## Health Checks

Health check endpoint available at:
```
GET /health
```

## Testing

### Run All Tests
```bash
dotnet test
```

### Run Tests with Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## API Documentation

Swagger UI provides interactive API documentation:
- Visit `http://localhost:5000` when running the application
- Test endpoints directly from the browser
- View request/response schemas

## Integration with Frontend

The API is configured to work with the React frontend:
1. Start the backend API
2. Update frontend API base URL to `http://localhost:5000/api/v1`
3. CORS is already configured to allow requests from `http://localhost:5173`

## Development Notes

- Seed data is automatically loaded on application startup (development only)
- In-Memory database is used for development/testing
- All timestamps are stored in UTC
- Validation errors return 400 Bad Request
- Missing records return 404 Not Found
- Duplicate records return 409 Conflict

## Next Steps

1. **Add Authentication**: Implement JWT-based authentication
2. **Add Pagination**: Implement pagination for GET all endpoints
3. **Add Filtering**: Add query parameters for filtering records
4. **Add Sorting**: Implement sorting capabilities
5. **Add Caching**: Implement Redis caching for frequently accessed data
6. **Add Rate Limiting**: Implement rate limiting middleware
7. **Production Database**: Configure SQL Server or PostgreSQL for production

## License

This project is part of the Haryana Statistical Abstract application.