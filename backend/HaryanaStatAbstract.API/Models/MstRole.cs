using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaryanaStatAbstract.API.Models
{
    /// <summary>
    /// Master table for User Roles
    /// </summary>
    [Table("Mst_Roles")]
    public class MstRole
    {
        [Key]
        [Column("RoleID")]
        public int RoleID { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("RoleName")]
        public string RoleName { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<MasterUser> Users { get; set; } = new List<MasterUser>();
    }
}
