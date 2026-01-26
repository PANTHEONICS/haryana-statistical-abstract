using HaryanaStatAbstract.API.Models.Education.Dtos;
using HaryanaStatAbstract.API.Services.Education;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace HaryanaStatAbstract.API.Controllers.Education
{
    /// <summary>
    /// Controller for managing Table 6.1 Institutions data
    /// Department: Education
    /// </summary>
    [ApiController]
    [Route("api/v1/Education/Table6_1/Institutions")]
    [Produces("application/json")]
    public class Table6_1InstitutionsController : ControllerBase
    {
        private readonly ITable6_1InstitutionsService _service;
        private readonly ILogger<Table6_1InstitutionsController> _logger;

        public Table6_1InstitutionsController(
            ITable6_1InstitutionsService service,
            ILogger<Table6_1InstitutionsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Get client IP address, handling IPv6 loopback and forwarded headers
        /// </summary>
        private string? GetClientIpAddress()
        {
            // Check for forwarded IP (if behind proxy/load balancer)
            var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                var ip = forwardedFor.Split(',')[0].Trim();
                if (IPAddress.TryParse(ip, out var parsedIp))
                {
                    return NormalizeIpAddress(parsedIp);
                }
            }

            // Check for real IP header
            var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp) && IPAddress.TryParse(realIp, out var parsedRealIp))
            {
                return NormalizeIpAddress(parsedRealIp);
            }

            // Get from connection
            var remoteIp = HttpContext.Connection.RemoteIpAddress;
            if (remoteIp != null)
            {
                return NormalizeIpAddress(remoteIp);
            }

            return null;
        }

        /// <summary>
        /// Normalize IP address: convert IPv6 loopback to IPv4, handle IPv6-mapped IPv4
        /// </summary>
        private string NormalizeIpAddress(IPAddress ipAddress)
        {
            // Handle IPv6 loopback (::1) -> convert to 127.0.0.1
            if (ipAddress.Equals(IPAddress.IPv6Loopback))
            {
                return "127.0.0.1";
            }

            // Handle IPv6-mapped IPv4 addresses (::ffff:127.0.0.1)
            if (ipAddress.IsIPv4MappedToIPv6)
            {
                return ipAddress.MapToIPv4().ToString();
            }

            // If it's an IPv4 address, return as is
            if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ipAddress.ToString();
            }

            // For other IPv6 addresses, return as string
            return ipAddress.ToString();
        }

        /// <summary>
        /// Get all institution records
        /// </summary>
        /// <returns>List of all institution records</returns>
        /// <response code="200">Returns the list of institution records</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Table6_1InstitutionsDto>>> GetAll()
        {
            var records = await _service.GetAllAsync();
            return Ok(records);
        }

        /// <summary>
        /// Get institution record by ID
        /// </summary>
        /// <param name="id">Institution ID (primary key)</param>
        /// <returns>Institution record for the specified ID</returns>
        /// <response code="200">Returns the institution record</response>
        /// <response code="404">Record not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Table6_1InstitutionsDto>> GetById(int id)
        {
            var record = await _service.GetByIdAsync(id);
            
            if (record == null)
            {
                return NotFound(new { message = $"Institution record with ID {id} not found" });
            }

            return Ok(record);
        }

        /// <summary>
        /// Get institution record by institution type
        /// </summary>
        /// <param name="institutionType">Institution type name</param>
        /// <returns>Institution record for the specified type</returns>
        /// <response code="200">Returns the institution record</response>
        /// <response code="404">Record not found</response>
        [HttpGet("type/{institutionType}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Table6_1InstitutionsDto>> GetByType(string institutionType)
        {
            var record = await _service.GetByInstitutionTypeAsync(institutionType);
            
            if (record == null)
            {
                return NotFound(new { message = $"Institution record for type '{institutionType}' not found" });
            }

            return Ok(record);
        }

        /// <summary>
        /// Create a new institution record
        /// </summary>
        /// <param name="createDto">Institution data to create</param>
        /// <returns>Created institution record</returns>
        /// <response code="201">Returns the newly created record</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="409">Record already exists</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<Table6_1InstitutionsDto>> Create([FromBody] CreateTable6_1InstitutionsDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Get user ID and IP address
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                int? userId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var parsedUserId) ? parsedUserId : null;
                var ipAddress = GetClientIpAddress();

                var record = await _service.CreateAsync(createDto, userId, ipAddress);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = record.InstitutionID },
                    record);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing institution record
        /// </summary>
        /// <param name="id">Institution ID (primary key) to update</param>
        /// <param name="updateDto">Updated institution data</param>
        /// <returns>Updated institution record</returns>
        /// <response code="200">Returns the updated record</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">Record not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Table6_1InstitutionsDto>> Update(
            int id,
            [FromBody] UpdateTable6_1InstitutionsDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Get user ID and IP address
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                int? userId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var parsedUserId) ? parsedUserId : null;
                var ipAddress = GetClientIpAddress();

                var record = await _service.UpdateAsync(id, updateDto, userId, ipAddress);
                
                if (record == null)
                {
                    return NotFound(new { message = $"Institution record with ID {id} not found" });
                }

                return Ok(record);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete an institution record
        /// </summary>
        /// <param name="id">Institution ID (primary key) to delete</param>
        /// <returns>No content</returns>
        /// <response code="204">Record deleted successfully</response>
        /// <response code="404">Record not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            
            if (!deleted)
            {
                return NotFound(new { message = $"Institution record with ID {id} not found" });
            }

            return NoContent();
        }
    }
}
