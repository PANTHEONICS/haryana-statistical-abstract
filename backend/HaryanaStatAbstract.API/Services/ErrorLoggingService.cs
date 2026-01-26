using HaryanaStatAbstract.API.Data;
using HaryanaStatAbstract.API.Models;
using HaryanaStatAbstract.API.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HaryanaStatAbstract.API.Services
{
    /// <summary>
    /// Error Logging Service Implementation
    /// Logs errors and exceptions to the database
    /// </summary>
    public class ErrorLoggingService : IErrorLoggingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ErrorLoggingService> _logger;

        public ErrorLoggingService(ApplicationDbContext context, ILogger<ErrorLoggingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<long> LogErrorAsync(CreateErrorLogDto errorLog, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = new ErrorLog
                {
                    ErrorLevel = errorLog.ErrorLevel,
                    ErrorMessage = errorLog.ErrorMessage,
                    ExceptionType = errorLog.ExceptionType,
                    StackTrace = errorLog.StackTrace,
                    InnerException = errorLog.InnerException,
                    Source = errorLog.Source,
                    MethodName = errorLog.MethodName,
                    RequestPath = errorLog.RequestPath,
                    RequestMethod = errorLog.RequestMethod,
                    UserID = errorLog.UserID,
                    UserLoginID = errorLog.UserLoginID,
                    IPAddress = errorLog.IPAddress,
                    RequestHeaders = errorLog.RequestHeaders,
                    RequestBody = errorLog.RequestBody,
                    QueryString = errorLog.QueryString,
                    AdditionalData = errorLog.AdditionalData,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ErrorLogs.Add(entity);
                await _context.SaveChangesAsync(cancellationToken);

                return entity.ErrorLogID;
            }
            catch (Exception ex)
            {
                // If database logging fails, log to application logger
                _logger.LogError(ex, "Failed to log error to database. Error: {ErrorMessage}", errorLog.ErrorMessage);
                throw;
            }
        }

        public async Task<long> LogExceptionAsync(Exception exception, string? source = null, string? methodName = null,
            string? requestPath = null, string? requestMethod = null, int? userId = null,
            string? userLoginId = null, string? ipAddress = null, string? requestHeaders = null,
            string? requestBody = null, string? queryString = null, object? additionalData = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Determine error level based on exception type
                string errorLevel = exception switch
                {
                    ArgumentNullException => "Warning",
                    ArgumentException => "Warning",
                    UnauthorizedAccessException => "Warning",
                    InvalidOperationException => "Error",
                    _ => "Error"
                };

                // Get inner exception message
                string? innerException = exception.InnerException != null
                    ? $"{exception.InnerException.GetType().Name}: {exception.InnerException.Message}"
                    : null;

                // Serialize additional data if provided
                string? additionalDataJson = null;
                if (additionalData != null)
                {
                    try
                    {
                        additionalDataJson = JsonSerializer.Serialize(additionalData);
                    }
                    catch
                    {
                        additionalDataJson = additionalData.ToString();
                    }
                }

                var errorLogDto = new CreateErrorLogDto
                {
                    ErrorLevel = errorLevel,
                    ErrorMessage = exception.Message,
                    ExceptionType = exception.GetType().FullName,
                    StackTrace = exception.StackTrace,
                    InnerException = innerException,
                    Source = source,
                    MethodName = methodName,
                    RequestPath = requestPath,
                    RequestMethod = requestMethod,
                    UserID = userId,
                    UserLoginID = userLoginId,
                    IPAddress = ipAddress,
                    RequestHeaders = requestHeaders,
                    RequestBody = requestBody,
                    QueryString = queryString,
                    AdditionalData = additionalDataJson
                };

                return await LogErrorAsync(errorLogDto, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log exception. Original exception: {ExceptionMessage}", exception.Message);
                throw;
            }
        }

        public async Task<ErrorLogPagedResponse> GetErrorLogsAsync(ErrorLogQueryDto query, CancellationToken cancellationToken = default)
        {
            var queryable = _context.ErrorLogs.AsQueryable();

            // Apply filters
            if (query.StartDate.HasValue)
            {
                queryable = queryable.Where(e => e.CreatedAt >= query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                queryable = queryable.Where(e => e.CreatedAt <= query.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(query.ErrorLevel))
            {
                queryable = queryable.Where(e => e.ErrorLevel == query.ErrorLevel);
            }

            if (query.IsResolved.HasValue)
            {
                queryable = queryable.Where(e => e.IsResolved == query.IsResolved.Value);
            }

            if (query.UserID.HasValue)
            {
                queryable = queryable.Where(e => e.UserID == query.UserID.Value);
            }

            if (!string.IsNullOrEmpty(query.Source))
            {
                queryable = queryable.Where(e => e.Source != null && e.Source.Contains(query.Source));
            }

            // Get total count
            var totalCount = await queryable.CountAsync(cancellationToken);

            // Apply pagination
            var errorLogs = await queryable
                .OrderByDescending(e => e.CreatedAt)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            var errorLogDtos = errorLogs.Select(MapToDto).ToList();

            return new ErrorLogPagedResponse
            {
                Data = errorLogDtos,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }

        public async Task<ErrorLogDto?> GetErrorLogByIdAsync(long errorLogId, CancellationToken cancellationToken = default)
        {
            var errorLog = await _context.ErrorLogs
                .FirstOrDefaultAsync(e => e.ErrorLogID == errorLogId, cancellationToken);

            return errorLog != null ? MapToDto(errorLog) : null;
        }

        public async Task<bool> MarkErrorResolvedAsync(long errorLogId, int resolvedByUserId, string? resolutionNotes = null, CancellationToken cancellationToken = default)
        {
            var errorLog = await _context.ErrorLogs
                .FirstOrDefaultAsync(e => e.ErrorLogID == errorLogId, cancellationToken);

            if (errorLog == null)
            {
                return false;
            }

            errorLog.IsResolved = true;
            errorLog.ResolvedBy = resolvedByUserId;
            errorLog.ResolvedAt = DateTime.UtcNow;
            errorLog.ResolutionNotes = resolutionNotes;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<ErrorStatisticsDto> GetErrorStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
        {
            var queryable = _context.ErrorLogs.AsQueryable();

            if (startDate.HasValue)
            {
                queryable = queryable.Where(e => e.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                queryable = queryable.Where(e => e.CreatedAt <= endDate.Value);
            }

            var allErrors = await queryable.ToListAsync(cancellationToken);

            var statistics = new ErrorStatisticsDto
            {
                TotalErrors = allErrors.Count,
                UnresolvedErrors = allErrors.Count(e => !e.IsResolved),
                ResolvedErrors = allErrors.Count(e => e.IsResolved),
                CriticalErrors = allErrors.Count(e => e.ErrorLevel == "Critical"),
                ErrorsByLevel = allErrors
                    .GroupBy(e => e.ErrorLevel)
                    .ToDictionary(g => g.Key, g => g.Count()),
                ErrorsBySource = allErrors
                    .Where(e => !string.IsNullOrEmpty(e.Source))
                    .GroupBy(e => e.Source!)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            return statistics;
        }

        private static ErrorLogDto MapToDto(ErrorLog errorLog)
        {
            return new ErrorLogDto
            {
                ErrorLogID = errorLog.ErrorLogID,
                ErrorLevel = errorLog.ErrorLevel,
                ErrorMessage = errorLog.ErrorMessage,
                ExceptionType = errorLog.ExceptionType,
                StackTrace = errorLog.StackTrace,
                InnerException = errorLog.InnerException,
                Source = errorLog.Source,
                MethodName = errorLog.MethodName,
                RequestPath = errorLog.RequestPath,
                RequestMethod = errorLog.RequestMethod,
                UserID = errorLog.UserID,
                UserLoginID = errorLog.UserLoginID,
                IPAddress = errorLog.IPAddress,
                RequestHeaders = errorLog.RequestHeaders,
                RequestBody = errorLog.RequestBody,
                QueryString = errorLog.QueryString,
                AdditionalData = errorLog.AdditionalData,
                IsResolved = errorLog.IsResolved,
                ResolvedBy = errorLog.ResolvedBy,
                ResolvedAt = errorLog.ResolvedAt,
                ResolutionNotes = errorLog.ResolutionNotes,
                CreatedAt = errorLog.CreatedAt
            };
        }
    }
}
