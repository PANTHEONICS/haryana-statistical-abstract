using HaryanaStatAbstract.API.Models;
using HaryanaStatAbstract.API.Models.Dtos;

namespace HaryanaStatAbstract.API.Services
{
    /// <summary>
    /// Interface for User Management Service
    /// </summary>
    public interface IUserManagementService
    {
        Task<UserManagementLoginResponseDto?> LoginAsync(UserManagementLoginDto loginDto);
        Task<UserManagementUserDto?> CreateUserAsync(CreateUserDto createUserDto);
        Task<List<UserManagementUserDto>> GetAllUsersAsync();
        Task<List<MstRole>> GetAllRolesAsync();
        Task<List<MstDepartment>> GetAllDepartmentsAsync();
        Task<List<MstSecurityQuestion>> GetAllSecurityQuestionsAsync();
        Task<UserManagementUserDto?> GetCurrentUserAsync(int userId);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
    }
}
