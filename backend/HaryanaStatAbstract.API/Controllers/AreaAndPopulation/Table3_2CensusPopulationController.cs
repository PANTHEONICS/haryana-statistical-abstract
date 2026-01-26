using HaryanaStatAbstract.API.Models.AreaAndPopulation.Dtos;
using HaryanaStatAbstract.API.Services.AreaAndPopulation;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace HaryanaStatAbstract.API.Controllers.AreaAndPopulation
{
    /// <summary>
    /// Controller for managing Census Population data (Table 3.2)
    /// Department: Area & Population
    /// </summary>
    [ApiController]
    [Route("api/v1/AP/Table3_2/CensusPopulation")]
    [Produces("application/json")]
    public class Table3_2CensusPopulationController : ControllerBase
    {
        private readonly ITable3_2CensusPopulationService _service;
        private readonly ILogger<Table3_2CensusPopulationController> _logger;

        public Table3_2CensusPopulationController(
            ITable3_2CensusPopulationService service,
            ILogger<Table3_2CensusPopulationController> logger)
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
        /// Get all census population records
        /// </summary>
        /// <returns>List of all census population records</returns>
        /// <response code="200">Returns the list of census population records</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Table3_2CensusPopulationDto>>> GetAll()
        {
            var records = await _service.GetAllAsync();
            return Ok(records);
        }

        /// <summary>
        /// Get census population record by year
        /// </summary>
        /// <param name="year">Census year (e.g., 2011)</param>
        /// <returns>Census population record for the specified year</returns>
        /// <response code="200">Returns the census population record</response>
        /// <response code="404">Record not found</response>
        [HttpGet("{year}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Table3_2CensusPopulationDto>> GetByYear(int year)
        {
            var record = await _service.GetByYearAsync(year);
            
            if (record == null)
            {
                return NotFound(new { message = $"Census record for year {year} not found" });
            }

            return Ok(record);
        }

        /// <summary>
        /// Get census population records by year range
        /// </summary>
        /// <param name="startYear">Start year</param>
        /// <param name="endYear">End year</param>
        /// <returns>List of census population records within the specified range</returns>
        /// <response code="200">Returns the list of census population records</response>
        [HttpGet("range")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Table3_2CensusPopulationDto>>> GetByRange(
            [FromQuery] int startYear,
            [FromQuery] int endYear)
        {
            var records = await _service.GetByYearRangeAsync(startYear, endYear);
            return Ok(records);
        }

        /// <summary>
        /// Create a new census population record
        /// </summary>
        /// <param name="createDto">Census population data to create</param>
        /// <returns>Created census population record</returns>
        /// <response code="201">Returns the newly created record</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="409">Record already exists</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<Table3_2CensusPopulationDto>> Create([FromBody] CreateTable3_2CensusPopulationDto createDto)
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
                    nameof(GetByYear),
                    new { year = record.Year },
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
        /// Update an existing census population record
        /// </summary>
        /// <param name="id">Census ID (primary key) to update</param>
        /// <param name="updateDto">Updated census population data</param>
        /// <returns>Updated census population record</returns>
        /// <response code="200">Returns the updated record</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">Record not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<Table3_2CensusPopulationDto>> Update(
            int id,
            [FromBody] UpdateTable3_2CensusPopulationDto updateDto)
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
                    return NotFound(new { message = $"Census record with ID {id} not found" });
                }

                return Ok(record);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a census population record
        /// </summary>
        /// <param name="id">Census ID (primary key) to delete</param>
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
                return NotFound(new { message = $"Census record with ID {id} not found" });
            }

            return NoContent();
        }
    }
}
