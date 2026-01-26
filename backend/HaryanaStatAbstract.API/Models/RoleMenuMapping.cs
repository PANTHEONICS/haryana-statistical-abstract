using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaryanaStatAbstract.API.Models
{
    /// <summary>
    /// Mapping table for Role and Menu access (Special role-based access)
    /// </summary>
    [Table("Mst_Role_Menu_Mapping")]
    public class RoleMenuMapping
    {
        [Key]
        [Column("MappingID")]
        public int MappingID { get; set; }

        [Required]
        [Column("RoleID")]
        public int RoleID { get; set; }

        [Required]
        [Column("MenuID")]
        public int MenuID { get; set; }

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Required]
        [Column("CreatedAt", TypeName = "DATETIME2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("CreatedBy")]
        public int? CreatedBy { get; set; }

        [Column("UpdatedAt", TypeName = "DATETIME2")]
        public DateTime? UpdatedAt { get; set; }

        [Column("UpdatedBy")]
        public int? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey("RoleID")]
        public virtual MstRole Role { get; set; } = null!;

        [ForeignKey("MenuID")]
        public virtual MstMenu Menu { get; set; } = null!;

        [ForeignKey("CreatedBy")]
        public virtual MasterUser? Creator { get; set; }

        [ForeignKey("UpdatedBy")]
        public virtual MasterUser? Updater { get; set; }
    }
}
