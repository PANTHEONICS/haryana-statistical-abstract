using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HaryanaStatAbstract.API.Models;

namespace HaryanaStatAbstract.API.Models.Education
{
    /// <summary>
    /// Represents data for Table 6.1: Number of recognised universities/colleges/schools in Haryana
    /// Source: Statistical Abstract of Haryana 2023-24, Table 6.1
    /// Department: Education
    /// Table Name: Ed_Table_6_1_Institutions
    /// </summary>
    [Table("Ed_Table_6_1_Institutions")]
    public class Table6_1Institutions
    {
        /// <summary>
        /// Unique identifier for institution record (Primary Key)
        /// </summary>
        [Key]
        [Column("InstitutionID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InstitutionID { get; set; }

        /// <summary>
        /// Type of institution (Unique Constraint)
        /// Examples: Universities, Arts and Science Colleges, Engineering Colleges, etc.
        /// </summary>
        [Required]
        [Column("InstitutionType")]
        [MaxLength(200)]
        public string InstitutionType { get; set; } = string.Empty;

        /// <summary>
        /// Count for year 1966-67
        /// </summary>
        [Column("Year_1966_67")]
        public int? Year196667 { get; set; }

        /// <summary>
        /// Count for year 1970-71
        /// </summary>
        [Column("Year_1970_71")]
        public int? Year197071 { get; set; }

        /// <summary>
        /// Count for year 1980-81
        /// </summary>
        [Column("Year_1980_81")]
        public int? Year198081 { get; set; }

        /// <summary>
        /// Count for year 1990-91
        /// </summary>
        [Column("Year_1990_91")]
        public int? Year199091 { get; set; }

        /// <summary>
        /// Count for year 2000-01
        /// </summary>
        [Column("Year_2000_01")]
        public int? Year200001 { get; set; }

        /// <summary>
        /// Count for year 2010-11
        /// </summary>
        [Column("Year_2010_11")]
        public int? Year201011 { get; set; }

        /// <summary>
        /// Count for year 2016-17
        /// </summary>
        [Column("Year_2016_17")]
        public int? Year201617 { get; set; }

        /// <summary>
        /// Count for year 2017-18
        /// </summary>
        [Column("Year_2017_18")]
        public int? Year201718 { get; set; }

        /// <summary>
        /// Count for year 2018-19
        /// </summary>
        [Column("Year_2018_19")]
        public int? Year201819 { get; set; }

        /// <summary>
        /// Count for year 2019-20
        /// </summary>
        [Column("Year_2019_20")]
        public int? Year201920 { get; set; }

        /// <summary>
        /// Count for year 2020-21
        /// </summary>
        [Column("Year_2020_21")]
        public int? Year202021 { get; set; }

        /// <summary>
        /// Count for year 2021-22
        /// </summary>
        [Column("Year_2021_22")]
        public int? Year202122 { get; set; }

        /// <summary>
        /// Count for year 2022-23
        /// </summary>
        [Column("Year_2022_23")]
        public int? Year202223 { get; set; }

        /// <summary>
        /// Count for year 2023-24 (Provisional)
        /// </summary>
        [Column("Year_2023_24")]
        public int? Year202324 { get; set; }

        /// <summary>
        /// Count for year 2024-25
        /// </summary>
        [Column("Year_2024_25")]
        public int? Year202425 { get; set; }

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
