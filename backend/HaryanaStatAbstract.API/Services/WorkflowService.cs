using HaryanaStatAbstract.API.Data;
using HaryanaStatAbstract.API.Helpers;
using HaryanaStatAbstract.API.Models;
using HaryanaStatAbstract.API.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HaryanaStatAbstract.API.Services
{
    /// <summary>
    /// Generic Workflow Engine Service Implementation
    /// Handles workflow actions across all business tables dynamically
    /// </summary>
    public class WorkflowService : IWorkflowService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WorkflowService> _logger;

        // Status IDs (loaded from database)
        private const int STATUS_DRAFT = 1;
        private const int STATUS_PENDING_CHECKER = 2;
        private const int STATUS_REJECTED_BY_CHECKER = 3;
        private const int STATUS_PENDING_HEAD = 4;
        private const int STATUS_REJECTED_BY_HEAD = 5;
        private const int STATUS_APPROVED = 6;

        public WorkflowService(ApplicationDbContext context, ILogger<WorkflowService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<WorkflowActionResponseDto> ProcessActionAsync(string tableName, int recordId, string action, string? remarks, int currentUserId)
        {
            // Validate table name to prevent SQL injection
            if (!IsValidTableName(tableName))
            {
                throw new ArgumentException($"Invalid table name: {tableName}");
            }

            // Get current status
            var currentStatusId = await GetCurrentStatusAsync(tableName, recordId);
            if (currentStatusId == null)
            {
                throw new InvalidOperationException($"Record {recordId} not found in table {tableName}");
            }

            // Determine new status based on action
            int fromStatusId = currentStatusId.Value;
            int toStatusId;
            string actionName;

            switch (action.ToLower())
            {
                case "submittochecker":
                    if (fromStatusId != STATUS_DRAFT && fromStatusId != STATUS_REJECTED_BY_CHECKER)
                    {
                        throw new InvalidOperationException($"Cannot submit to checker. Current status: {fromStatusId}");
                    }
                    toStatusId = STATUS_PENDING_CHECKER;
                    actionName = "Submit to Checker";
                    break;

                case "checkerreject":
                    if (fromStatusId != STATUS_PENDING_CHECKER && fromStatusId != STATUS_REJECTED_BY_HEAD)
                    {
                        throw new InvalidOperationException($"Cannot reject. Current status: {fromStatusId}");
                    }
                    if (string.IsNullOrWhiteSpace(remarks))
                    {
                        throw new ArgumentException("Remarks are mandatory for rejection");
                    }
                    // Happy Path: Loop back to Maker Entry instead of Rejected by Checker status
                    toStatusId = STATUS_DRAFT;
                    actionName = fromStatusId == STATUS_REJECTED_BY_HEAD 
                        ? "Rejected by Checker (returned to Maker Entry)" 
                        : "Rejected by Checker (returned to Maker Entry)";
                    break;

                case "checkerapprove":
                    if (fromStatusId != STATUS_PENDING_CHECKER && fromStatusId != STATUS_REJECTED_BY_HEAD)
                    {
                        throw new InvalidOperationException($"Cannot approve. Current status: {fromStatusId}");
                    }
                    toStatusId = STATUS_PENDING_HEAD;
                    actionName = fromStatusId == STATUS_REJECTED_BY_HEAD 
                        ? "Re-approved by Checker (after Head rejection)" 
                        : "Approved by Checker";
                    break;

                case "headreject":
                    if (fromStatusId != STATUS_PENDING_HEAD)
                    {
                        throw new InvalidOperationException($"Cannot reject. Current status: {fromStatusId}");
                    }
                    if (string.IsNullOrWhiteSpace(remarks))
                    {
                        throw new ArgumentException("Remarks are mandatory for rejection");
                    }
                    // Happy Path: Loop back to Pending Checker instead of Rejected by DESA Head status
                    toStatusId = STATUS_PENDING_CHECKER;
                    actionName = "Rejected by DESA Head (returned to Checker)";
                    break;

                case "headapprove":
                    if (fromStatusId != STATUS_PENDING_HEAD)
                    {
                        throw new InvalidOperationException($"Cannot approve. Current status: {fromStatusId}");
                    }
                    toStatusId = STATUS_APPROVED;
                    actionName = "Approved by DESA Head";
                    break;

                default:
                    throw new ArgumentException($"Unknown action: {action}");
            }

            // Update the target table using raw SQL (dynamic table update)
            var updateSql = $@"
                UPDATE [{tableName}]
                SET [CurrentStatusID] = {{0}}
                WHERE (SELECT TOP 1 COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
                       WHERE TABLE_NAME = '{tableName}' AND CONSTRAINT_NAME LIKE 'PK_%') = {{1}}";

            // Get the primary key column name dynamically
            var primaryKeyColumn = await GetPrimaryKeyColumnNameAsync(tableName);
            if (string.IsNullOrEmpty(primaryKeyColumn))
            {
                throw new InvalidOperationException($"Could not determine primary key for table {tableName}");
            }

            // Use parameterized query to prevent SQL injection
            var safeUpdateSql = $@"
                UPDATE [{tableName}]
                SET [CurrentStatusID] = @ToStatusId
                WHERE [{primaryKeyColumn}] = @RecordId";

            await _context.Database.ExecuteSqlRawAsync(
                safeUpdateSql,
                new Microsoft.Data.SqlClient.SqlParameter("@ToStatusId", toStatusId),
                new Microsoft.Data.SqlClient.SqlParameter("@RecordId", recordId));

            // Get status names
            var fromStatus = await _context.MstWorkflowStatuses.FindAsync(fromStatusId);
            var toStatus = await _context.MstWorkflowStatuses.FindAsync(toStatusId);

            // Create audit history entry
            var auditHistory = new WorkflowAuditHistory
            {
                TargetTableName = tableName,
                TargetRecordID = recordId,
                ActionName = actionName,
                FromStatusID = fromStatusId,
                ToStatusID = toStatusId,
                Remarks = remarks,
                ActionByUserID = currentUserId,
                ActionDate = HaryanaStatAbstract.API.Helpers.DateTimeHelper.GetISTNow()
            };

            _context.WorkflowAuditHistories.Add(auditHistory);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Workflow action processed: Table={TableName}, RecordID={RecordId}, Action={Action}, FromStatus={FromStatus}, ToStatus={ToStatus}, UserID={UserId}",
                tableName, recordId, action, fromStatusId, toStatusId, currentUserId);

            return new WorkflowActionResponseDto
            {
                Success = true,
                Message = $"{actionName} successfully",
                NewStatusID = toStatusId,
                NewStatusName = toStatus?.StatusName ?? "Unknown",
                AuditID = auditHistory.AuditID
            };
        }

        public async Task<List<WorkflowAuditHistoryDto>> GetAuditHistoryAsync(string tableName, int recordId)
        {
            // Validate table name
            if (!IsValidTableName(tableName))
            {
                throw new ArgumentException($"Invalid table name: {tableName}");
            }

            var auditHistory = await _context.WorkflowAuditHistories
                .Include(a => a.FromStatus)
                .Include(a => a.ToStatus)
                .Include(a => a.ActionByUser)
                .Where(a => a.TargetTableName == tableName && a.TargetRecordID == recordId)
                .OrderByDescending(a => a.ActionDate)
                .ToListAsync();

            return auditHistory.Select(a => new WorkflowAuditHistoryDto
            {
                AuditID = a.AuditID,
                TargetTableName = a.TargetTableName,
                TargetRecordID = a.TargetRecordID,
                ActionName = a.ActionName,
                FromStatusID = a.FromStatusID,
                FromStatusName = a.FromStatus?.StatusName,
                ToStatusID = a.ToStatusID,
                ToStatusName = a.ToStatus?.StatusName ?? "Unknown",
                Remarks = a.Remarks,
                ActionByUserID = a.ActionByUserID,
                ActionByUserName = a.ActionByUser.FullName ?? a.ActionByUser.LoginID,
                ActionDate = a.ActionDate
            }).ToList();
        }

        public async Task<int?> GetCurrentStatusAsync(string tableName, int recordId)
        {
            // Validate table name
            if (!IsValidTableName(tableName))
            {
                throw new ArgumentException($"Invalid table name: {tableName}");
            }

            // Get the primary key column name
            var primaryKeyColumn = await GetPrimaryKeyColumnNameAsync(tableName);
            if (string.IsNullOrEmpty(primaryKeyColumn))
            {
                return null;
            }

            // Query current status using raw SQL with connection
            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = $"SELECT [CurrentStatusID] FROM [{tableName}] WHERE [{primaryKeyColumn}] = @RecordId";
                var param = command.CreateParameter();
                param.ParameterName = "@RecordId";
                param.Value = recordId;
                command.Parameters.Add(param);
                
                var scalarResult = await command.ExecuteScalarAsync();
                if (scalarResult != null && scalarResult != DBNull.Value)
                {
                    return Convert.ToInt32(scalarResult);
                }
            }
            finally
            {
                await connection.CloseAsync();
            }

            return null;
        }

        /// <summary>
        /// Get primary key column name for a table
        /// </summary>
        private async Task<string?> GetPrimaryKeyColumnNameAsync(string tableName)
        {
            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT TOP 1 COLUMN_NAME
                    FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                    WHERE TABLE_NAME = @TableName
                      AND CONSTRAINT_NAME LIKE 'PK_%'
                    ORDER BY ORDINAL_POSITION";
                
                var param = command.CreateParameter();
                param.ParameterName = "@TableName";
                param.Value = tableName;
                command.Parameters.Add(param);
                
                var result = await command.ExecuteScalarAsync();
                return result?.ToString();
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        /// <summary>
        /// Validate table name to prevent SQL injection
        /// Only allows alphanumeric characters, underscores, and specific prefixes
        /// </summary>
        private bool IsValidTableName(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                return false;

            // Only allow alphanumeric, underscores, and specific prefixes (Tbl_, Mst_)
            return System.Text.RegularExpressions.Regex.IsMatch(
                tableName,
                @"^[A-Za-z][A-Za-z0-9_]*$",
                System.Text.RegularExpressions.RegexOptions.None);
        }

        // ============================================
        // Screen-Level Workflow Methods
        // ============================================

        public async Task<WorkflowActionResponseDto> ProcessScreenWorkflowActionAsync(string screenCode, string action, string? remarks, int currentUserId)
        {
            // Get the screen workflow record
            var screenWorkflow = await _context.ScreenWorkflows
                .Include(s => s.WorkflowStatus)
                .Include(s => s.CreatedByUser)
                .FirstOrDefaultAsync(s => s.ScreenCode == screenCode && s.IsActive);

            if (screenWorkflow == null)
            {
                throw new InvalidOperationException($"Screen workflow not found for screen code: {screenCode}");
            }

            // Get current status
            int fromStatusId = screenWorkflow.CurrentStatusID;
            int toStatusId;
            string actionName;

            // Determine new status based on action (same logic as record-level)
            switch (action.ToLower())
            {
                case "submittochecker":
                    if (fromStatusId != STATUS_DRAFT && fromStatusId != STATUS_REJECTED_BY_CHECKER)
                    {
                        throw new InvalidOperationException($"Cannot submit to checker. Current status: {fromStatusId}");
                    }
                    toStatusId = STATUS_PENDING_CHECKER;
                    actionName = "Submitted to Checker";
                    break;

                case "checkerreject":
                    if (fromStatusId != STATUS_PENDING_CHECKER && fromStatusId != STATUS_REJECTED_BY_HEAD)
                    {
                        throw new InvalidOperationException($"Cannot reject. Current status: {fromStatusId}");
                    }
                    if (string.IsNullOrWhiteSpace(remarks))
                    {
                        throw new ArgumentException("Remarks are mandatory for rejection");
                    }
                    // Happy Path: Loop back to Maker Entry instead of Rejected by Checker status
                    toStatusId = STATUS_DRAFT;
                    actionName = fromStatusId == STATUS_REJECTED_BY_HEAD 
                        ? "Rejected by Checker (returned to Maker Entry)" 
                        : "Rejected by Checker (returned to Maker Entry)";
                    break;

                case "checkerapprove":
                    if (fromStatusId != STATUS_PENDING_CHECKER && fromStatusId != STATUS_REJECTED_BY_HEAD)
                    {
                        throw new InvalidOperationException($"Cannot approve. Current status: {fromStatusId}");
                    }
                    toStatusId = STATUS_PENDING_HEAD;
                    actionName = fromStatusId == STATUS_REJECTED_BY_HEAD 
                        ? "Re-approved by Checker (after Head rejection)" 
                        : "Approved by Checker";
                    break;

                case "headreject":
                    if (fromStatusId != STATUS_PENDING_HEAD)
                    {
                        throw new InvalidOperationException($"Cannot reject. Current status: {fromStatusId}");
                    }
                    if (string.IsNullOrWhiteSpace(remarks))
                    {
                        throw new ArgumentException("Remarks are mandatory for rejection");
                    }
                    // Happy Path: Loop back to Pending Checker instead of Rejected by DESA Head status
                    toStatusId = STATUS_PENDING_CHECKER;
                    actionName = "Rejected by DESA Head (returned to Checker)";
                    break;

                case "headapprove":
                    if (fromStatusId != STATUS_PENDING_HEAD)
                    {
                        throw new InvalidOperationException($"Cannot approve. Current status: {fromStatusId}");
                    }
                    toStatusId = STATUS_APPROVED;
                    actionName = "Approved by DESA Head";
                    break;

                default:
                    throw new ArgumentException($"Unknown action: {action}");
            }

            // Update screen workflow status
            screenWorkflow.CurrentStatusID = toStatusId;
            screenWorkflow.UpdatedAt = DateTime.UtcNow;

            // Get status names
            var fromStatus = await _context.MstWorkflowStatuses.FindAsync(fromStatusId);
            var toStatus = await _context.MstWorkflowStatuses.FindAsync(toStatusId);
            
            // Update denormalized status name
            if (toStatus != null)
            {
                screenWorkflow.CurrentStatusName = toStatus.StatusName;
            }

            // Create audit history entry for screen-level workflow
            var auditHistory = new WorkflowAuditHistory
            {
                TargetTableName = screenWorkflow.TableName,
                TargetRecordID = 0, // Screen-level workflows don't have specific record IDs
                ActionName = actionName,
                FromStatusID = fromStatusId,
                ToStatusID = toStatusId,
                Remarks = remarks,
                ActionByUserID = currentUserId,
                ActionDate = HaryanaStatAbstract.API.Helpers.DateTimeHelper.GetISTNow(),
                ScreenWorkflowID = screenWorkflow.ScreenWorkflowID // Link to screen workflow
            };

            _context.WorkflowAuditHistories.Add(auditHistory);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Screen workflow action processed: ScreenCode={ScreenCode}, Action={Action}, FromStatus={FromStatus}, ToStatus={ToStatus}, UserID={UserId}",
                screenCode, action, fromStatusId, toStatusId, currentUserId);

            return new WorkflowActionResponseDto
            {
                Success = true,
                Message = $"{actionName} successfully",
                NewStatusID = toStatusId,
                NewStatusName = toStatus?.StatusName ?? "Unknown",
                AuditID = auditHistory.AuditID
            };
        }

        public async Task<List<WorkflowAuditHistoryDto>> GetScreenAuditHistoryAsync(string screenCode)
        {
            var screenWorkflow = await _context.ScreenWorkflows
                .FirstOrDefaultAsync(s => s.ScreenCode == screenCode && s.IsActive);

            if (screenWorkflow == null)
            {
                throw new InvalidOperationException($"Screen workflow not found for screen code: {screenCode}");
            }

            var auditHistory = await _context.WorkflowAuditHistories
                .Include(a => a.FromStatus)
                .Include(a => a.ToStatus)
                .Include(a => a.ActionByUser)
                .Where(a => a.ScreenWorkflowID == screenWorkflow.ScreenWorkflowID)
                .OrderByDescending(a => a.ActionDate)
                .ToListAsync();

            return auditHistory.Select(a => new WorkflowAuditHistoryDto
            {
                AuditID = a.AuditID,
                TargetTableName = a.TargetTableName,
                TargetRecordID = a.TargetRecordID,
                ActionName = a.ActionName,
                FromStatusID = a.FromStatusID,
                FromStatusName = a.FromStatus?.StatusName,
                ToStatusID = a.ToStatusID,
                ToStatusName = a.ToStatus?.StatusName ?? "Unknown",
                Remarks = a.Remarks,
                ActionByUserID = a.ActionByUserID,
                ActionByUserName = a.ActionByUser.FullName ?? a.ActionByUser.LoginID,
                ActionDate = a.ActionDate
            }).ToList();
        }

        public async Task<int?> GetScreenCurrentStatusAsync(string screenCode)
        {
            var screenWorkflow = await _context.ScreenWorkflows
                .FirstOrDefaultAsync(s => s.ScreenCode == screenCode && s.IsActive);

            if (screenWorkflow == null)
            {
                return null;
            }

            return screenWorkflow.CurrentStatusID;
        }

        /// <summary>
        /// Reset screen workflow to draft phase (Admin only - for testing)
        /// Clears all audit history and resets status to Draft (1)
        /// </summary>
        public async Task<WorkflowActionResponseDto> ResetScreenWorkflowAsync(string screenCode, int currentUserId)
        {
            // Get the screen workflow record
            var screenWorkflow = await _context.ScreenWorkflows
                .Include(s => s.WorkflowStatus)
                .FirstOrDefaultAsync(s => s.ScreenCode == screenCode && s.IsActive);

            if (screenWorkflow == null)
            {
                throw new InvalidOperationException($"Screen workflow not found for screen code: {screenCode}");
            }

            // Verify user is admin (check role)
            var user = await _context.MasterUsers
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserID == currentUserId);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            if (user.Role?.RoleName != "System Admin")
            {
                throw new UnauthorizedAccessException("Only System Admin can reset workflow");
            }

            // Get current status before reset
            int fromStatusId = screenWorkflow.CurrentStatusID;

            // Delete all audit history for this screen
            var auditHistories = await _context.WorkflowAuditHistories
                .Where(a => a.ScreenWorkflowID == screenWorkflow.ScreenWorkflowID)
                .ToListAsync();

            if (auditHistories.Any())
            {
                _context.WorkflowAuditHistories.RemoveRange(auditHistories);
                _logger.LogWarning(
                    "Admin {UserId} reset workflow for screen {ScreenCode}. Deleted {Count} audit history entries.",
                    currentUserId, screenCode, auditHistories.Count);
            }

            // Reset screen workflow to Draft (Status 1)
            screenWorkflow.CurrentStatusID = STATUS_DRAFT;
            screenWorkflow.CurrentStatusName = null; // Clear denormalized status name
            screenWorkflow.UpdatedAt = DateTime.UtcNow;

            // Get draft status
            var draftStatus = await _context.MstWorkflowStatuses.FindAsync(STATUS_DRAFT);
            
            // Update denormalized status name if status exists
            if (draftStatus != null)
            {
                screenWorkflow.CurrentStatusName = draftStatus.StatusName;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Workflow reset: Screen={ScreenCode}, FromStatus={FromStatus}, ToStatus=Maker Entry, UserID={UserId}",
                screenCode, fromStatusId, currentUserId);

            return new WorkflowActionResponseDto
            {
                Success = true,
                Message = "Workflow reset to Maker Entry phase successfully. All audit history has been cleared.",
                NewStatusID = STATUS_DRAFT,
                NewStatusName = draftStatus?.StatusName ?? "Maker Entry",
                AuditID = null // No audit entry for reset (history is cleared)
            };
        }
    }
}
