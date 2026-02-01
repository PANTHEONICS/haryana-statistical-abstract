using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaryanaStatAbstract.API.Models
{
    [Table("Mst_ScreenRegistry")]
    public class MstScreenRegistry
    {
        [Key]
        [Column("ScreenRegistryID")]
        public int ScreenRegistryID { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("ScreenCode")]
        public string ScreenCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [Column("ScreenName")]
        public string ScreenName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("TableName")]
        public string TableName { get; set; } = string.Empty;

        [Required]
        [Column("DepartmentID")]
        public int DepartmentID { get; set; }

        [ForeignKey("DepartmentID")]
        public virtual MstDepartment Department { get; set; } = null!;
    }
}
