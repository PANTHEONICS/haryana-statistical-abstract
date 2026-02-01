using HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence.Dtos;
using HaryanaStatAbstract.API.Services.SocialSecurityAndSocialDefence;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace HaryanaStatAbstract.API.Controllers.SocialSecurityAndSocialDefence
{
    [ApiController]
    [Route("api/v1/SSD/Table7_6/NoOfPrisonersClasswise")]
    [Produces("application/json")]
    public class Table7_6NoOfPrisonersClasswiseController : ControllerBase
    {
        private readonly ITable7_6NoOfPrisonersClasswiseService _service;
        private readonly ILogger<Table7_6NoOfPrisonersClasswiseController> _logger;

        public Table7_6NoOfPrisonersClasswiseController(ITable7_6NoOfPrisonersClasswiseService service, ILogger<Table7_6NoOfPrisonersClasswiseController> logger)
        {
            _service = service;
            _logger = logger;
        }

        private string? GetClientIpAddress()
        {
            var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor)) { var ip = forwardedFor.Split(',')[0].Trim(); if (IPAddress.TryParse(ip, out var p)) return p.ToString(); }
            return HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Table7_6NoOfPrisonersClasswiseDto>>> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Table7_6NoOfPrisonersClasswiseDto>> GetById(int id)
        {
            var r = await _service.GetByIdAsync(id);
            return r == null ? NotFound(new { message = $"Record with ID {id} not found" }) : Ok(r);
        }

        [HttpGet("year/{year}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Table7_6NoOfPrisonersClasswiseDto>> GetByYear(string year)
        {
            var r = await _service.GetByYearAsync(year);
            return r == null ? NotFound(new { message = $"Record for year {year} not found" }) : Ok(r);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<Table7_6NoOfPrisonersClasswiseDto>> Create([FromBody] CreateTable7_6NoOfPrisonersClasswiseDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier) != null && int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value, out var uid) ? uid : (int?)null;
                var r = await _service.CreateAsync(dto, userId, GetClientIpAddress());
                return CreatedAtAction(nameof(GetById), new { id = r.Id }, r);
            }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<Table7_6NoOfPrisonersClasswiseDto>> Update(int id, [FromBody] UpdateTable7_6NoOfPrisonersClasswiseDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier) != null && int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value, out var uid) ? uid : (int?)null;
                var r = await _service.UpdateAsync(id, dto, userId, GetClientIpAddress());
                return r == null ? NotFound(new { message = $"Record with ID {id} not found" }) : Ok(r);
            }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            return ok ? NoContent() : NotFound(new { message = $"Record with ID {id} not found" });
        }
    }
}
