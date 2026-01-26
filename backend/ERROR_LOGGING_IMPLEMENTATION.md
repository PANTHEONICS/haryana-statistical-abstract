# Error Logging System - Implementation Guide

## Overview

A comprehensive, generalized error logging mechanism that automatically captures and stores runtime errors and exceptions in the database. This system provides:

- **Automatic Error Capture**: All unhandled exceptions are automatically logged
- **Database Storage**: Errors are stored in the `Error_Logs` table
- **Rich Context**: Captures user information, request details, stack traces, and more
- **Error Management**: Admin interface to view, filter, and resolve errors
- **Statistics**: Error analytics and reporting

---

## Database Setup

### Step 1: Run the Schema Script

Execute the SQL script to create the error logging tables and stored procedures:

```sql
-- Execute in SQL Server Management Studio:
backend/database/error-logging-schema.sql
```

This creates:
- `Error_Logs` table
- `sp_LogError` stored procedure
- `sp_GetErrorLogs` stored procedure
- `sp_MarkErrorResolved` stored procedure

---

## Architecture

### Components

1. **ErrorLog Model** (`Models/ErrorLog.cs`)
   - Entity model for the `Error_Logs` table

2. **ErrorLoggingService** (`Services/ErrorLoggingService.cs`)
   - Core service for logging errors
   - Implements `IErrorLoggingService`

3. **GlobalErrorHandlingMiddleware** (`Middleware/GlobalErrorHandlingMiddleware.cs`)
   - Catches all unhandled exceptions
   - Automatically logs to database
   - Returns user-friendly error responses

4. **ErrorLogController** (`Controllers/ErrorLogController.cs`)
   - Admin API endpoints for viewing and managing error logs

5. **ErrorLoggingExtensions** (`Extensions/ErrorLoggingExtensions.cs`)
   - Helper methods for easy error logging from anywhere

---

## Usage

### Automatic Error Logging

The `GlobalErrorHandlingMiddleware` automatically catches all unhandled exceptions. No additional code needed!

**Example:**
```csharp
// This exception will be automatically logged
public async Task<IActionResult> GetData()
{
    throw new InvalidOperationException("Something went wrong");
    // Error is automatically logged to database with full context
}
```

### Manual Error Logging

You can manually log errors using the service:

```csharp
public class MyService
{
    private readonly IErrorLoggingService _errorLoggingService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MyService(
        IErrorLoggingService errorLoggingService,
        IHttpContextAccessor httpContextAccessor)
    {
        _errorLoggingService = errorLoggingService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task DoSomething()
    {
        try
        {
            // Your code here
        }
        catch (Exception ex)
        {
            // Log with HTTP context
            await _errorLoggingService.LogExceptionWithContextAsync(
                ex,
                _httpContextAccessor.HttpContext,
                source: "MyService",
                methodName: nameof(DoSomething)
            );
            throw; // Re-throw if needed
        }
    }
}
```

### Logging Simple Errors

```csharp
await _errorLoggingService.LogErrorWithContextAsync(
    "Custom error message",
    "Warning",
    _httpContextAccessor.HttpContext,
    source: "MyService",
    methodName: nameof(MyMethod)
);
```

---

## API Endpoints

### Get Error Logs (Admin Only)

```http
GET /api/ErrorLog?startDate=2024-01-01&endDate=2024-01-31&errorLevel=Error&isResolved=false&pageNumber=1&pageSize=50
```

**Query Parameters:**
- `startDate` (optional): Filter errors from this date
- `endDate` (optional): Filter errors until this date
- `errorLevel` (optional): Filter by level (Error, Warning, Information, Critical, Debug)
- `isResolved` (optional): Filter by resolution status
- `userId` (optional): Filter by user ID
- `source` (optional): Filter by source (e.g., "Controller", "Service")
- `pageNumber` (default: 1): Page number for pagination
- `pageSize` (default: 50): Number of records per page

**Response:**
```json
{
  "data": [
    {
      "errorLogID": 1,
      "errorLevel": "Error",
      "errorMessage": "Something went wrong",
      "exceptionType": "System.InvalidOperationException",
      "stackTrace": "...",
      "source": "MyController",
      "requestPath": "/api/data",
      "requestMethod": "GET",
      "userID": 123,
      "userLoginID": "admin",
      "ipAddress": "127.0.0.1",
      "createdAt": "2024-01-22T10:30:00Z",
      "isResolved": false
    }
  ],
  "totalCount": 100,
  "pageNumber": 1,
  "pageSize": 50,
  "totalPages": 2
}
```

### Get Error Log by ID (Admin Only)

```http
GET /api/ErrorLog/{id}
```

### Mark Error as Resolved (Admin Only)

```http
POST /api/ErrorLog/{id}/resolve
Content-Type: application/json

{
  "resolutionNotes": "Fixed by updating the service logic"
}
```

### Get Error Statistics (Admin Only)

```http
GET /api/ErrorLog/statistics?startDate=2024-01-01&endDate=2024-01-31
```

**Response:**
```json
{
  "totalErrors": 150,
  "unresolvedErrors": 25,
  "resolvedErrors": 125,
  "criticalErrors": 5,
  "errorsByLevel": {
    "Error": 100,
    "Warning": 40,
    "Critical": 5,
    "Information": 5
  },
  "errorsBySource": {
    "MyController": 50,
    "MyService": 30,
    "GlobalErrorHandlingMiddleware": 70
  }
}
```

---

## Error Log Fields

| Field | Type | Description |
|-------|------|-------------|
| ErrorLogID | BIGINT | Primary key (auto-increment) |
| ErrorLevel | NVARCHAR(50) | Error, Warning, Information, Critical, Debug |
| ErrorMessage | NVARCHAR(MAX) | Error message |
| ExceptionType | NVARCHAR(200) | Full name of exception type |
| StackTrace | NVARCHAR(MAX) | Stack trace |
| InnerException | NVARCHAR(MAX) | Inner exception details |
| Source | NVARCHAR(500) | Source component (Controller, Service, etc.) |
| MethodName | NVARCHAR(200) | Method where error occurred |
| RequestPath | NVARCHAR(500) | HTTP request path |
| RequestMethod | NVARCHAR(10) | HTTP method (GET, POST, etc.) |
| UserID | INT | User ID (if authenticated) |
| UserLoginID | NVARCHAR(50) | User login ID |
| IPAddress | NVARCHAR(50) | Client IP address |
| RequestHeaders | NVARCHAR(MAX) | Request headers (JSON, excludes sensitive data) |
| RequestBody | NVARCHAR(MAX) | Request body (if available) |
| QueryString | NVARCHAR(MAX) | Query string parameters |
| AdditionalData | NVARCHAR(MAX) | Additional context (JSON) |
| IsResolved | BIT | Whether error is resolved |
| ResolvedBy | INT | User ID who resolved it |
| ResolvedAt | DATETIME2 | Resolution timestamp |
| ResolutionNotes | NVARCHAR(MAX) | Resolution notes |
| CreatedAt | DATETIME2 | Error timestamp |

---

## Error Levels

- **Critical**: System-critical errors that require immediate attention
- **Error**: Application errors that need investigation
- **Warning**: Non-critical issues that should be monitored
- **Information**: Informational messages
- **Debug**: Debug information (typically not logged to database)

---

## Best Practices

1. **Don't Log Sensitive Data**: The system automatically excludes Authorization headers and cookies
2. **Use Appropriate Error Levels**: Choose the right level for each error
3. **Provide Context**: Include source and method name when logging manually
4. **Resolve Errors**: Mark errors as resolved after fixing them
5. **Monitor Statistics**: Regularly check error statistics to identify patterns

---

## Integration

The error logging system is automatically integrated:

1. **Database**: `Error_Logs` table created via schema script
2. **Service**: `IErrorLoggingService` registered in `Program.cs`
3. **Middleware**: `GlobalErrorHandlingMiddleware` added to pipeline
4. **Controller**: `ErrorLogController` available for admin access

---

## Testing

To test the error logging:

1. **Trigger an error** in any API endpoint
2. **Check the database**: Query `Error_Logs` table
3. **View via API**: Call `GET /api/ErrorLog` (as admin)
4. **Check statistics**: Call `GET /api/ErrorLog/statistics`

---

## Maintenance

### Cleanup Old Error Logs

```sql
-- Delete resolved errors older than 90 days
DELETE FROM [dbo].[Error_Logs]
WHERE [IsResolved] = 1
AND [ResolvedAt] < DATEADD(DAY, -90, GETUTCDATE());
```

### Archive Error Logs

Consider archiving old error logs to a separate table for long-term storage.

---

## Security

- **Admin Only**: Error log viewing and management requires System Admin role
- **Sensitive Data**: Authorization headers and cookies are automatically excluded
- **Request Body**: Only logged for debugging purposes (consider excluding in production)

---

## Troubleshooting

### Errors Not Being Logged

1. Check if `GlobalErrorHandlingMiddleware` is registered in `Program.cs`
2. Verify `IErrorLoggingService` is registered
3. Check database connection
4. Review application logs for error logging failures

### Database Logging Fails

If database logging fails, errors are still logged to the application logger (Serilog). Check log files in `logs/` directory.

---

## Summary

The error logging system provides:
- ✅ Automatic error capture
- ✅ Database storage with rich context
- ✅ Admin interface for error management
- ✅ Error statistics and reporting
- ✅ Easy-to-use extension methods
- ✅ Generalized and reusable across the application

All runtime errors are now automatically logged to the database for monitoring and debugging!
