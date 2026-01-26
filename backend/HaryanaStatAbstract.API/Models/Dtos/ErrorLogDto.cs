namespace HaryanaStatAbstract.API.Models.Dtos
{
    /// <summary>
    /// Error Log DTO for API responses
    /// </summary>
    public class ErrorLogDto
    {
        public long ErrorLogID { get; set; }
        public string ErrorLevel { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string? ExceptionType { get; set; }
        public string? StackTrace { get; set; }
        public string? InnerException { get; set; }
        public string? Source { get; set; }
        public string? MethodName { get; set; }
        public string? RequestPath { get; set; }
        public string? RequestMethod { get; set; }
        public int? UserID { get; set; }
        public string? UserLoginID { get; set; }
        public string? IPAddress { get; set; }
        public string? RequestHeaders { get; set; }
        public string? RequestBody { get; set; }
        public string? QueryString { get; set; }
        public string? AdditionalData { get; set; }
        public bool IsResolved { get; set; }
        public int? ResolvedBy { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string? ResolutionNotes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO for creating error log entries
    /// </summary>
    public class CreateErrorLogDto
    {
        public string ErrorLevel { get; set; } = "Error";
        public string ErrorMessage { get; set; } = string.Empty;
        public string? ExceptionType { get; set; }
        public string? StackTrace { get; set; }
        public string? InnerException { get; set; }
        public string? Source { get; set; }
        public string? MethodName { get; set; }
        public string? RequestPath { get; set; }
        public string? RequestMethod { get; set; }
        public int? UserID { get; set; }
        public string? UserLoginID { get; set; }
        public string? IPAddress { get; set; }
        public string? RequestHeaders { get; set; }
        public string? RequestBody { get; set; }
        public string? QueryString { get; set; }
        public string? AdditionalData { get; set; }
    }

    /// <summary>
    /// DTO for marking error as resolved
    /// </summary>
    public class ResolveErrorLogDto
    {
        public string? ResolutionNotes { get; set; }
    }

    /// <summary>
    /// DTO for error log query parameters
    /// </summary>
    public class ErrorLogQueryDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ErrorLevel { get; set; }
        public bool? IsResolved { get; set; }
        public int? UserID { get; set; }
        public string? Source { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    /// <summary>
    /// Paginated response for error logs
    /// </summary>
    public class ErrorLogPagedResponse
    {
        public List<ErrorLogDto> Data { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
