using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HaryanaStatAbstract.API.Models;

namespace HaryanaStatAbstract.API.Models.AreaAndPopulation
{
    /// <summary>
    /// Represents census population data for Haryana (1901-2011 Census)
    /// Source: Statistical Abstract of Haryana 2023-24, Table 3.2
    /// Department: Area & Population
    /// </summary>
    [Table("AP_Table_3_2_CensusPopulation")]
    public class Table3_2CensusPopulation
    {
        /// <summary>
        /// Unique identifier for census population record (Primary Key)
        /// </summary>
        [Key]
        [Column("CensusID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CensusID { get; set; }

        /// <summary>
        /// Census year (Unique Constraint)
        /// Decennial years: 1901-2011
        /// </summary>
        [Column("census_year")]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
        public int Year { get; set; }

        /// <summary>
        /// Total population count
        /// </summary>
        [Required]
        [Column("total_population")]
        [Range(1, long.MaxValue, ErrorMessage = "Total population must be greater than 0")]
        public long TotalPopulation { get; set; }

        /// <summary>
        /// Variation in population from previous census
        /// Can be negative (decrease) or positive (increase)
        /// NULL for first record (1901)
        /// </summary>
        [Column("variation_in_population")]
        public long? VariationInPopulation { get; set; }

        /// <summary>
        /// Decennial percentage increase
        /// Can be negative (decrease) or positive (increase)
        /// NULL for first record (1901)
        /// </summary>
        [Column("decennial_percentage_increase", TypeName = "decimal(5,2)")]
        public decimal? DecennialPercentageIncrease { get; set; }

        /// <summary>
        /// Male population count
        /// </summary>
        [Required]
        [Column("male_population")]
        [Range(1, long.MaxValue, ErrorMessage = "Male population must be greater than 0")]
        public long MalePopulation { get; set; }

        /// <summary>
        /// Female population count
        /// </summary>
        [Required]
        [Column("female_population")]
        [Range(1, long.MaxValue, ErrorMessage = "Female population must be greater than 0")]
        public long FemalePopulation { get; set; }

        /// <summary>
        /// Sex ratio (females per 1000 males)
        /// Typically ranges from 800-1000
        /// </summary>
        [Required]
        [Column("sex_ratio")]
        [Range(0, 2000, ErrorMessage = "Sex ratio must be between 0 and 2000")]
        public int SexRatio { get; set; }

        /// <summary>
        /// Timestamp when record was created
        /// </summary>
        [Column("CreatedDateTime")]
        public DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// Timestamp when record was last modified
        /// </summary>
        [Column("ModifiedDateTime")]
        public DateTime ModifiedDateTime { get; set; }

        /// <summary>
        /// User ID who created the record
        /// </summary>
        [Column("CreatedBy")]
        public int? CreatedBy { get; set; }

        /// <summary>
        /// IP Address from which the record was created
        /// </summary>
        [Column("CreatedByIPAddress")]
        [MaxLength(50)]
        public string? CreatedByIPAddress { get; set; }

        /// <summary>
        /// User ID who last modified the record
        /// </summary>
        [Column("ModifiedBy")]
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// IP Address from which the record was last modified
        /// </summary>
        [Column("ModifiedByIPAddress")]
        [MaxLength(50)]
        public string? ModifiedByIPAddress { get; set; }

        // Navigation properties
        [ForeignKey("CreatedBy")]
        public virtual MasterUser? CreatedByUser { get; set; }

        [ForeignKey("ModifiedBy")]
        public virtual MasterUser? ModifiedByUser { get; set; }
    }
}
