using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HaryanaStatAbstract.API.Models;

namespace HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence
{
    [Table("SSD_Table_7_1_Sanctioned_strength_Police")]
    public class Table7_1SanctionedStrengthPolice
    {
        [Key]
        [Column("Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("Year")]
        [Range(1900, 2100)]
        public int Year { get; set; }

        [Column("DG_ADG_IG_DyIG")]
        public int DG_ADG_IG_DyIG { get; set; }

        [Column("Asst_IG")]
        public int Asst_IG { get; set; }

        [Column("Superintendents_Addl_Dy_Asst")]
        public int Superintendents_Addl_Dy_Asst { get; set; }

        [Column("Inspectors_SI_ASI")]
        public int Inspectors_SI_ASI { get; set; }

        [Column("Head_Constables_RC")]
        public int Head_Constables_RC { get; set; }

        [Column("Mounted_Foot_Constables")]
        public int Mounted_Foot_Constables { get; set; }

        [Column("Total")]
        public int Total { get; set; }

        [Column("CreatedBy")]
        public int? CreatedBy { get; set; }

        [Column("CreatedByIPAddress")]
        [MaxLength(50)]
        public string? CreatedByIPAddress { get; set; }

        [Column("CreatedDateTime")]
        public DateTime CreatedDateTime { get; set; }

        [Column("ModifiedBy")]
        public int? ModifiedBy { get; set; }

        [Column("ModifiedByIPAddress")]
        [MaxLength(50)]
        public string? ModifiedByIPAddress { get; set; }

        [Column("ModifiedDateTime")]
        public DateTime ModifiedDateTime { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual MasterUser? CreatedByUser { get; set; }

        [ForeignKey("ModifiedBy")]
        public virtual MasterUser? ModifiedByUser { get; set; }
    }
}
