using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaryanaStatAbstract.API.Models
{
    /// <summary>
    /// Master table for Workflow Statuses
    /// </summary>
    [Table("Mst_WorkflowStatus")]
    public class MstWorkflowStatus
    {
        [Key]
        [Column("StatusID")]
        public int StatusID { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("StatusName")]
        public string StatusName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("StatusCode")]
        public string StatusCode { get; set; } = string.Empty;

        [MaxLength(500)]
        [Column("Description")]
        public string? Description { get; set; }

        [Column("DisplayOrder")]
        public int DisplayOrder { get; set; } = 0;

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Required]
        [Column("CreatedAt", TypeName = "DATETIME2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt", TypeName = "DATETIME2")]
        public DateTime? UpdatedAt { get; set; }

        [MaxLength(50)]
        [Column("Visual_Stage_Key")]
        public string? VisualStageKey { get; set; }

        // Navigation properties
        public virtual ICollection<WorkflowAuditHistory> FromStatusAudits { get; set; } = new List<WorkflowAuditHistory>();
        public virtual ICollection<WorkflowAuditHistory> ToStatusAudits { get; set; } = new List<WorkflowAuditHistory>();
    }
}
