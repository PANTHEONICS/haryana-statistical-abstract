namespace HaryanaStatAbstract.API.Models
{
    /// <summary>
    /// Interface that all workflow-enabled entities must implement
    /// Every business table that needs workflow integration must implement this interface
    /// </summary>
    public interface IWorkflowEntity
    {
        /// <summary>
        /// Current workflow status ID (references Mst_WorkflowStatus.StatusID)
        /// Default value should be 1 (Draft)
        /// </summary>
        int CurrentStatusID { get; set; }

        /// <summary>
        /// User ID who created the record (references Master_User.UserID)
        /// </summary>
        int CreatedByUserID { get; set; }
    }
}
