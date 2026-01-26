using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaryanaStatAbstract.API.Models
{
    /// <summary>
    /// Generic audit history table for workflow actions
    /// Tracks all workflow actions across all business tables
    /// </summary>
    [Table("Workflow_AuditHistory")]
    public class WorkflowAuditHistory
    {
        [Key]
        [Column("AuditID")]
        public int AuditID { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("TargetTableName")]
        public string TargetTableName { get; set; } = string.Empty;

        [Required]
        [Column("TargetRecordID")]
        public int TargetRecordID { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("ActionName")]
        public string ActionName { get; set; } = string.Empty;

        [Column("FromStatusID")]
        public int? FromStatusID { get; set; }

        [Required]
        [Column("ToStatusID")]
        public int ToStatusID { get; set; }

        [Column("Remarks", TypeName = "NVARCHAR(MAX)")]
        public string? Remarks { get; set; }

        [Required]
        [Column("ActionByUserID")]
        public int ActionByUserID { get; set; }

        [Required]
        [Column("ActionDate", TypeName = "DATETIME2")]
        public DateTime ActionDate { get; set; }

        [Column("ScreenWorkflowID")]
        public int? ScreenWorkflowID { get; set; }

        // Navigation properties
        [ForeignKey("FromStatusID")]
        public virtual MstWorkflowStatus? FromStatus { get; set; }

        [ForeignKey("ScreenWorkflowID")]
        public virtual ScreenWorkflow? ScreenWorkflow { get; set; }

        [ForeignKey("ToStatusID")]
        public virtual MstWorkflowStatus ToStatus { get; set; } = null!;

        [ForeignKey("ActionByUserID")]
        public virtual MasterUser ActionByUser { get; set; } = null!;
    }
}
