using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaryanaStatAbstract.API.Models
{
    /// <summary>
    /// Master table for Menus
    /// </summary>
    [Table("Mst_Menus")]
    public class MstMenu
    {
        [Key]
        [Column("MenuID")]
        public int MenuID { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("MenuName")]
        public string MenuName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [Column("MenuPath")]
        public string MenuPath { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column("MenuIcon")]
        public string? MenuIcon { get; set; }

        [Column("ParentMenuID")]
        public int? ParentMenuID { get; set; }

        [Required]
        [Column("DisplayOrder")]
        public int DisplayOrder { get; set; } = 0;

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Required]
        [Column("IsAdminOnly")]
        public bool IsAdminOnly { get; set; } = false;

        [MaxLength(500)]
        [Column("MenuDescription")]
        public string? MenuDescription { get; set; }

        [Required]
        [Column("CreatedAt", TypeName = "DATETIME2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt", TypeName = "DATETIME2")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ParentMenuID")]
        public virtual MstMenu? ParentMenu { get; set; }

        public virtual ICollection<MstMenu> ChildMenus { get; set; } = new List<MstMenu>();
        public virtual ICollection<DepartmentMenuMapping> DepartmentMenuMappings { get; set; } = new List<DepartmentMenuMapping>();
        public virtual ICollection<RoleMenuMapping> RoleMenuMappings { get; set; } = new List<RoleMenuMapping>();
    }
}
