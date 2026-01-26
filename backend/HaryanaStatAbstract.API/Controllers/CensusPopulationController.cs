using HaryanaStatAbstract.API.Models.Dtos;
using HaryanaStatAbstract.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HaryanaStatAbstract.API.Controllers
{
    /// <summary>
    /// Controller for managing Census Population data
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class CensusPopulationController : ControllerBase
    {
        private readonly ICensusPopulationService _service;
        private readonly ILogger<CensusPopulationController> _logger;

        public CensusPopulationController(
            ICensusPopulationService service,
            ILogger<CensusPopulationController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Get all census population records
        /// </summary>
        /// <returns>List of all census population records</returns>
        /// <response code="200">Returns the list of census population records</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CensusPopulationDto>>> GetAll()
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
        public async Task<ActionResult<CensusPopulationDto>> GetByYear(int year)
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
        public async Task<ActionResult<IEnumerable<CensusPopulationDto>>> GetByRange(
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
        public async Task<ActionResult<CensusPopulationDto>> Create([FromBody] CreateCensusPopulationDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var record = await _service.CreateAsync(createDto);
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
        public async Task<ActionResult<CensusPopulationDto>> Update(
            int id,
            [FromBody] UpdateCensusPopulationDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var record = await _service.UpdateAsync(id, updateDto);
                
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