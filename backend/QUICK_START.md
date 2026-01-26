# Quick Start Guide

Get the API up and running in minutes!

## 1. Install Prerequisites

- Install [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

Verify installation:
```bash
dotnet --version
```

## 2. Navigate to Project

```bash
cd backend/HaryanaStatAbstract.API
```

## 3. Restore Dependencies

```bash
dotnet restore
```

## 4. Build the Project

```bash
dotnet build
```

## 5. Run the Application

```bash
dotnet run
```

You should see:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
      Now listening on: https://localhost:5001
```

## 6. Access Swagger UI

Open your browser and navigate to:
```
http://localhost:5000
```

Swagger UI will display all available API endpoints.

## 7. Test the API

### Quick Test - Get All Records

In Swagger UI:
1. Click on `GET /api/v1/CensusPopulation`
2. Click "Try it out"
3. Click "Execute"
4. You should see 12 census records (1901-2011)

### Quick Test - Get Record by Year

1. Click on `GET /api/v1/CensusPopulation/{year}`
2. Click "Try it out"
3. Enter `2011` in the year field
4. Click "Execute"
5. You should see the 2011 census data

## 8. Run Tests

Open a new terminal:

```bash
cd backend/HaryanaStatAbstract.API.Tests
dotnet test
```

All tests should pass!

## Common Issues

### Port Already in Use

If port 5000 is already in use:
1. Stop the other application
2. Or update `launchSettings.json` to use a different port

### Swagger Not Loading

Make sure you're running in Development mode:
```bash
dotnet run --environment Development
```

### Tests Failing

Make sure you've restored dependencies:
```bash
dotnet restore
dotnet test
```

## Next Steps

- Read [README.md](README.md) for detailed documentation
- Read [API_TESTING_GUIDE.md](API_TESTING_GUIDE.md) for testing examples
- Integrate with the frontend React application

## Production Deployment

For production:
1. Update `appsettings.json` with SQL Server connection string
2. Update `Program.cs` to use SQL Server instead of In-Memory database
3. Configure environment variables
4. Set up proper logging and monitoring

Happy coding! 🚀