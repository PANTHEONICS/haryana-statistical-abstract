using HaryanaStatAbstract.API.Models.Dtos;

namespace HaryanaStatAbstract.API.Services
{
    /// <summary>
    /// Error Logging Service Interface
    /// Provides methods to log errors and exceptions to the database
    /// </summary>
    public interface IErrorLoggingService
    {
        /// <summary>
        /// Log an error to the database
        /// </summary>
        Task<long> LogErrorAsync(CreateErrorLogDto errorLog, CancellationToken cancellationToken = default);

        /// <summary>
        /// Log an exception to the database
        /// </summary>
        Task<long> LogExceptionAsync(Exception exception, string? source = null, string? methodName = null, 
            string? requestPath = null, string? requestMethod = null, int? userId = null, 
            string? userLoginId = null, string? ipAddress = null, string? requestHeaders = null,
            string? requestBody = null, string? queryString = null, object? additionalData = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get error logs with filtering and pagination
        /// </summary>
        Task<ErrorLogPagedResponse> GetErrorLogsAsync(ErrorLogQueryDto query, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a specific error log by ID
        /// </summary>
        Task<ErrorLogDto?> GetErrorLogByIdAsync(long errorLogId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Mark an error as resolved
        /// </summary>
        Task<bool> MarkErrorResolvedAsync(long errorLogId, int resolvedByUserId, string? resolutionNotes = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get error statistics
        /// </summary>
        Task<ErrorStatisticsDto> GetErrorStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Error Statistics DTO
    /// </summary>
    public class ErrorStatisticsDto
    {
        public int TotalErrors { get; set; }
        public int UnresolvedErrors { get; set; }
        public int ResolvedErrors { get; set; }
        public int CriticalErrors { get; set; }
        public Dictionary<string, int> ErrorsBySource { get; set; } = new();
        public Dictionary<string, int> ErrorsByLevel { get; set; } = new();
    }
}
