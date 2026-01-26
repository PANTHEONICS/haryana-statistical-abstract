using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaryanaStatAbstract.API.Models
{
    /// <summary>
    /// Master User table - Transaction table for user management
    /// </summary>
    [Table("Master_User")]
    public class MasterUser
    {
        [Key]
        [Column("UserID")]
        public int UserID { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("LoginID")]
        public string LoginID { get; set; } = string.Empty;

        [Required]
        [Column("UserPassword", TypeName = "NVARCHAR(MAX)")]
        public string UserPassword { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        [Column("UserMobileNo")]
        public string UserMobileNo { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("UserEmailID")]
        public string? UserEmailID { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("FullName")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Column("RoleID")]
        public int RoleID { get; set; }

        [Column("DepartmentID")]
        public int? DepartmentID { get; set; }

        [Column("SecurityQuestionID")]
        public int? SecurityQuestionID { get; set; }

        [MaxLength(100)]
        [Column("SecurityQuestionAnswer")]
        public string? SecurityQuestionAnswer { get; set; }

        [MaxLength(100)]
        [Column("LoggedInSessionID")]
        public string? LoggedInSessionID { get; set; }

        [Column("CurrentLoginDateTime", TypeName = "DATETIME2")]
        public DateTime? CurrentLoginDateTime { get; set; }

        [Column("LastLoginDateTime", TypeName = "DATETIME2")]
        public DateTime? LastLoginDateTime { get; set; }

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("RoleID")]
        public virtual MstRole Role { get; set; } = null!;

        [ForeignKey("DepartmentID")]
        public virtual MstDepartment? Department { get; set; }

        [ForeignKey("SecurityQuestionID")]
        public virtual MstSecurityQuestion? SecurityQuestion { get; set; }
    }
}
