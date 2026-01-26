using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaryanaStatAbstract.API.Models
{
    /// <summary>
    /// Screen-Level Workflow Management
    /// Tracks workflow status at the screen level, not individual records
    /// </summary>
    [Table("Workflow_Screen")]
    public class ScreenWorkflow
    {
        [Key]
        [Column("ScreenWorkflowID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ScreenWorkflowID { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("ScreenName")]
        public string ScreenName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("ScreenCode")]
        public string ScreenCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("TableName")]
        public string TableName { get; set; } = string.Empty;

        [Required]
        [Column("CurrentStatusID")]
        public int CurrentStatusID { get; set; } = 1; // Default to Draft

        [MaxLength(100)]
        [Column("CurrentStatusName")]
        public string? CurrentStatusName { get; set; }

        [Required]
        [Column("CreatedByUserID")]
        public int CreatedByUserID { get; set; }

        [MaxLength(100)]
        [Column("CreatedByUserName")]
        public string? CreatedByUserName { get; set; }

        [Required]
        [Column("CreatedAt", TypeName = "DATETIME2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt", TypeName = "DATETIME2")]
        public DateTime? UpdatedAt { get; set; }

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("CurrentStatusID")]
        public virtual MstWorkflowStatus WorkflowStatus { get; set; } = null!;

        [ForeignKey("CreatedByUserID")]
        public virtual MasterUser CreatedByUser { get; set; } = null!;
    }
}
