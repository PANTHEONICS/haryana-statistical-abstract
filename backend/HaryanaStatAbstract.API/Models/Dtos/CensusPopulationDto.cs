using System.ComponentModel.DataAnnotations;

namespace HaryanaStatAbstract.API.Models.Dtos
{
    /// <summary>
    /// Data Transfer Object for Census Population
    /// Used for API requests and responses
    /// </summary>
    public class CensusPopulationDto
    {
        public int CensusID { get; set; }
        public int Year { get; set; }
        public long TotalPopulation { get; set; }
        public long? VariationInPopulation { get; set; }
        public decimal? DecennialPercentageIncrease { get; set; }
        public long MalePopulation { get; set; }
        public long FemalePopulation { get; set; }
        public int SexRatio { get; set; }
        public string? SourceDocument { get; set; }
        public string? SourceTable { get; set; }
        public string? SourceReference { get; set; }
    }

    /// <summary>
    /// DTO for creating a new census population record
    /// </summary>
    public class CreateCensusPopulationDto
    {
        [Required]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
        public int Year { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Total population must be greater than 0")]
        public long TotalPopulation { get; set; }

        public long? VariationInPopulation { get; set; }

        [Range(-100, 1000, ErrorMessage = "Decennial percentage increase must be between -100 and 1000")]
        public decimal? DecennialPercentageIncrease { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Male population must be greater than 0")]
        public long MalePopulation { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Female population must be greater than 0")]
        public long FemalePopulation { get; set; }

        [Required]
        [Range(0, 2000, ErrorMessage = "Sex ratio must be between 0 and 2000")]
        public int SexRatio { get; set; }

        [MaxLength(255)]
        public string? SourceDocument { get; set; }

        [MaxLength(50)]
        public string? SourceTable { get; set; }

        [MaxLength(500)]
        public string? SourceReference { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing census population record
    /// </summary>
    public class UpdateCensusPopulationDto
    {
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
        public int? Year { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = "Total population must be greater than 0")]
        public long? TotalPopulation { get; set; }

        public long? VariationInPopulation { get; set; }

        [Range(-100, 1000, ErrorMessage = "Decennial percentage increase must be between -100 and 1000")]
        public decimal? DecennialPercentageIncrease { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = "Male population must be greater than 0")]
        public long? MalePopulation { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = "Female population must be greater than 0")]
        public long? FemalePopulation { get; set; }

        [Range(0, 2000, ErrorMessage = "Sex ratio must be between 0 and 2000")]
        public int? SexRatio { get; set; }

        [MaxLength(255)]
        public string? SourceDocument { get; set; }

        [MaxLength(50)]
        public string? SourceTable { get; set; }

        [MaxLength(500)]
        public string? SourceReference { get; set; }
    }
}