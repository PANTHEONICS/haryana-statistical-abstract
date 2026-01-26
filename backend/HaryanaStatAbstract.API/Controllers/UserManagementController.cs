using HaryanaStatAbstract.API.Data;
using HaryanaStatAbstract.API.Models.Dtos;
using HaryanaStatAbstract.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BCrypt.Net;

namespace HaryanaStatAbstract.API.Controllers
{
    /// <summary>
    /// User Management Controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(
            IUserManagementService userManagementService,
            ILogger<UserManagementController> logger)
        {
            _userManagementService = userManagementService;
            _logger = logger;
        }

        /// <summary>
        /// Login with LoginID and Password
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserManagementLoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] UserManagementLoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userManagementService.LoginAsync(loginDto);
            if (result == null)
            {
                return Unauthorized(new { message = "Invalid LoginID or password." });
            }

            return Ok(result);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        [HttpPost("users")]
        [Authorize]
        [ProducesResponseType(typeof(UserManagementUserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _userManagementService.CreateUserAsync(createUserDto);
                if (result == null)
                {
                    return BadRequest(new { message = "Failed to create user." });
                }

                return CreatedAtAction(nameof(GetUser), new { id = result.UserID }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, new { message = "An error occurred while creating the user." });
            }
        }

        /// <summary>
        /// Get all users
        /// </summary>
        [HttpGet("users")]
        [Authorize]
        [ProducesResponseType(typeof(List<UserManagementUserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManagementService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("users/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(UserManagementUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userManagementService.GetCurrentUserAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }

        /// <summary>
        /// Get current user information
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserManagementUserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }

            var user = await _userManagementService.GetCurrentUserAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }

        /// <summary>
        /// Change password for current user
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var result = await _userManagementService.ChangePasswordAsync(userId, changePasswordDto);
            if (!result)
            {
                return BadRequest(new { message = "Failed to change password. Please verify your current password." });
            }

            return Ok(new { message = "Password changed successfully." });
        }

        /// <summary>
        /// Get all roles (Master Data)
        /// </summary>
        [HttpGet("roles")]
        [Authorize]
        [ProducesResponseType(typeof(List<Models.MstRole>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _userManagementService.GetAllRolesAsync();
            return Ok(roles);
        }

        /// <summary>
        /// Get all departments (Master Data)
        /// </summary>
        [HttpGet("departments")]
        [Authorize]
        [ProducesResponseType(typeof(List<Models.MstDepartment>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllDepartments()
        {
            var departments = await _userManagementService.GetAllDepartmentsAsync();
            return Ok(departments);
        }

        /// <summary>
        /// Get all security questions (Master Data)
        /// </summary>
        [HttpGet("security-questions")]
        [Authorize]
        [ProducesResponseType(typeof(List<Models.MstSecurityQuestion>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllSecurityQuestions()
        {
            var questions = await _userManagementService.GetAllSecurityQuestionsAsync();
            return Ok(questions);
        }

        /// <summary>
        /// Fix admin password (Development only - Should be secured or removed in production)
        /// WARNING: This endpoint allows anonymous access. Secure with [Authorize(Roles = "SystemAdmin")] in production.
        /// </summary>
        [HttpPost("fix-admin-password")]
        [AllowAnonymous] // TODO: Remove [AllowAnonymous] and add [Authorize(Roles = "SystemAdmin")] in production
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> FixAdminPassword()
        {
            try
            {
                using var context = HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
                var adminUser = await context.MasterUsers.FirstOrDefaultAsync(u => u.LoginID == "admin");
                
                if (adminUser == null)
                {
                    return NotFound(new { message = "Admin user not found." });
                }

                // Generate proper BCrypt hash for "Admin@123"
                var passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
                adminUser.UserPassword = passwordHash;
                
                await context.SaveChangesAsync();
                
                return Ok(new { 
                    message = "Admin password fixed successfully.", 
                    loginID = "admin",
                    password = "Admin@123",
                    hashGenerated = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fixing admin password");
                return StatusCode(500, new { message = "Error fixing admin password." });
            }
        }

        /// <summary>
        /// Fix all test user passwords (Development only - Should be secured or removed in production)
        /// WARNING: This endpoint allows anonymous access. Secure with [Authorize(Roles = "SystemAdmin")] in production.
        /// </summary>
        [HttpPost("fix-all-test-passwords")]
        [AllowAnonymous] // TODO: Remove [AllowAnonymous] and add [Authorize(Roles = "SystemAdmin")] in production
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> FixAllTestPasswords()
        {
            try
            {
                using var context = HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
                var testUsers = await context.MasterUsers
                    .Where(u => u.LoginID == "admin" || 
                               u.LoginID == "desa_head" || 
                               u.LoginID == "hfw_maker" || 
                               u.LoginID == "hfw_check" || 
                               u.LoginID == "ap_maker" ||
                               u.LoginID == "edu_maker" ||
                               u.LoginID == "edu_check")
                    .ToListAsync();

                if (!testUsers.Any())
                {
                    return NotFound(new { message = "Test users not found." });
                }

                // Generate proper BCrypt hash for "Admin@123"
                var passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
                
                var fixedUsers = new List<string>();
                foreach (var user in testUsers)
                {
                    user.UserPassword = passwordHash;
                    fixedUsers.Add(user.LoginID);
                }
                
                await context.SaveChangesAsync();
                
                return Ok(new { 
                    message = "All test user passwords fixed successfully.", 
                    password = "Admin@123",
                    fixedUsers = fixedUsers,
                    count = fixedUsers.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fixing test user passwords");
                return StatusCode(500, new { message = "Error fixing test user passwords." });
            }
        }
    }
}
