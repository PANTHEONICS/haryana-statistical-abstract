using HaryanaStatAbstract.API.Models.Dtos;
using HaryanaStatAbstract.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HaryanaStatAbstract.API.Controllers
{
    /// <summary>
    /// Error Log Management Controller
    /// Provides endpoints to view and manage error logs
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class ErrorLogController : ControllerBase
    {
        private readonly IErrorLoggingService _errorLoggingService;
        private readonly ILogger<ErrorLogController> _logger;

        public ErrorLogController(
            IErrorLoggingService errorLoggingService,
            ILogger<ErrorLogController> logger)
        {
            _errorLoggingService = errorLoggingService;
            _logger = logger;
        }

        /// <summary>
        /// Get error logs with filtering and pagination (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "System Admin")]
        [ProducesResponseType(typeof(ErrorLogPagedResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetErrorLogs(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? errorLevel,
            [FromQuery] bool? isResolved,
            [FromQuery] int? userId,
            [FromQuery] string? source,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var query = new ErrorLogQueryDto
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    ErrorLevel = errorLevel,
                    IsResolved = isResolved,
                    UserID = userId,
                    Source = source,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _errorLoggingService.GetErrorLogsAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving error logs");
                return StatusCode(500, new { message = "An error occurred while retrieving error logs." });
            }
        }

        /// <summary>
        /// Get a specific error log by ID (Admin only)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "System Admin")]
        [ProducesResponseType(typeof(ErrorLogDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetErrorLogById(long id)
        {
            try
            {
                var errorLog = await _errorLoggingService.GetErrorLogByIdAsync(id);
                
                if (errorLog == null)
                {
                    return NotFound(new { message = $"Error log with ID {id} not found." });
                }

                return Ok(errorLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving error log {ErrorLogId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the error log." });
            }
        }

        /// <summary>
        /// Mark an error as resolved (Admin only)
        /// </summary>
        [HttpPost("{id}/resolve")]
        [Authorize(Roles = "System Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkErrorResolved(long id, [FromBody] ResolveErrorLogDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                {
                    return Unauthorized();
                }

                var success = await _errorLoggingService.MarkErrorResolvedAsync(id, userId, dto.ResolutionNotes);
                
                if (!success)
                {
                    return NotFound(new { message = $"Error log with ID {id} not found." });
                }

                return Ok(new { message = "Error log marked as resolved successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking error log {ErrorLogId} as resolved", id);
                return StatusCode(500, new { message = "An error occurred while marking the error as resolved." });
            }
        }

        /// <summary>
        /// Get error statistics (Admin only)
        /// </summary>
        [HttpGet("statistics")]
        [Authorize(Roles = "System Admin")]
        [ProducesResponseType(typeof(ErrorStatisticsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetErrorStatistics(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var statistics = await _errorLoggingService.GetErrorStatisticsAsync(startDate, endDate);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving error statistics");
                return StatusCode(500, new { message = "An error occurred while retrieving error statistics." });
            }
        }
    }
}
