using System.Net;
using System.Text;
using System.Text.Json;
using HaryanaStatAbstract.API.Models.Dtos;
using HaryanaStatAbstract.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HaryanaStatAbstract.API.Middleware
{
    /// <summary>
    /// Global Error Handling Middleware
    /// Catches all unhandled exceptions and logs them to the database
    /// </summary>
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public GlobalErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<GlobalErrorHandlingMiddleware> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;

            // Get user information from context
            int? userId = null;
            string? userLoginId = null;
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var parsedUserId))
                {
                    userId = parsedUserId;
                }

                var loginIdClaim = context.User.FindFirst("LoginID");
                userLoginId = loginIdClaim?.Value;
            }

            // Get request information
            var requestPath = context.Request.Path.Value;
            var requestMethod = context.Request.Method;
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var queryString = context.Request.QueryString.ToString();

            // Read request body if available (for POST/PUT requests)
            string? requestBody = null;
            if (context.Request.Body.CanSeek)
            {
                context.Request.Body.Position = 0;
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            // Get request headers (excluding sensitive information)
            var requestHeaders = new Dictionary<string, string>();
            foreach (var header in context.Request.Headers)
            {
                // Exclude sensitive headers
                if (!header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase) &&
                    !header.Key.Equals("Cookie", StringComparison.OrdinalIgnoreCase))
                {
                    requestHeaders[header.Key] = string.Join(", ", header.Value);
                }
            }
            var requestHeadersJson = JsonSerializer.Serialize(requestHeaders);

            // Determine HTTP status code and error message
            var (statusCode, errorMessage) = exception switch
            {
                ArgumentNullException => (HttpStatusCode.BadRequest, "Invalid request: Missing required parameter"),
                ArgumentException => (HttpStatusCode.BadRequest, "Invalid request: " + exception.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized access"),
                InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message),
                KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
                _ => (HttpStatusCode.InternalServerError, "An error occurred while processing your request")
            };

            // Log error to database using a service scope
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var errorLoggingService = scope.ServiceProvider.GetRequiredService<IErrorLoggingService>();
                    await errorLoggingService.LogExceptionAsync(
                        exception,
                        source: "GlobalErrorHandlingMiddleware",
                        methodName: context.Request.Method,
                        requestPath: requestPath,
                        requestMethod: requestMethod,
                        userId: userId,
                        userLoginId: userLoginId,
                        ipAddress: ipAddress,
                        requestHeaders: requestHeadersJson,
                        requestBody: requestBody,
                        queryString: queryString
                    );
                }
            }
            catch (Exception logEx)
            {
                // If database logging fails, log to application logger
                _logger.LogError(logEx, "Failed to log error to database. Original exception: {ExceptionMessage}", exception.Message);
            }

            // Log to application logger
            _logger.LogError(exception, "Unhandled exception occurred. Path: {Path}, Method: {Method}", requestPath, requestMethod);

            // Create error response
            var errorResponse = new
            {
                error = new
                {
                    message = errorMessage,
                    statusCode = (int)statusCode,
                    timestamp = DateTime.UtcNow,
                    path = requestPath,
                    method = requestMethod
                }
            };

            response.StatusCode = (int)statusCode;
            var jsonResponse = JsonSerializer.Serialize(errorResponse);
            await response.WriteAsync(jsonResponse);
        }
    }
}
