using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HaryanaStatAbstract.API.Models;

namespace HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence
{
    [Table("SSD_Table_7_7_PrisonerMaintenance_Expenditure")]
    public class Table7_7PrisonerMaintenanceExpenditure
    {
        [Key]
        [Column("Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("Year")]
        [Required]
        [MaxLength(20)]
        public string Year { get; set; } = string.Empty;

        [Column("Avg_Prisoners")]
        public long Avg_Prisoners { get; set; }

        [Column("Exp_Establishment")]
        public long Exp_Establishment { get; set; }

        [Column("Exp_Diet")]
        public long Exp_Diet { get; set; }

        [Column("Exp_Others")]
        public long Exp_Others { get; set; }

        [Column("Exp_Total")]
        public long Exp_Total { get; set; }

        [Column("Cost_Per_Prisoner")]
        public long Cost_Per_Prisoner { get; set; }

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
