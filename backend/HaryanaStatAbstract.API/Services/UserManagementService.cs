using HaryanaStatAbstract.API.Configuration;
using HaryanaStatAbstract.API.Data;
using HaryanaStatAbstract.API.Models;
using HaryanaStatAbstract.API.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace HaryanaStatAbstract.API.Services
{
    /// <summary>
    /// User Management Service Implementation
    /// </summary>
    public class UserManagementService : IUserManagementService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(
            ApplicationDbContext context,
            IOptions<JwtSettings> jwtSettings,
            ILogger<UserManagementService> logger)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public async Task<UserManagementLoginResponseDto?> LoginAsync(UserManagementLoginDto loginDto)
        {
            try
            {
                // Find user by LoginID
                var user = await _context.MasterUsers
                    .Include(u => u.Role)
                    .Include(u => u.Department)
                    .FirstOrDefaultAsync(u => u.LoginID == loginDto.LoginID && u.IsActive);

                if (user == null)
                {
                    _logger.LogWarning("Login failed: User {LoginID} not found or inactive", loginDto.LoginID);
                    return null;
                }

                // Verify password using BCrypt
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.UserPassword))
                {
                    _logger.LogWarning("Login failed: Invalid password for user {LoginID}", loginDto.LoginID);
                    return null;
                }

                // Update login tracking - Audit trail
                // Copy CurrentLoginDateTime -> LastLoginDateTime, then set CurrentLoginDateTime -> Now
                var now = DateTime.UtcNow;
                var sessionId = Guid.NewGuid().ToString();
                var lastLogin = user.CurrentLoginDateTime; // Save current login as last login

                // Use direct SQL to avoid OUTPUT clause conflict with triggers
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE Master_User SET LastLoginDateTime = {0}, CurrentLoginDateTime = {1}, LoggedInSessionID = {2} WHERE UserID = {3}",
                    lastLogin, now, sessionId, user.UserID);

                // Refresh user from database to get updated values
                user = await _context.MasterUsers
                    .Include(u => u.Role)
                    .Include(u => u.Department)
                    .FirstOrDefaultAsync(u => u.UserID == user.UserID);

                if (user == null)
                {
                    _logger.LogError("User not found after update");
                    return null;
                }

                _logger.LogInformation("User {LoginID} logged in successfully", user.LoginID);

                // Generate tokens
                var accessToken = GenerateAccessToken(user);
                var refreshToken = await GenerateRefreshTokenAsync(user.UserID);

                // Map to DTO
                var userDto = new UserManagementUserDto
                {
                    UserID = user.UserID,
                    LoginID = user.LoginID,
                    FullName = user.FullName,
                    UserMobileNo = user.UserMobileNo,
                    UserEmailID = user.UserEmailID,
                    RoleName = user.Role.RoleName,
                    DepartmentName = user.Department?.DepartmentName,
                    DepartmentCode = user.Department?.DepartmentCode,
                    CurrentLoginDateTime = user.CurrentLoginDateTime,
                    LastLoginDateTime = user.LastLoginDateTime,
                    IsActive = user.IsActive
                };

                return new UserManagementLoginResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                    User = userDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                return null;
            }
        }

        public async Task<UserManagementUserDto?> CreateUserAsync(CreateUserDto createUserDto)
        {
            try
            {
                // Validate mobile number length
                if (createUserDto.UserMobileNo.Length != 10)
                {
                    throw new ArgumentException("Mobile number must be exactly 10 digits");
                }

                // Check if LoginID already exists
                if (await _context.MasterUsers.AnyAsync(u => u.LoginID == createUserDto.LoginID))
                {
                    throw new InvalidOperationException("LoginID already exists");
                }

                // Check if Role is System Admin - Department should be NULL
                var role = await _context.MstRoles.FindAsync(createUserDto.RoleID);
                if (role?.RoleName == "System Admin" && createUserDto.DepartmentID.HasValue)
                {
                    throw new InvalidOperationException("System Admin cannot have a Department assigned");
                }

                // Check for Department Checker constraint - only one per department
                if (role?.RoleName == "Department Checker" && createUserDto.DepartmentID.HasValue)
                {
                    var existingChecker = await _context.MasterUsers
                        .Where(u => u.RoleID == createUserDto.RoleID
                            && u.DepartmentID == createUserDto.DepartmentID
                            && u.IsActive)
                        .FirstOrDefaultAsync();

                    if (existingChecker != null)
                    {
                        throw new InvalidOperationException(
                            $"A Department Checker already exists for this department. Only one active Department Checker is allowed per department.");
                    }
                }

                // Hash password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);

                // Create user
                var user = new MasterUser
                {
                    LoginID = createUserDto.LoginID,
                    UserPassword = passwordHash,
                    UserMobileNo = createUserDto.UserMobileNo,
                    UserEmailID = createUserDto.UserEmailID,
                    FullName = createUserDto.FullName,
                    RoleID = createUserDto.RoleID,
                    DepartmentID = role?.RoleName == "System Admin" ? null : createUserDto.DepartmentID,
                    SecurityQuestionID = createUserDto.SecurityQuestionID,
                    SecurityQuestionAnswer = createUserDto.SecurityQuestionAnswer,
                    IsActive = true
                };

                _context.MasterUsers.Add(user);
                await _context.SaveChangesAsync();

                // Reload with relationships
                user = await _context.MasterUsers
                    .Include(u => u.Role)
                    .Include(u => u.Department)
                    .FirstOrDefaultAsync(u => u.UserID == user.UserID);

                if (user == null)
                {
                    throw new Exception("Failed to retrieve created user");
                }

                _logger.LogInformation("User {LoginID} created successfully", user.LoginID);

                return new UserManagementUserDto
                {
                    UserID = user.UserID,
                    LoginID = user.LoginID,
                    FullName = user.FullName,
                    UserMobileNo = user.UserMobileNo,
                    UserEmailID = user.UserEmailID,
                    RoleName = user.Role.RoleName,
                    DepartmentName = user.Department?.DepartmentName,
                    DepartmentCode = user.Department?.DepartmentCode,
                    CurrentLoginDateTime = user.CurrentLoginDateTime,
                    LastLoginDateTime = user.LastLoginDateTime,
                    IsActive = user.IsActive
                };
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error during user creation");
                
                // Handle SQL Server unique constraint violation for Department Checker
                if (ex.InnerException?.Message.Contains("IX_Master_User_UniqueCheckerPerDepartment") == true)
                {
                    throw new InvalidOperationException(
                        "A Department Checker already exists for this department. Only one active Department Checker is allowed per department.");
                }
                
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user creation");
                throw;
            }
        }

        public async Task<List<UserManagementUserDto>> GetAllUsersAsync()
        {
            var users = await _context.MasterUsers
                .Include(u => u.Role)
                .Include(u => u.Department)
                .Where(u => u.IsActive)
                .ToListAsync();

            return users.Select(u => new UserManagementUserDto
            {
                UserID = u.UserID,
                LoginID = u.LoginID,
                FullName = u.FullName,
                UserMobileNo = u.UserMobileNo,
                UserEmailID = u.UserEmailID,
                RoleName = u.Role.RoleName,
                DepartmentName = u.Department?.DepartmentName,
                DepartmentCode = u.Department?.DepartmentCode,
                CurrentLoginDateTime = u.CurrentLoginDateTime,
                LastLoginDateTime = u.LastLoginDateTime,
                IsActive = u.IsActive
            }).ToList();
        }

        public async Task<List<MstRole>> GetAllRolesAsync()
        {
            return await _context.MstRoles.ToListAsync();
        }

        public async Task<List<MstDepartment>> GetAllDepartmentsAsync()
        {
            return await _context.MstDepartments.ToListAsync();
        }

        public async Task<List<MstSecurityQuestion>> GetAllSecurityQuestionsAsync()
        {
            return await _context.MstSecurityQuestions.ToListAsync();
        }

        public async Task<UserManagementUserDto?> GetCurrentUserAsync(int userId)
        {
            var user = await _context.MasterUsers
                .Include(u => u.Role)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.UserID == userId && u.IsActive);

            if (user == null)
            {
                return null;
            }

            return new UserManagementUserDto
            {
                UserID = user.UserID,
                LoginID = user.LoginID,
                FullName = user.FullName,
                UserMobileNo = user.UserMobileNo,
                UserEmailID = user.UserEmailID,
                RoleName = user.Role.RoleName,
                DepartmentName = user.Department?.DepartmentName,
                DepartmentCode = user.Department?.DepartmentCode,
                CurrentLoginDateTime = user.CurrentLoginDateTime,
                LastLoginDateTime = user.LastLoginDateTime,
                IsActive = user.IsActive
            };
        }

        private string GenerateAccessToken(MasterUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.LoginID),
                new Claim("FullName", user.FullName),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (!string.IsNullOrEmpty(user.UserEmailID))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.UserEmailID));
            }

            if (user.DepartmentID.HasValue)
            {
                claims.Add(new Claim("DepartmentID", user.DepartmentID.Value.ToString()));
                if (!string.IsNullOrEmpty(user.Department?.DepartmentCode))
                {
                    claims.Add(new Claim("DepartmentCode", user.Department.DepartmentCode));
                }
            }

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

        private async Task<string> GenerateRefreshTokenAsync(int userId)
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var token = Convert.ToBase64String(randomBytes);

            // Store refresh token in database (using existing RefreshToken table or create a new one)
            // For now, we'll just return the token. You can extend this to store it properly.
            return token;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            try
            {
                // Find user by UserID
                var user = await _context.MasterUsers
                    .FirstOrDefaultAsync(u => u.UserID == userId && u.IsActive);

                if (user == null)
                {
                    _logger.LogWarning("Change password failed: User {UserId} not found or inactive", userId);
                    return false;
                }

                // Verify current password using BCrypt
                if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.UserPassword))
                {
                    _logger.LogWarning("Change password failed: Invalid current password for user {UserId}", userId);
                    return false;
                }

                // Validate new password and confirm password match
                if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
                {
                    _logger.LogWarning("Change password failed: New password and confirm password do not match for user {UserId}", userId);
                    return false;
                }

                // Hash new password using BCrypt
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);

                // Update password using direct SQL to avoid OUTPUT clause conflict with triggers
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE Master_User SET UserPassword = {0} WHERE UserID = {1}",
                    hashedPassword, userId);

                _logger.LogInformation("Password changed successfully for user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId);
                return false;
            }
        }
    }
}
