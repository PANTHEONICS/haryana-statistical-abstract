using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HaryanaStatAbstract.API.Models;

namespace HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence
{
    [Table("SSD_Table_7_8_Jail_Industry_Production_Progress")]
    public class Table7_8JailIndustryProductionProgress
    {
        [Key]
        [Column("Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("Year")]
        [Required]
        [MaxLength(20)]
        public string Year { get; set; } = string.Empty;

        [Column("Carpentry")]
        public long Carpentry { get; set; }
        [Column("Textile")]
        public long Textile { get; set; }
        [Column("Leather")]
        public long Leather { get; set; }
        [Column("Durries")]
        public long Durries { get; set; }
        [Column("Tailoring")]
        public long Tailoring { get; set; }
        [Column("Munj")]
        public long Munj { get; set; }
        [Column("Chicks")]
        public long Chicks { get; set; }
        [Column("Oil_OilCake")]
        public long Oil_OilCake { get; set; }
        [Column("Tents")]
        public long Tents { get; set; }
        [Column("Blankets")]
        public long Blankets { get; set; }
        [Column("Smithy")]
        public long Smithy { get; set; }
        [Column("Niwar_Tapes")]
        public long Niwar_Tapes { get; set; }
        [Column("Misc")]
        public long Misc { get; set; }
        [Column("Total")]
        public long Total { get; set; }

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
