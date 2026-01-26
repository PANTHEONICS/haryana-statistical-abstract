using System.ComponentModel.DataAnnotations;

namespace HaryanaStatAbstract.API.Models.Dtos
{
    /// <summary>
    /// DTO for executing workflow actions
    /// </summary>
    public class WorkflowActionDto
    {
        [Required]
        [MaxLength(100)]
        public string TableName { get; set; } = string.Empty;

        [Required]
        public int RecordId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Remarks are mandatory for rejections
        /// </summary>
        public string? Remarks { get; set; }
    }

    /// <summary>
    /// DTO for workflow action response
    /// </summary>
    public class WorkflowActionResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int NewStatusID { get; set; }
        public string NewStatusName { get; set; } = string.Empty;
        public int? AuditID { get; set; } // Nullable since reset operation doesn't create an audit entry
    }

    /// <summary>
    /// DTO for workflow audit history
    /// </summary>
    public class WorkflowAuditHistoryDto
    {
        public int AuditID { get; set; }
        public string TargetTableName { get; set; } = string.Empty;
        public int TargetRecordID { get; set; }
        public string ActionName { get; set; } = string.Empty;
        public int? FromStatusID { get; set; }
        public string? FromStatusName { get; set; }
        public int ToStatusID { get; set; }
        public string ToStatusName { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public int ActionByUserID { get; set; }
        public string ActionByUserName { get; set; } = string.Empty;
        public DateTime ActionDate { get; set; }
    }
}
