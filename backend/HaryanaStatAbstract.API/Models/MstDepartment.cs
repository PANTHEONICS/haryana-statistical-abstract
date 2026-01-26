using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaryanaStatAbstract.API.Models
{
    /// <summary>
    /// Master table for Departments
    /// </summary>
    [Table("Mst_Departments")]
    public class MstDepartment
    {
        [Key]
        [Column("DepartmentID")]
        public int DepartmentID { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("DepartmentName")]
        public string DepartmentName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("DepartmentCode")]
        public string DepartmentCode { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<MasterUser> Users { get; set; } = new List<MasterUser>();
    }
}
