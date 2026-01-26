using HaryanaStatAbstract.API.Models.Dtos;

namespace HaryanaStatAbstract.API.Services
{
    /// <summary>
    /// Interface for generic workflow engine service
    /// </summary>
    public interface IWorkflowService
    {
        /// <summary>
        /// Process a workflow action on any table
        /// </summary>
        /// <param name="tableName">Target table name (e.g., "Tbl_PopulationData")</param>
        /// <param name="recordId">Primary key of the target record</param>
        /// <param name="action">Action name (SubmitToChecker, CheckerReject, CheckerApprove, HeadReject, HeadApprove)</param>
        /// <param name="remarks">Optional remarks (mandatory for rejections)</param>
        /// <param name="currentUserId">User ID performing the action</param>
        /// <returns>Workflow action response</returns>
        Task<WorkflowActionResponseDto> ProcessActionAsync(string tableName, int recordId, string action, string? remarks, int currentUserId);

        /// <summary>
        /// Get audit history for a specific record
        /// </summary>
        /// <param name="tableName">Target table name</param>
        /// <param name="recordId">Primary key of the target record</param>
        /// <returns>List of audit history entries</returns>
        Task<List<WorkflowAuditHistoryDto>> GetAuditHistoryAsync(string tableName, int recordId);

        /// <summary>
        /// Get current workflow status for a record
        /// </summary>
        /// <param name="tableName">Target table name</param>
        /// <param name="recordId">Primary key of the target record</param>
        /// <returns>Current status ID, or null if record doesn't exist</returns>
        Task<int?> GetCurrentStatusAsync(string tableName, int recordId);

        /// <summary>
        /// Process a workflow action at the screen level
        /// </summary>
        /// <param name="screenCode">Screen code (e.g., "CENSUS_POPULATION")</param>
        /// <param name="action">Action name (SubmitToChecker, CheckerReject, CheckerApprove, HeadReject, HeadApprove)</param>
        /// <param name="remarks">Optional remarks (mandatory for rejections)</param>
        /// <param name="currentUserId">User ID performing the action</param>
        /// <returns>Workflow action response</returns>
        Task<WorkflowActionResponseDto> ProcessScreenWorkflowActionAsync(string screenCode, string action, string? remarks, int currentUserId);

        /// <summary>
        /// Get audit history for a screen
        /// </summary>
        /// <param name="screenCode">Screen code</param>
        /// <returns>List of audit history entries</returns>
        Task<List<WorkflowAuditHistoryDto>> GetScreenAuditHistoryAsync(string screenCode);

        /// <summary>
        /// Get current workflow status for a screen
        /// </summary>
        /// <param name="screenCode">Screen code</param>
        /// <returns>Current status ID, or null if screen doesn't exist</returns>
        Task<int?> GetScreenCurrentStatusAsync(string screenCode);

        /// <summary>
        /// Reset screen workflow to draft phase (Admin only - for testing)
        /// Clears all audit history and resets status to Draft (1)
        /// </summary>
        /// <param name="screenCode">Screen code</param>
        /// <param name="currentUserId">User ID performing the reset (must be admin)</param>
        /// <returns>Workflow action response</returns>
        Task<WorkflowActionResponseDto> ResetScreenWorkflowAsync(string screenCode, int currentUserId);
    }
}
