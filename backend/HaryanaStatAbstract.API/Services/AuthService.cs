using HaryanaStatAbstract.API.Configuration;
using HaryanaStatAbstract.API.Data;
using HaryanaStatAbstract.API.Models;
using HaryanaStatAbstract.API.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace HaryanaStatAbstract.API.Services
{
    /// <summary>
    /// Authentication service implementation (Legacy - Uses MasterUser table)
    /// Note: This service is kept for backward compatibility with frontend api.js
    /// Consider migrating to UserManagementService which uses MasterUser natively
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            ApplicationDbContext context,
            IOptions<JwtSettings> jwtSettings,
            ILogger<AuthService> logger)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                // Check if LoginID already exists
                if (await _context.MasterUsers.AnyAsync(u => u.LoginID == registerDto.Username))
                {
                    _logger.LogWarning("Registration failed: LoginID {Username} already exists", registerDto.Username);
                    return null;
                }

                // Check if email already exists
                if (!string.IsNullOrEmpty(registerDto.Email) && 
                    await _context.MasterUsers.AnyAsync(u => u.UserEmailID == registerDto.Email))
                {
                    _logger.LogWarning("Registration failed: Email {Email} already exists", registerDto.Email);
                    return null;
                }

                // Hash password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

                // Get default "Department Maker" role (or first available role)
                var defaultRole = await _context.MstRoles.FirstOrDefaultAsync(r => r.RoleName == "Department Maker");
                if (defaultRole == null)
                {
                    defaultRole = await _context.MstRoles.FirstOrDefaultAsync();
                }

                if (defaultRole == null)
                {
                    _logger.LogError("No roles found in database. Cannot register user.");
                    return null;
                }

                // Create user in Master_User table
                var user = new MasterUser
                {
                    LoginID = registerDto.Username,
                    UserEmailID = registerDto.Email,
                    UserPassword = passwordHash,
                    FullName = $"{registerDto.FirstName} {registerDto.LastName}".Trim(),
                    UserMobileNo = "0000000000", // Default mobile - should be updated later
                    RoleID = defaultRole.RoleID,
                    IsActive = true
                };

                _context.MasterUsers.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {LoginID} registered successfully", user.LoginID);

                // Generate tokens
                return await GenerateAuthResponseAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return null;
            }
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            try
            {
                // Find user by LoginID or email (now using Master_User table)
                var user = await _context.MasterUsers
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => 
                        u.LoginID == loginDto.UsernameOrEmail || 
                        u.UserEmailID == loginDto.UsernameOrEmail);

                if (user == null || !user.IsActive)
                {
                    _logger.LogWarning("Login failed: User {UsernameOrEmail} not found or inactive", loginDto.UsernameOrEmail);
                    return null;
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.UserPassword))
                {
                    _logger.LogWarning("Login failed: Invalid password for user {LoginID}", user.LoginID);
                    return null;
                }

                // Update last login using direct SQL to avoid OUTPUT clause conflict with triggers
                var now = DateTime.UtcNow;
                var sessionId = Guid.NewGuid().ToString();
                var lastLogin = user.CurrentLoginDateTime; // Save current login as last login

                // Use direct SQL to avoid OUTPUT clause conflict with triggers
                // Handle NULL properly for SQL Server
                if (lastLogin.HasValue)
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "UPDATE Master_User SET LastLoginDateTime = {0}, CurrentLoginDateTime = {1}, LoggedInSessionID = {2} WHERE UserID = {3}",
                        lastLogin.Value, now, sessionId, user.UserID);
                }
                else
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "UPDATE Master_User SET LastLoginDateTime = NULL, CurrentLoginDateTime = {0}, LoggedInSessionID = {1} WHERE UserID = {2}",
                        now, sessionId, user.UserID);
                }

                _logger.LogInformation("User {LoginID} logged in successfully", user.LoginID);

                // Generate tokens
                return await GenerateAuthResponseAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                return null;
            }
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            // RefreshToken table has been removed - return null
            // Consider implementing refresh token logic using UserManagementService if needed
            _logger.LogWarning("Refresh token functionality is not available. Old RefreshToken table has been removed.");
            return null;
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            // RefreshToken table has been removed - return false
            _logger.LogWarning("Refresh token revocation is not available. Old RefreshToken table has been removed.");
            return false;
        }

        public async Task<bool> LogoutAsync(int userId)
        {
            try
            {
                // Clear session ID in Master_User table
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE Master_User SET LoggedInSessionID = NULL WHERE UserID = {0}",
                    userId);

                _logger.LogInformation("User {UserId} logged out successfully", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user logout");
                return false;
            }
        }

        private async Task<AuthResponseDto> GenerateAuthResponseAsync(MasterUser user)
        {
            var accessToken = GenerateAccessToken(user);

            // RefreshToken functionality removed - return empty string
            // Access tokens are JWT tokens that can be validated without database lookup

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = string.Empty, // Refresh token functionality removed
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                User = new UserDto
                {
                    Id = user.UserID,
                    Username = user.LoginID,
                    Email = user.UserEmailID ?? string.Empty,
                    FirstName = user.FullName.Split(' ')[0],
                    LastName = user.FullName.Split(' ').Length > 1 ? string.Join(" ", user.FullName.Split(' ').Skip(1)) : string.Empty,
                    FullName = user.FullName,
                    IsActive = user.IsActive,
                    EmailConfirmed = !string.IsNullOrEmpty(user.UserEmailID),
                    LastLoginAt = user.LastLoginDateTime,
                    CreatedAt = null, // MasterUser doesn't have CreatedAt - could add if needed
                    Roles = new List<string> { user.Role?.RoleName ?? "User" }
                }
            };
        }

        private string GenerateAccessToken(MasterUser user)
        {
            var roleName = user.Role?.RoleName ?? "User";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.LoginID),
                new Claim(ClaimTypes.Email, user.UserEmailID ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, roleName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
