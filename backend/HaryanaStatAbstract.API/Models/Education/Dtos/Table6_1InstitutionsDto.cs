using System.ComponentModel.DataAnnotations;

namespace HaryanaStatAbstract.API.Models.Education.Dtos
{
    /// <summary>
    /// Data Transfer Object for Table 6.1 Institutions
    /// Used for API requests and responses
    /// </summary>
    public class Table6_1InstitutionsDto
    {
        public int InstitutionID { get; set; }
        public string InstitutionType { get; set; } = string.Empty;
        public int? Year196667 { get; set; }
        public int? Year197071 { get; set; }
        public int? Year198081 { get; set; }
        public int? Year199091 { get; set; }
        public int? Year200001 { get; set; }
        public int? Year201011 { get; set; }
        public int? Year201617 { get; set; }
        public int? Year201718 { get; set; }
        public int? Year201819 { get; set; }
        public int? Year201920 { get; set; }
        public int? Year202021 { get; set; }
        public int? Year202122 { get; set; }
        public int? Year202223 { get; set; }
        public int? Year202324 { get; set; }
        public int? Year202425 { get; set; }
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
    /// DTO for creating a new institution record
    /// </summary>
    public class CreateTable6_1InstitutionsDto
    {
        [Required]
        [MaxLength(200)]
        public string InstitutionType { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year196667 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year197071 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year198081 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year199091 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year200001 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year201011 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year201617 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year201718 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year201819 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year201920 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year202021 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year202122 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year202223 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year202324 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year202425 { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing institution record
    /// </summary>
    public class UpdateTable6_1InstitutionsDto
    {
        [MaxLength(200)]
        public string? InstitutionType { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year196667 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year197071 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year198081 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year199091 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year200001 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year201011 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year201617 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year201718 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year201819 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year201920 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year202021 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year202122 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year202223 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year202324 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0")]
        public int? Year202425 { get; set; }
    }
}
