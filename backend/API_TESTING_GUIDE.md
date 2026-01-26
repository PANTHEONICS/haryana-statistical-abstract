# API Testing Guide

This guide provides instructions for testing the Haryana Statistical Abstract API.

## Prerequisites

1. .NET 8.0 SDK installed
2. Restore NuGet packages: `dotnet restore`
3. Run the application: `dotnet run`

## Base URL

```
http://localhost:5000/api/v1/CensusPopulation
```

## Testing Methods

### 1. Using Swagger UI (Recommended)

1. Start the application
2. Navigate to `http://localhost:5000`
3. Swagger UI will display all available endpoints
4. Click on any endpoint to expand and test it

### 2. Using cURL

#### Get All Records
```bash
curl -X GET "http://localhost:5000/api/v1/CensusPopulation" \
  -H "accept: application/json"
```

#### Get Record by Year
```bash
curl -X GET "http://localhost:5000/api/v1/CensusPopulation/2011" \
  -H "accept: application/json"
```

#### Get Records by Year Range
```bash
curl -X GET "http://localhost:5000/api/v1/CensusPopulation/range?startYear=2000&endYear=2010" \
  -H "accept: application/json"
```

#### Create New Record
```bash
curl -X POST "http://localhost:5000/api/v1/CensusPopulation" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d "{
  \"year\": 2021,
  \"totalPopulation\": 30000000,
  \"malePopulation\": 15000000,
  \"femalePopulation\": 15000000,
  \"sexRatio\": 1000,
  \"sourceDocument\": \"Statistical Abstract of Haryana 2023-24\",
  \"sourceTable\": \"Table 3.2\",
  \"sourceReference\": \"Census of India, 2011, Administrative Atlas\"
}"
```

#### Update Record
```bash
curl -X PUT "http://localhost:5000/api/v1/CensusPopulation/2011" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d "{
  \"totalPopulation\": 26000000,
  \"malePopulation\": 13000000,
  \"femalePopulation\": 13000000,
  \"sexRatio\": 1000
}"
```

#### Delete Record
```bash
curl -X DELETE "http://localhost:5000/api/v1/CensusPopulation/2021" \
  -H "accept: application/json"
```

### 3. Using PowerShell (Windows)

#### Get All Records
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/v1/CensusPopulation" -Method Get
```

#### Get Record by Year
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/v1/CensusPopulation/2011" -Method Get
```

#### Create New Record
```powershell
$body = @{
    year = 2021
    totalPopulation = 30000000
    malePopulation = 15000000
    femalePopulation = 15000000
    sexRatio = 1000
    sourceDocument = "Statistical Abstract of Haryana 2023-24"
    sourceTable = "Table 3.2"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/v1/CensusPopulation" -Method Post -Body $body -ContentType "application/json"
```

#### Update Record
```powershell
$body = @{
    totalPopulation = 26000000
    malePopulation = 13000000
    femalePopulation = 13000000
    sexRatio = 1000
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/v1/CensusPopulation/2011" -Method Put -Body $body -ContentType "application/json"
```

#### Delete Record
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/v1/CensusPopulation/2021" -Method Delete
```

### 4. Using Postman

1. Import the API collection (if available)
2. Or manually create requests using the endpoints above
3. Set the base URL to `http://localhost:5000/api/v1/CensusPopulation`
4. Set appropriate headers: `Content-Type: application/json`

## Test Scenarios

### Scenario 1: Retrieve All Records
**Expected**: Returns all 12 census records (1901-2011)

### Scenario 2: Retrieve Specific Year
**Test**: GET `/api/v1/CensusPopulation/2011`
**Expected**: Returns 2011 census data

### Scenario 3: Create New Record
**Test**: POST with valid data
**Expected**: Returns 201 Created with the new record

### Scenario 4: Create Duplicate Record
**Test**: POST with year that already exists
**Expected**: Returns 409 Conflict

### Scenario 5: Update Record
**Test**: PUT with existing year
**Expected**: Returns 200 OK with updated record

### Scenario 6: Update Non-existent Record
**Test**: PUT with non-existent year
**Expected**: Returns 404 Not Found

### Scenario 7: Delete Record
**Test**: DELETE with existing year
**Expected**: Returns 204 No Content

### Scenario 8: Validation Error
**Test**: POST with total population != male + female
**Expected**: Returns 400 Bad Request

### Scenario 9: Year Range Query
**Test**: GET `/api/v1/CensusPopulation/range?startYear=2000&endYear=2010`
**Expected**: Returns records for 2001 only

## Response Codes

- **200 OK**: Successful GET or PUT request
- **201 Created**: Successful POST request
- **204 No Content**: Successful DELETE request
- **400 Bad Request**: Validation error or invalid input
- **404 Not Found**: Resource not found
- **409 Conflict**: Duplicate record (year already exists)
- **500 Internal Server Error**: Server error

## Running Unit Tests

```bash
cd backend/HaryanaStatAbstract.API.Tests
dotnet test
```

Expected output: All tests should pass

## Health Check

Test the health check endpoint:
```bash
curl -X GET "http://localhost:5000/health"
```

Expected: Returns health status

## Notes

- The API uses In-Memory database for development/testing
- Data is automatically seeded on application startup
- All timestamps are in UTC
- Year is the primary key and must be unique