using System.ComponentModel.DataAnnotations;

namespace HaryanaStatAbstract.API.Models.AreaAndPopulation.Dtos
{
    /// <summary>
    /// Data Transfer Object for Census Population
    /// Used for API requests and responses
    /// </summary>
    public class Table3_2CensusPopulationDto
    {
        public int CensusID { get; set; }
        public int Year { get; set; }
        public long TotalPopulation { get; set; }
        public long? VariationInPopulation { get; set; }
        public decimal? DecennialPercentageIncrease { get; set; }
        public long MalePopulation { get; set; }
        public long FemalePopulation { get; set; }
        public int SexRatio { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public int? CreatedBy { get; set; }
        public string? CreatedByIPAddress { get; set; }
        public int? ModifiedBy { get; set; }
        public string? ModifiedByIPAddress { get; set; }
        public string? CreatedByUserName { get; set; }
        public string? ModifiedByUserName { get; set; }
    }

    /// <summary>
    /// DTO for creating a new census population record
    /// </summary>
    public class CreateTable3_2CensusPopulationDto
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
    }

    /// <summary>
    /// DTO for updating an existing census population record
    /// </summary>
    public class UpdateTable3_2CensusPopulationDto
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
    }
}
