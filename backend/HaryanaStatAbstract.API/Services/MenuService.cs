using HaryanaStatAbstract.API.Data;
using HaryanaStatAbstract.API.Models;
using HaryanaStatAbstract.API.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HaryanaStatAbstract.API.Services
{
    /// <summary>
    /// Menu Management Service Implementation
    /// </summary>
    public class MenuService : IMenuService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MenuService> _logger;

        public MenuService(ApplicationDbContext context, ILogger<MenuService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<MenuDto>> GetAllMenusAsync()
        {
            var menus = await _context.MstMenus
                .Where(m => m.IsActive)
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync();

            return menus.Select(m => new MenuDto
            {
                MenuID = m.MenuID,
                MenuName = m.MenuName,
                MenuPath = m.MenuPath,
                MenuIcon = m.MenuIcon,
                ParentMenuID = m.ParentMenuID,
                DisplayOrder = m.DisplayOrder,
                IsActive = m.IsActive,
                IsAdminOnly = m.IsAdminOnly,
                MenuDescription = m.MenuDescription
            }).ToList();
        }

        public async Task<List<MenuDto>> GetUserMenusAsync(int userId)
        {
            var user = await _context.MasterUsers
                .Include(u => u.Role)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.UserID == userId && u.IsActive);

            if (user == null)
            {
                return new List<MenuDto>();
            }

            // Admin has access to all menus
            if (user.Role.RoleName == "System Admin")
            {
                return await GetAllMenusAsync();
            }

            // DESA Head has access to all non-admin menus except: Data Management, Detail View, Workflow, Board View, Analytics
            if (user.Role.RoleName == "DESA Head")
            {
                // Exclude these menu paths for DESA Head
                var excludedPaths = new[] { "/data", "/detail", "/workflow", "/board", "/analytics" };
                
                var menus = await _context.MstMenus
                    .Where(m => m.IsActive && !m.IsAdminOnly && !excludedPaths.Contains(m.MenuPath))
                    .OrderBy(m => m.DisplayOrder)
                    .ToListAsync();

                return menus.Select(m => new MenuDto
                {
                    MenuID = m.MenuID,
                    MenuName = m.MenuName,
                    MenuPath = m.MenuPath,
                    MenuIcon = m.MenuIcon,
                    ParentMenuID = m.ParentMenuID,
                    DisplayOrder = m.DisplayOrder,
                    IsActive = m.IsActive,
                    IsAdminOnly = m.IsAdminOnly,
                    MenuDescription = m.MenuDescription
                }).ToList();
            }

            // Department users can only see menus assigned to their department
            if (user.DepartmentID.HasValue)
            {
                var departmentMenus = await _context.DepartmentMenuMappings
                    .Include(dm => dm.Menu)
                    .Where(dm => dm.DepartmentID == user.DepartmentID.Value && dm.IsActive && dm.Menu.IsActive)
                    .Select(dm => dm.Menu)
                    .OrderBy(m => m.DisplayOrder)
                    .ToListAsync();

                // Also include Dashboard which should be accessible to all
                var dashboard = await _context.MstMenus
                    .FirstOrDefaultAsync(m => m.MenuPath == "/" && m.IsActive);

                var result = departmentMenus.Select(m => new MenuDto
                {
                    MenuID = m.MenuID,
                    MenuName = m.MenuName,
                    MenuPath = m.MenuPath,
                    MenuIcon = m.MenuIcon,
                    ParentMenuID = m.ParentMenuID,
                    DisplayOrder = m.DisplayOrder,
                    IsActive = m.IsActive,
                    IsAdminOnly = m.IsAdminOnly,
                    MenuDescription = m.MenuDescription
                }).ToList();

                // Add Dashboard if not already included
                if (dashboard != null && !result.Any(m => m.MenuPath == "/"))
                {
                    result.Insert(0, new MenuDto
                    {
                        MenuID = dashboard.MenuID,
                        MenuName = dashboard.MenuName,
                        MenuPath = dashboard.MenuPath,
                        MenuIcon = dashboard.MenuIcon,
                        ParentMenuID = dashboard.ParentMenuID,
                        DisplayOrder = dashboard.DisplayOrder,
                        IsActive = dashboard.IsActive,
                        IsAdminOnly = dashboard.IsAdminOnly,
                        MenuDescription = dashboard.MenuDescription
                    });
                }

                return result.OrderBy(m => m.DisplayOrder).ToList();
            }

            return new List<MenuDto>();
        }

        public async Task<List<MenuDto>> GetDepartmentMenusAsync(int departmentId)
        {
            var menus = await _context.DepartmentMenuMappings
                .Include(dm => dm.Menu)
                .Where(dm => dm.DepartmentID == departmentId && dm.IsActive && dm.Menu.IsActive)
                .Select(dm => dm.Menu)
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync();

            return menus.Select(m => new MenuDto
            {
                MenuID = m.MenuID,
                MenuName = m.MenuName,
                MenuPath = m.MenuPath,
                MenuIcon = m.MenuIcon,
                ParentMenuID = m.ParentMenuID,
                DisplayOrder = m.DisplayOrder,
                IsActive = m.IsActive,
                IsAdminOnly = m.IsAdminOnly,
                MenuDescription = m.MenuDescription
            }).ToList();
        }

        public async Task<bool> CanUserAccessMenuAsync(int userId, string menuPath)
        {
            return await CheckMenuAccessAsync(userId, menuPath);
        }

        public async Task<bool> CheckMenuAccessAsync(int userId, string menuPath)
        {
            var user = await _context.MasterUsers
                .Include(u => u.Role)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.UserID == userId && u.IsActive);

            if (user == null)
            {
                return false;
            }

            // Admin has access to all menus
            if (user.Role.RoleName == "System Admin")
            {
                return true;
            }

            var menu = await _context.MstMenus
                .FirstOrDefaultAsync(m => m.MenuPath == menuPath && m.IsActive);

            if (menu == null)
            {
                return false;
            }

            // Admin-only menus are restricted
            if (menu.IsAdminOnly && user.Role.RoleName != "System Admin")
            {
                return false;
            }

            // DESA Head has access to all non-admin menus
            if (user.Role.RoleName == "DESA Head")
            {
                return !menu.IsAdminOnly;
            }

            // Dashboard is accessible to all
            if (menuPath == "/")
            {
                return true;
            }

            // Department users can only access menus assigned to their department
            if (user.DepartmentID.HasValue)
            {
                var hasAccess = await _context.DepartmentMenuMappings
                    .AnyAsync(dm => dm.DepartmentID == user.DepartmentID.Value 
                        && dm.MenuID == menu.MenuID 
                        && dm.IsActive 
                        && dm.Menu.IsActive);

                return hasAccess;
            }

            return false;
        }

        public async Task<List<DepartmentMenuMappingDto>> GetDepartmentMenuMappingsAsync()
        {
            var mappings = await _context.DepartmentMenuMappings
                .Include(dm => dm.Department)
                .Include(dm => dm.Menu)
                .Where(dm => dm.IsActive)
                .ToListAsync();

            return mappings.Select(dm => new DepartmentMenuMappingDto
            {
                MappingID = dm.MappingID,
                DepartmentID = dm.DepartmentID,
                DepartmentName = dm.Department.DepartmentName,
                MenuID = dm.MenuID,
                MenuName = dm.Menu.MenuName,
                IsActive = dm.IsActive
            }).ToList();
        }

        public async Task AssignMenusToDepartmentAsync(int departmentId, List<int> menuIds, int createdBy)
        {
            // Get existing mappings
            var existingMappings = await _context.DepartmentMenuMappings
                .Where(dm => dm.DepartmentID == departmentId)
                .ToListAsync();

            // Remove mappings that are not in the new list
            var toRemove = existingMappings
                .Where(em => !menuIds.Contains(em.MenuID))
                .ToList();

            foreach (var mapping in toRemove)
            {
                mapping.IsActive = false;
                mapping.UpdatedAt = DateTime.UtcNow;
                mapping.UpdatedBy = createdBy;
            }

            // Add new mappings
            var existingMenuIds = existingMappings.Select(em => em.MenuID).ToList();
            var toAdd = menuIds.Where(menuId => !existingMenuIds.Contains(menuId)).ToList();

            foreach (var menuId in toAdd)
            {
                var existing = existingMappings.FirstOrDefault(em => em.MenuID == menuId);
                if (existing != null)
                {
                    // Reactivate if it was deactivated
                    existing.IsActive = true;
                    existing.UpdatedAt = DateTime.UtcNow;
                    existing.UpdatedBy = createdBy;
                }
                else
                {
                    var newMapping = new DepartmentMenuMapping
                    {
                        DepartmentID = departmentId,
                        MenuID = menuId,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = createdBy
                    };
                    _context.DepartmentMenuMappings.Add(newMapping);
                }
            }

            // Also reactivate existing mappings that are in the new list
            var toReactivate = existingMappings
                .Where(em => menuIds.Contains(em.MenuID) && !em.IsActive)
                .ToList();

            foreach (var mapping in toReactivate)
            {
                mapping.IsActive = true;
                mapping.UpdatedAt = DateTime.UtcNow;
                mapping.UpdatedBy = createdBy;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Menus assigned to department {DepartmentID} by user {UserID}", departmentId, createdBy);
        }
    }
}
