using HaryanaStatAbstract.API.Models.Dtos;

namespace HaryanaStatAbstract.API.Services
{
    /// <summary>
    /// Interface for Menu Management Service
    /// </summary>
    public interface IMenuService
    {
        Task<List<MenuDto>> GetAllMenusAsync();
        Task<List<MenuDto>> GetUserMenusAsync(int userId);
        Task<List<MenuDto>> GetDepartmentMenusAsync(int departmentId);
        Task<bool> CanUserAccessMenuAsync(int userId, string menuPath);
        Task<List<DepartmentMenuMappingDto>> GetDepartmentMenuMappingsAsync();
        Task AssignMenusToDepartmentAsync(int departmentId, List<int> menuIds, int createdBy);
        Task<bool> CheckMenuAccessAsync(int userId, string menuPath);
    }
}
