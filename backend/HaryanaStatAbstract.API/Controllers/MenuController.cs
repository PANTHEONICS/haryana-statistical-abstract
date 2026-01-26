using HaryanaStatAbstract.API.Models.Dtos;
using HaryanaStatAbstract.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HaryanaStatAbstract.API.Controllers
{
    /// <summary>
    /// Menu Management Controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly ILogger<MenuController> _logger;

        public MenuController(IMenuService menuService, ILogger<MenuController> logger)
        {
            _menuService = menuService;
            _logger = logger;
        }

        /// <summary>
        /// Get all menus (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "System Admin")]
        [ProducesResponseType(typeof(List<MenuDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllMenus()
        {
            var menus = await _menuService.GetAllMenusAsync();
            return Ok(menus);
        }

        /// <summary>
        /// Get user's accessible menus based on role and department
        /// </summary>
        [HttpGet("user-menus")]
        [ProducesResponseType(typeof(List<MenuDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserMenus()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }

            var menus = await _menuService.GetUserMenusAsync(userId);
            return Ok(menus);
        }

        /// <summary>
        /// Check if user can access a menu
        /// </summary>
        [HttpGet("check-access")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckMenuAccess([FromQuery] string menuPath)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }

            var hasAccess = await _menuService.CheckMenuAccessAsync(userId, menuPath);
            return Ok(new { hasAccess, menuPath });
        }

        /// <summary>
        /// Get department menu mappings (Admin only)
        /// </summary>
        [HttpGet("department-mappings")]
        [Authorize(Roles = "System Admin")]
        [ProducesResponseType(typeof(List<DepartmentMenuMappingDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDepartmentMenuMappings()
        {
            var mappings = await _menuService.GetDepartmentMenuMappingsAsync();
            return Ok(mappings);
        }

        /// <summary>
        /// Get menus assigned to a department
        /// </summary>
        [HttpGet("department/{departmentId}")]
        [Authorize(Roles = "System Admin")]
        [ProducesResponseType(typeof(List<MenuDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDepartmentMenus(int departmentId)
        {
            var menus = await _menuService.GetDepartmentMenusAsync(departmentId);
            return Ok(menus);
        }

        /// <summary>
        /// Assign menus to a department (Admin only)
        /// </summary>
        [HttpPost("assign-to-department")]
        [Authorize(Roles = "System Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AssignMenusToDepartment([FromBody] AssignMenusToDepartmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }

            try
            {
                await _menuService.AssignMenusToDepartmentAsync(dto.DepartmentID, dto.MenuIDs, userId);
                return Ok(new { message = "Menus assigned to department successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning menus to department");
                return StatusCode(500, new { message = "An error occurred while assigning menus." });
            }
        }
    }
}
