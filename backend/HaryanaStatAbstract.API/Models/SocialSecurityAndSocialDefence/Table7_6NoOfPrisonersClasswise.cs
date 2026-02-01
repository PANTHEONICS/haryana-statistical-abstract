using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HaryanaStatAbstract.API.Models;

namespace HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence
{
    [Table("SSD_Table_7_6_NoOfPrisoners_Classwise")]
    public class Table7_6NoOfPrisonersClasswise
    {
        [Key]
        [Column("Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("Year")]
        [Required]
        [MaxLength(20)]
        public string Year { get; set; } = string.Empty;

        [Column("Beg_Convicted")]
        public int Beg_Convicted { get; set; }

        [Column("Beg_UnderTrial")]
        public int Beg_UnderTrial { get; set; }

        [Column("Beg_Civil")]
        public int Beg_Civil { get; set; }

        [Column("Adm_Convicted")]
        public int Adm_Convicted { get; set; }

        [Column("Adm_UnderTrial")]
        public int Adm_UnderTrial { get; set; }

        [Column("Adm_Civil")]
        public int Adm_Civil { get; set; }

        [Column("Dis_Convicted")]
        public int Dis_Convicted { get; set; }

        [Column("Dis_UnderTrial")]
        public int Dis_UnderTrial { get; set; }

        [Column("Dis_Civil")]
        public int Dis_Civil { get; set; }

        [Column("Rem_Convicted")]
        public int Rem_Convicted { get; set; }

        [Column("Rem_UnderTrial")]
        public int Rem_UnderTrial { get; set; }

        [Column("Rem_Civil")]
        public int Rem_Civil { get; set; }

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
