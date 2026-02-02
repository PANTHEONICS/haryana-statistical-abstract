using HaryanaStatAbstract.API.Models.Dtos;
using HaryanaStatAbstract.API.Services;
using HaryanaStatAbstract.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HaryanaStatAbstract.API.Controllers
{
    /// <summary>
    /// Generic Workflow Engine Controller
    /// Handles workflow actions across all business tables
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class WorkflowController : ControllerBase
    {
        private readonly IWorkflowService _workflowService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WorkflowController> _logger;

        public WorkflowController(
            IWorkflowService workflowService,
            ApplicationDbContext context,
            ILogger<WorkflowController> logger)
        {
            _workflowService = workflowService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Execute a workflow action on any table
        /// </summary>
        /// <param name="dto">Workflow action details</param>
        /// <returns>Workflow action response</returns>
        [HttpPost("execute")]
        [ProducesResponseType(typeof(WorkflowActionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExecuteAction([FromBody] WorkflowActionDto dto)
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

            try
            {
                // Convert year to CensusID for census tables (old and new table names)
                // Frontend passes year, but backend uses CensusID as primary key
                int recordId = dto.RecordId;
                if (dto.TableName.ToLower() == "census_population" || dto.TableName.ToLower() == "ap_table_3_2_censuspopulation")
                {
                    var censusRecord = await _context.Table3_2CensusPopulations
                        .FirstOrDefaultAsync(c => c.Year == dto.RecordId);
                    
                    if (censusRecord == null)
                    {
                        return NotFound(new { message = $"Census record for year {dto.RecordId} not found" });
                    }
                    
                    recordId = censusRecord.CensusID;
                }

                var result = await _workflowService.ProcessActionAsync(
                    dto.TableName,
                    recordId,
                    dto.Action,
                    dto.Remarks,
                    userId);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid workflow action request");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Workflow action failed");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing workflow action");
                return StatusCode(500, new { message = "An error occurred while processing the workflow action." });
            }
        }

        /// <summary>
        /// Get audit history for a specific record
        /// </summary>
        /// <param name="tableName">Target table name</param>
        /// <param name="recordId">Primary key of the target record</param>
        /// <returns>List of audit history entries</returns>
        [HttpGet("history/{tableName}/{recordId}")]
        [ProducesResponseType(typeof(List<WorkflowAuditHistoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuditHistory(string tableName, int recordId)
        {
            try
            {
                // Convert year to CensusID for census tables (old and new table names)
                if (tableName.ToLower() == "census_population" || tableName.ToLower() == "ap_table_3_2_censuspopulation")
                {
                    var censusRecord = await _context.Table3_2CensusPopulations
                        .FirstOrDefaultAsync(c => c.Year == recordId);
                    
                    if (censusRecord == null)
                    {
                        return NotFound(new { message = $"Census record for year {recordId} not found" });
                    }
                    
                    recordId = censusRecord.CensusID;
                }

                var history = await _workflowService.GetAuditHistoryAsync(tableName, recordId);
                return Ok(history);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid audit history request");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit history");
                return StatusCode(500, new { message = "An error occurred while retrieving audit history." });
            }
        }

        /// <summary>
        /// Get current workflow status for a record
        /// </summary>
        /// <param name="tableName">Target table name</param>
        /// <param name="recordId">Primary key of the target record</param>
        /// <returns>Current status ID</returns>
        [HttpGet("status/{tableName}/{recordId}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentStatus(string tableName, int recordId)
        {
            try
            {
                // Convert year to CensusID for census tables (old and new table names)
                if (tableName.ToLower() == "census_population" || tableName.ToLower() == "ap_table_3_2_censuspopulation")
                {
                    var censusRecord = await _context.Table3_2CensusPopulations
                        .FirstOrDefaultAsync(c => c.Year == recordId);
                    
                    if (censusRecord == null)
                    {
                        return NotFound(new { message = $"Census record for year {recordId} not found" });
                    }
                    
                    recordId = censusRecord.CensusID;
                }

                var statusId = await _workflowService.GetCurrentStatusAsync(tableName, recordId);
                if (statusId == null)
                {
                    return NotFound(new { message = $"Record {recordId} not found in table {tableName}" });
                }
                return Ok(new { statusId = statusId.Value });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid status request");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current status");
                return StatusCode(500, new { message = "An error occurred while retrieving current status." });
            }
        }

        /// <summary>
        /// Get all workflow statuses
        /// </summary>
        /// <returns>List of all workflow statuses</returns>
        [HttpGet("statuses")]
        [ProducesResponseType(typeof(List<WorkflowStatusDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllStatuses()
        {
            try
            {
                var statuses = await _context.MstWorkflowStatuses
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.DisplayOrder)
                    .ThenBy(s => s.StatusID)
                    .Select(s => new WorkflowStatusDto
                    {
                        StatusID = s.StatusID,
                        StatusName = s.StatusName,
                        StatusCode = s.StatusCode,
                        Description = s.Description,
                        DisplayOrder = s.DisplayOrder,
                        VisualStageKey = s.VisualStageKey
                    })
                    .ToListAsync();

                return Ok(statuses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow statuses");
                return StatusCode(500, new { message = "An error occurred while retrieving workflow statuses." });
            }
        }

        // ============================================
        // Screen-Level Workflow Endpoints
        // ============================================

        /// <summary>
        /// Execute a workflow action at the screen level
        /// </summary>
        /// <param name="dto">Screen workflow action details</param>
        /// <returns>Workflow action response</returns>
        [HttpPost("screen/execute")]
        [ProducesResponseType(typeof(WorkflowActionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExecuteScreenAction([FromBody] ScreenWorkflowActionDto dto)
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

            try
            {
                var result = await _workflowService.ProcessScreenWorkflowActionAsync(
                    dto.ScreenCode,
                    dto.Action,
                    dto.Remarks,
                    userId);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid screen workflow action request");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Screen workflow action failed");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing screen workflow action");
                return StatusCode(500, new { message = "An error occurred while processing the screen workflow action." });
            }
        }

        /// <summary>
        /// Get audit history for a screen
        /// </summary>
        /// <param name="screenCode">Screen code</param>
        /// <returns>List of audit history entries</returns>
        [HttpGet("screen/history/{screenCode}")]
        [ProducesResponseType(typeof(List<WorkflowAuditHistoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetScreenAuditHistory(string screenCode)
        {
            try
            {
                var history = await _workflowService.GetScreenAuditHistoryAsync(screenCode);
                return Ok(history);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid screen audit history request");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving screen audit history");
                return StatusCode(500, new { message = "An error occurred while retrieving screen audit history." });
            }
        }

        /// <summary>
        /// Get current workflow status for a screen
        /// </summary>
        /// <param name="screenCode">Screen code</param>
        /// <returns>Current status ID</returns>
        [HttpGet("screen/status/{screenCode}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetScreenCurrentStatus(string screenCode)
        {
            try
            {
                var statusId = await _workflowService.GetScreenCurrentStatusAsync(screenCode);
                if (statusId == null)
                {
                    return NotFound(new { message = $"Screen workflow not found for screen code: {screenCode}" });
                }
                return Ok(new { statusId = statusId.Value });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving screen current status");
                return StatusCode(500, new { message = "An error occurred while retrieving screen current status." });
            }
        }

        /// <summary>
        /// Reset screen workflow to draft phase (Admin only - for testing)
        /// Clears all audit history and resets status to Draft
        /// </summary>
        /// <param name="screenCode">Screen code</param>
        /// <returns>Workflow action response</returns>
        [HttpPost("screen/reset/{screenCode}")]
        [Authorize(Roles = "System Admin")]
        [ProducesResponseType(typeof(WorkflowActionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResetScreenWorkflow(string screenCode)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    ?? User.FindFirst("sub")?.Value 
                    ?? User.FindFirst("userId")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                var result = await _workflowService.ResetScreenWorkflowAsync(screenCode, currentUserId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized workflow reset attempt");
                return StatusCode(403, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid workflow reset request");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting screen workflow");
                return StatusCode(500, new { message = "An error occurred while resetting workflow." });
            }
        }

        /// <summary>
        /// Get screens waiting for action based on user role
        /// Returns screens grouped by status that require action from the current user
        /// </summary>
        /// <returns>List of screens waiting for action, grouped by status</returns>
        [HttpGet("screens/waiting-for-action")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetScreensWaitingForAction()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                var user = await _context.MasterUsers
                    .Include(u => u.Role)
                    .Include(u => u.Department)
                    .FirstOrDefaultAsync(u => u.UserID == userId && u.IsActive);

                if (user == null)
                {
                    return Unauthorized(new { message = "User not found" });
                }

                var roleName = user.Role.RoleName;
                var departmentId = user.DepartmentID;

                // Get department-menu mappings to determine which screens belong to which department
                var departmentMenuMappings = await _context.DepartmentMenuMappings
                    .Include(dm => dm.Menu)
                    .Include(dm => dm.Department)
                    .Where(dm => dm.IsActive && dm.Menu.IsActive)
                    .ToListAsync();

                // Create a mapping of menu paths to departments
                var menuPathToDepartment = departmentMenuMappings
                    .Where(dm => !string.IsNullOrEmpty(dm.Menu.MenuPath) && dm.Menu.MenuPath != "/")
                    .GroupBy(dm => dm.Menu.MenuPath)
                    .ToDictionary(g => g.Key, g => g.Select(dm => new { dm.DepartmentID, dm.Department.DepartmentName }).ToList());

                var allScreenWorkflows = await _context.ScreenWorkflows
                    .Include(sw => sw.WorkflowStatus)
                    .Include(sw => sw.CreatedByUser)
                    .Where(sw => sw.IsActive)
                    .ToListAsync();

                var screenRegistryDeptMap = new Dictionary<string, (int DeptId, string DeptName)>(StringComparer.OrdinalIgnoreCase);
                try
                {
                    var registries = await _context.MstScreenRegistries.Include(r => r.Department).Where(r => r.Department != null).ToListAsync();
                    foreach (var r in registries) screenRegistryDeptMap[r.ScreenCode] = (r.DepartmentID, r.Department.DepartmentName);
                }
                catch { }

                // Get user's accessible menus to filter screens
                var accessibleMenuPaths = new List<string>();
                var userDepartmentMenus = new List<string>();
                
                if (roleName == "System Admin")
                {
                    // Admin can see all screens
                    accessibleMenuPaths = await _context.MstMenus
                        .Where(m => m.IsActive && !string.IsNullOrEmpty(m.MenuPath) && m.MenuPath != "/")
                        .Select(m => m.MenuPath)
                        .ToListAsync();
                }
                else if (roleName == "DESA Head")
                {
                    // DESA Head can see all non-admin menus except: Data Management, Detail View, Workflow, Board View, Analytics
                    var excludedPaths = new[] { "/data", "/detail", "/workflow", "/board", "/analytics" };
                    accessibleMenuPaths = await _context.MstMenus
                        .Where(m => m.IsActive && !m.IsAdminOnly && !string.IsNullOrEmpty(m.MenuPath) && m.MenuPath != "/" && !excludedPaths.Contains(m.MenuPath))
                        .Select(m => m.MenuPath)
                        .ToListAsync();
                }
                else if (departmentId.HasValue)
                {
                    // Department users (Maker/Checker) can only see menus assigned to their department
                    userDepartmentMenus = await _context.DepartmentMenuMappings
                        .Include(dm => dm.Menu)
                        .Where(dm => dm.DepartmentID == departmentId.Value && dm.IsActive && dm.Menu.IsActive && !string.IsNullOrEmpty(dm.Menu.MenuPath) && dm.Menu.MenuPath != "/")
                        .Select(dm => dm.Menu.MenuPath)
                        .ToListAsync();
                    accessibleMenuPaths = userDepartmentMenus;
                }

                Func<string, string, string, int?> getDepartmentForScreen = (tableName, screenCode, screenName) =>
                {
                    if (screenRegistryDeptMap.TryGetValue(screenCode, out var regDept)) return regDept.DeptId;
                    foreach (var mapping in departmentMenuMappings)
                    {
                        var menu = mapping.Menu;
                        var menuPath = menu.MenuPath?.ToLower() ?? "";
                        var menuName = menu.MenuName?.ToLower() ?? "";
                        var tableNameLower = tableName?.ToLower() ?? "";
                        var screenCodeLower = screenCode?.ToLower() ?? "";
                        var screenNameLower = screenName?.ToLower() ?? "";

                        // Match by menu path containing keywords
                        if (!string.IsNullOrEmpty(menuPath) && menuPath != "/")
                        {
                            // Check if menu path contains table-related keywords
                            bool pathMatches = false;
                            
                            if (menuPath.Contains("census") && (tableNameLower.Contains("census") || screenCodeLower.Contains("census") || screenNameLower.Contains("census")))
                            {
                                pathMatches = true;
                            }
                            else if ((menuPath.Contains("table6-1") || menuPath.Contains("table6_1") || menuPath.Contains("education")) && 
                                     (tableNameLower.Contains("education") || screenCodeLower.Contains("education") || screenNameLower.Contains("education") || screenNameLower.Contains("institution")))
                            {
                                pathMatches = true;
                            }
                            else if ((menuPath.Contains("table3-2") || menuPath.Contains("table3_2")) && 
                                     (tableNameLower.Contains("table") || screenCodeLower.Contains("table") || screenNameLower.Contains("census")))
                            {
                                pathMatches = true;
                            }
                            else if ((menuPath.Contains("social-security") || menuPath.Contains("table7-1") || menuPath.Contains("table7_1")) && 
                                     (tableNameLower.Contains("ssd") || screenCodeLower.Contains("ssd") || screenCodeLower.Contains("table_7_1") || screenNameLower.Contains("police")))
                            {
                                pathMatches = true;
                            }
                            else if ((menuPath.Contains("social-security") || menuPath.Contains("table7-6") || menuPath.Contains("table7_6")) && 
                                     (tableNameLower.Contains("ssd") || screenCodeLower.Contains("ssd") || screenCodeLower.Contains("table_7_6") || screenNameLower.Contains("prisoner")))
                            {
                                pathMatches = true;
                            }
                            else if ((menuPath.Contains("social-security") || menuPath.Contains("table7-7") || menuPath.Contains("table7_7")) && 
                                     (tableNameLower.Contains("ssd") || screenCodeLower.Contains("ssd") || screenCodeLower.Contains("table_7_7") || screenNameLower.Contains("expenditure") || screenNameLower.Contains("maintenance")))
                            {
                                pathMatches = true;
                            }
                            else if ((menuPath.Contains("social-security") || menuPath.Contains("table7-8") || menuPath.Contains("table7_8")) && 
                                     (tableNameLower.Contains("ssd") || screenCodeLower.Contains("ssd") || screenCodeLower.Contains("table_7_8") || screenNameLower.Contains("jail") || screenNameLower.Contains("industry")))
                            {
                                pathMatches = true;
                            }
                            if (pathMatches)
                            {
                                return mapping.DepartmentID;
                            }
                        }

                        // Match by menu name containing screen keywords
                        if (!string.IsNullOrEmpty(menuName))
                        {
                            bool nameMatches = false;
                            
                            if (menuName.Contains("census") && (screenNameLower.Contains("census") || tableNameLower.Contains("census")))
                            {
                                nameMatches = true;
                            }
                            else if (menuName.Contains("institution") && (screenNameLower.Contains("institution") || screenNameLower.Contains("education")))
                            {
                                nameMatches = true;
                            }
                            else if ((menuName.Contains("table 6.1") || menuName.Contains("table6.1")) && screenNameLower.Contains("table"))
                            {
                                nameMatches = true;
                            }
                            else if ((menuName.Contains("table 3.2") || menuName.Contains("table3.2")) && screenNameLower.Contains("census"))
                            {
                                nameMatches = true;
                            }
                            else if ((menuName.Contains("police") || menuName.Contains("table 7.1") || menuName.Contains("table7.1") || menuName.Contains("sanctioned")) && 
                                     (screenCodeLower.Contains("ssd") || tableNameLower.Contains("ssd") || screenNameLower.Contains("police")))
                            {
                                nameMatches = true;
                            }
                            else if ((menuName.Contains("prisoner") || menuName.Contains("table 7.6") || menuName.Contains("table7.6") || menuName.Contains("classwise")) && 
                                     (screenCodeLower.Contains("ssd") || tableNameLower.Contains("ssd") || screenNameLower.Contains("prisoner")))
                            {
                                nameMatches = true;
                            }
                            else if ((menuName.Contains("expenditure") || menuName.Contains("maintenance") || menuName.Contains("table 7.7") || menuName.Contains("table7.7")) && 
                                     (screenCodeLower.Contains("ssd") || tableNameLower.Contains("ssd") || screenNameLower.Contains("expenditure")))
                            {
                                nameMatches = true;
                            }
                            else if ((menuName.Contains("jail") || menuName.Contains("industry") || menuName.Contains("table 7.8") || menuName.Contains("table7.8") || menuName.Contains("production")) && 
                                     (screenCodeLower.Contains("ssd") || tableNameLower.Contains("ssd") || screenNameLower.Contains("jail")))
                            {
                                nameMatches = true;
                            }
                            if (nameMatches)
                            {
                                return mapping.DepartmentID;
                            }
                        }
                    }
                    return null;
                };

                // Filter screens based on role and status
                var screensWaitingForAction = new List<object>();
                const int STATUS_MAKER_ENTRY = 1;
                const int STATUS_PENDING_CHECKER = 2;
                const int STATUS_PENDING_HEAD = 4;
                const int STATUS_APPROVED = 6;

                foreach (var screenWorkflow in allScreenWorkflows)
                {
                    var screenCode = screenWorkflow.ScreenCode;
                    var tableName = screenWorkflow.TableName;
                    var screenName = screenWorkflow.ScreenName;
                    var currentStatusId = screenWorkflow.CurrentStatusID;
                    var statusName = screenWorkflow.WorkflowStatus?.StatusName ?? "Unknown";

                    var screenDepartmentId = getDepartmentForScreen(tableName, screenCode, screenName);
                    string? screenDepartmentName = null;
                    if (screenDepartmentId.HasValue)
                    {
                        if (screenRegistryDeptMap.TryGetValue(screenCode, out var regDept)) screenDepartmentName = regDept.DeptName;
                        if (string.IsNullOrEmpty(screenDepartmentName))
                            screenDepartmentName = departmentMenuMappings.Where(dm => dm.DepartmentID == screenDepartmentId.Value).Select(dm => dm.Department.DepartmentName).FirstOrDefault();
                    }

                    // For Maker/Checker: Only include screens from their department
                    if ((roleName == "Department Maker" || roleName == "Department Checker") && departmentId.HasValue)
                    {
                        // Check if this screen belongs to user's department
                        bool belongsToUserDepartment = false;
                        
                        // Method 1: Check by department ID match
                        if (screenDepartmentId.HasValue && screenDepartmentId.Value == departmentId.Value)
                        {
                            belongsToUserDepartment = true;
                        }
                        
                        // Method 2: Check by menu path/name matching with user's department menus
                        if (!belongsToUserDepartment)
                        {
                            var userDeptMenus = departmentMenuMappings
                                .Where(dm => dm.DepartmentID == departmentId.Value)
                                .Select(dm => dm.Menu)
                                .ToList();
                            
                            var tableNameLower = tableName?.ToLower() ?? "";
                            var screenCodeLower = screenCode?.ToLower() ?? "";
                            var screenNameLower = screenName?.ToLower() ?? "";

                            foreach (var menu in userDeptMenus)
                            {
                                var menuPath = menu.MenuPath?.ToLower() ?? "";
                                var menuName = menu.MenuName?.ToLower() ?? "";

                                // Match by keywords - more flexible matching
                                bool menuMatches = false;
                                
                                // Match census-related screens
                                if ((menuPath.Contains("census") || menuName.Contains("census")) && 
                                    (tableNameLower.Contains("census") || screenCodeLower.Contains("census") || screenNameLower.Contains("census") || 
                                     tableNameLower.Contains("table_3_2") || screenCodeLower.Contains("table_3_2") || screenCodeLower.Contains("table3_2")))
                                {
                                    menuMatches = true;
                                }
                                // Match education/institution-related screens
                                else if ((menuPath.Contains("education") || menuPath.Contains("table6-1") || menuPath.Contains("table6_1") || 
                                         menuName.Contains("education") || menuName.Contains("institution") || menuName.Contains("table 6.1")) && 
                                         (tableNameLower.Contains("education") || screenCodeLower.Contains("education") || screenNameLower.Contains("education") || 
                                          screenNameLower.Contains("institution") || tableNameLower.Contains("table_6_1") || screenCodeLower.Contains("table_6_1") || 
                                          screenCodeLower.Contains("table6_1")))
                                {
                                    menuMatches = true;
                                }
                                // Match SSD / Social Security Defence / Table 7.1
                                else if ((menuPath.Contains("social-security") || menuPath.Contains("table7-1") || menuPath.Contains("table7_1") || 
                                         menuName.Contains("police") || menuName.Contains("table 7.1") || menuName.Contains("sanctioned")) && 
                                         (tableNameLower.Contains("ssd") || screenCodeLower.Contains("ssd") || screenCodeLower.Contains("table_7_1") || screenNameLower.Contains("police")))
                                {
                                    menuMatches = true;
                                }
                                // Match SSD / Table 7.6 Prisoners
                                else if ((menuPath.Contains("social-security") || menuPath.Contains("table7-6") || menuPath.Contains("table7_6") || 
                                         menuName.Contains("prisoner") || menuName.Contains("table 7.6") || menuName.Contains("classwise")) && 
                                         (tableNameLower.Contains("ssd") || screenCodeLower.Contains("ssd") || screenCodeLower.Contains("table_7_6") || screenNameLower.Contains("prisoner")))
                                {
                                    menuMatches = true;
                                }
                                // Match SSD / Table 7.7 Expenditure
                                else if ((menuPath.Contains("social-security") || menuPath.Contains("table7-7") || menuPath.Contains("table7_7") || 
                                         menuName.Contains("expenditure") || menuName.Contains("maintenance") || menuName.Contains("table 7.7")) && 
                                         (tableNameLower.Contains("ssd") || screenCodeLower.Contains("ssd") || screenCodeLower.Contains("table_7_7") || screenNameLower.Contains("expenditure")))
                                {
                                    menuMatches = true;
                                }
                                // Match SSD / Table 7.8 Jail Industry
                                else if ((menuPath.Contains("social-security") || menuPath.Contains("table7-8") || menuPath.Contains("table7_8") || 
                                         menuName.Contains("jail") || menuName.Contains("industry") || menuName.Contains("table 7.8")) && 
                                         (tableNameLower.Contains("ssd") || screenCodeLower.Contains("ssd") || screenCodeLower.Contains("table_7_8") || screenNameLower.Contains("jail")))
                                {
                                    menuMatches = true;
                                }
                                
                                if (menuMatches)
                                {
                                    belongsToUserDepartment = true;
                                    break;
                                }
                            }
                        }

                        if (!belongsToUserDepartment)
                        {
                            continue; // Skip screens not from user's department
                        }
                    }

                    bool requiresAction = false;
                    string actionType = "";

                    // Determine if this screen requires action from current user
                    if (roleName == "Department Maker")
                    {
                        // Maker can act on Maker Entry (1) or Rejected by Checker (3, which loops back to 1)
                        if (currentStatusId == STATUS_MAKER_ENTRY)
                        {
                            requiresAction = true;
                            actionType = "Submit to Checker";
                        }
                    }
                    else if (roleName == "Department Checker")
                    {
                        // Checker can act on Pending Checker (2) or Rejected by DESA Head (5, which loops back to 2)
                        if (currentStatusId == STATUS_PENDING_CHECKER)
                        {
                            requiresAction = true;
                            actionType = "Review & Approve/Reject";
                        }
                    }
                    else if (roleName == "DESA Head")
                    {
                        // DESA Head can act on Pending Head (4)
                        if (currentStatusId == STATUS_PENDING_HEAD)
                        {
                            requiresAction = true;
                            actionType = "Final Approval/Reject";
                        }
                    }

                    // Include all accessible screens (not just waiting for action)
                    screensWaitingForAction.Add(new
                    {
                        screenWorkflowID = screenWorkflow.ScreenWorkflowID,
                        screenName = screenWorkflow.ScreenName,
                        screenCode = screenCode,
                        tableName = tableName,
                        currentStatusID = currentStatusId,
                        currentStatusName = statusName,
                        requiresAction = requiresAction,
                        actionType = actionType,
                        departmentID = screenDepartmentId,
                        departmentName = screenDepartmentName,
                        createdByUserName = screenWorkflow.CreatedByUser?.FullName ?? screenWorkflow.CreatedByUser?.LoginID ?? "Unknown",
                        updatedAt = screenWorkflow.UpdatedAt,
                        createdAt = screenWorkflow.CreatedAt
                    });
                }

                // For DESA Head: Group by department first, then by status
                // For Maker/Checker: Group by status only
                if (roleName == "DESA Head")
                {
                    // Group by department, then by status
                    var groupedByDepartment = screensWaitingForAction
                        .GroupBy(s => new
                        {
                            deptId = ((dynamic)s).departmentID != null ? (int?)((dynamic)s).departmentID : null,
                            deptName = (string)((dynamic)s).departmentName ?? "Unknown Department"
                        })
                        .Select(deptGroup => new
                        {
                            departmentID = deptGroup.Key.deptId,
                            departmentName = deptGroup.Key.deptName,
                            screensByStatus = deptGroup
                                .GroupBy(s => new
                                {
                                    statusId = (int)((dynamic)s).currentStatusID,
                                    statusName = (string)((dynamic)s).currentStatusName
                                })
                                .Select(statusGroup => new
                                {
                                    statusId = statusGroup.Key.statusId,
                                    statusName = statusGroup.Key.statusName,
                                    screens = statusGroup.ToList(),
                                    count = statusGroup.Count(),
                                    waitingForActionCount = statusGroup.Count(s => ((dynamic)s).requiresAction == true)
                                })
                                .OrderBy(s => s.statusId)
                                .ToList(),
                            totalScreens = deptGroup.Count(),
                            totalWaitingForAction = deptGroup.Count(s => ((dynamic)s).requiresAction == true)
                        })
                        .OrderBy(d => d.departmentName)
                        .ToList();

                    return Ok(new
                    {
                        role = roleName,
                        departmentId = departmentId,
                        screensByDepartment = groupedByDepartment,
                        screensByStatus = new List<object>(), // Empty for DESA Head
                        totalScreens = screensWaitingForAction.Count,
                        totalWaitingForAction = screensWaitingForAction.Count(s => ((dynamic)s).requiresAction == true)
                    });
                }
                else
                {
                    // Group by status for Maker/Checker/Admin
                    var groupedByStatus = screensWaitingForAction
                        .GroupBy(s => new
                        {
                            statusId = (int)((dynamic)s).currentStatusID,
                            statusName = (string)((dynamic)s).currentStatusName
                        })
                        .Select(g => new
                        {
                            statusId = g.Key.statusId,
                            statusName = g.Key.statusName,
                            screens = g.ToList(),
                            count = g.Count(),
                            waitingForActionCount = g.Count(s => ((dynamic)s).requiresAction == true)
                        })
                        .OrderBy(g => g.statusId)
                        .ToList();

                    return Ok(new
                    {
                        role = roleName,
                        departmentId = departmentId,
                        screensByStatus = groupedByStatus,
                        screensByDepartment = new List<object>(), // Empty for Maker/Checker
                        totalScreens = screensWaitingForAction.Count,
                        totalWaitingForAction = screensWaitingForAction.Count(s => ((dynamic)s).requiresAction == true)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving screens waiting for action");
                return StatusCode(500, new { message = "An error occurred while retrieving screens waiting for action." });
            }
        }
    }
}
