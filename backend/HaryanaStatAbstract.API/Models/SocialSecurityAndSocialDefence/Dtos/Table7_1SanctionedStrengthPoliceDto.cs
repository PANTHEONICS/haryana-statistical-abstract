using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence.Dtos
{
    public class Table7_1SanctionedStrengthPoliceDto
    {
        public int Id { get; set; }
        public int Year { get; set; }
        [JsonPropertyName("dg_ADG_IG_DyIG")] public int DG_ADG_IG_DyIG { get; set; }
        [JsonPropertyName("asst_IG")] public int Asst_IG { get; set; }
        [JsonPropertyName("superintendents_Addl_Dy_Asst")] public int Superintendents_Addl_Dy_Asst { get; set; }
        [JsonPropertyName("inspectors_SI_ASI")] public int Inspectors_SI_ASI { get; set; }
        [JsonPropertyName("head_Constables_RC")] public int Head_Constables_RC { get; set; }
        [JsonPropertyName("mounted_Foot_Constables")] public int Mounted_Foot_Constables { get; set; }
        public int Total { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public int? CreatedBy { get; set; }
        public string? CreatedByIPAddress { get; set; }
        public int? ModifiedBy { get; set; }
        public string? ModifiedByIPAddress { get; set; }
        public string? CreatedByUserName { get; set; }
        public string? ModifiedByUserName { get; set; }
    }

    public class CreateTable7_1SanctionedStrengthPoliceDto
    {
        [Required][Range(1900, 2100)] public int Year { get; set; }
        [JsonPropertyName("dg_ADG_IG_DyIG")][Required][Range(0, int.MaxValue)] public int DG_ADG_IG_DyIG { get; set; }
        [JsonPropertyName("asst_IG")][Required][Range(0, int.MaxValue)] public int Asst_IG { get; set; }
        [JsonPropertyName("superintendents_Addl_Dy_Asst")][Required][Range(0, int.MaxValue)] public int Superintendents_Addl_Dy_Asst { get; set; }
        [JsonPropertyName("inspectors_SI_ASI")][Required][Range(0, int.MaxValue)] public int Inspectors_SI_ASI { get; set; }
        [JsonPropertyName("head_Constables_RC")][Required][Range(0, int.MaxValue)] public int Head_Constables_RC { get; set; }
        [JsonPropertyName("mounted_Foot_Constables")][Required][Range(0, int.MaxValue)] public int Mounted_Foot_Constables { get; set; }
    }

    public class UpdateTable7_1SanctionedStrengthPoliceDto
    {
        [Range(1900, 2100)] public int? Year { get; set; }
        [JsonPropertyName("dg_ADG_IG_DyIG")][Range(0, int.MaxValue)] public int? DG_ADG_IG_DyIG { get; set; }
        [JsonPropertyName("asst_IG")][Range(0, int.MaxValue)] public int? Asst_IG { get; set; }
        [JsonPropertyName("superintendents_Addl_Dy_Asst")][Range(0, int.MaxValue)] public int? Superintendents_Addl_Dy_Asst { get; set; }
        [JsonPropertyName("inspectors_SI_ASI")][Range(0, int.MaxValue)] public int? Inspectors_SI_ASI { get; set; }
        [JsonPropertyName("head_Constables_RC")][Range(0, int.MaxValue)] public int? Head_Constables_RC { get; set; }
        [JsonPropertyName("mounted_Foot_Constables")][Range(0, int.MaxValue)] public int? Mounted_Foot_Constables { get; set; }
    }
}
