using HaryanaStatAbstract.API.Models.Dtos;
using HaryanaStatAbstract.API.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace HaryanaStatAbstract.API.Extensions
{
    /// <summary>
    /// Extension methods for error logging
    /// Makes it easy to log errors from anywhere in the application
    /// </summary>
    public static class ErrorLoggingExtensions
    {
        /// <summary>
        /// Log an exception with HTTP context information
        /// </summary>
        public static async Task<long> LogExceptionWithContextAsync(
            this IErrorLoggingService errorLoggingService,
            Exception exception,
            HttpContext? httpContext,
            string? source = null,
            string? methodName = null,
            object? additionalData = null,
            CancellationToken cancellationToken = default)
        {
            if (httpContext == null)
            {
                return await errorLoggingService.LogExceptionAsync(
                    exception, source, methodName, null, null, null, null, null, null, null, null, additionalData, cancellationToken);
            }

            // Extract user information
            int? userId = null;
            string? userLoginId = null;
            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var parsedUserId))
                {
                    userId = parsedUserId;
                }

                var loginIdClaim = httpContext.User.FindFirst("LoginID");
                userLoginId = loginIdClaim?.Value;
            }

            // Extract request information
            var requestPath = httpContext.Request.Path.Value;
            var requestMethod = httpContext.Request.Method;
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            var queryString = httpContext.Request.QueryString.ToString();

            // Read request body if available
            string? requestBody = null;
            if (httpContext.Request.Body.CanSeek && httpContext.Request.ContentLength > 0)
            {
                try
                {
                    httpContext.Request.Body.Position = 0;
                    using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, leaveOpen: true);
                    requestBody = await reader.ReadToEndAsync();
                    httpContext.Request.Body.Position = 0;
                }
                catch
                {
                    // Ignore if we can't read the body
                }
            }

            // Get request headers (excluding sensitive information)
            var requestHeaders = new Dictionary<string, string>();
            foreach (var header in httpContext.Request.Headers)
            {
                if (!header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase) &&
                    !header.Key.Equals("Cookie", StringComparison.OrdinalIgnoreCase))
                {
                    requestHeaders[header.Key] = string.Join(", ", header.Value);
                }
            }
            var requestHeadersJson = JsonSerializer.Serialize(requestHeaders);

            return await errorLoggingService.LogExceptionAsync(
                exception,
                source: source,
                methodName: methodName ?? httpContext.Request.Method,
                requestPath: requestPath,
                requestMethod: requestMethod,
                userId: userId,
                userLoginId: userLoginId,
                ipAddress: ipAddress,
                requestHeaders: requestHeadersJson,
                requestBody: requestBody,
                queryString: queryString,
                additionalData: additionalData,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Log an error message with HTTP context information
        /// </summary>
        public static async Task<long> LogErrorWithContextAsync(
            this IErrorLoggingService errorLoggingService,
            string errorMessage,
            string errorLevel,
            HttpContext? httpContext,
            string? source = null,
            string? methodName = null,
            object? additionalData = null,
            CancellationToken cancellationToken = default)
        {
            var errorLogDto = new CreateErrorLogDto
            {
                ErrorLevel = errorLevel,
                ErrorMessage = errorMessage,
                Source = source,
                MethodName = methodName
            };

            if (httpContext != null)
            {
                // Extract user information
                if (httpContext.User?.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
                    {
                        errorLogDto.UserID = userId;
                    }

                    var loginIdClaim = httpContext.User.FindFirst("LoginID");
                    errorLogDto.UserLoginID = loginIdClaim?.Value;
                }

                errorLogDto.RequestPath = httpContext.Request.Path.Value;
                errorLogDto.RequestMethod = httpContext.Request.Method;
                errorLogDto.IPAddress = httpContext.Connection.RemoteIpAddress?.ToString();
                errorLogDto.QueryString = httpContext.Request.QueryString.ToString();
            }

            // Serialize additional data if provided
            if (additionalData != null)
            {
                try
                {
                    errorLogDto.AdditionalData = JsonSerializer.Serialize(additionalData);
                }
                catch
                {
                    errorLogDto.AdditionalData = additionalData.ToString();
                }
            }

            return await errorLoggingService.LogErrorAsync(errorLogDto, cancellationToken);
        }
    }
}
