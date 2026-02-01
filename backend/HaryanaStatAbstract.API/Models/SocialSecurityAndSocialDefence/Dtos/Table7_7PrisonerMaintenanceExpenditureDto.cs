using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence.Dtos
{
    public class Table7_7PrisonerMaintenanceExpenditureDto
    {
        public int Id { get; set; }
        public string Year { get; set; } = string.Empty;
        [JsonPropertyName("avg_Prisoners")] public long Avg_Prisoners { get; set; }
        [JsonPropertyName("exp_Establishment")] public long Exp_Establishment { get; set; }
        [JsonPropertyName("exp_Diet")] public long Exp_Diet { get; set; }
        [JsonPropertyName("exp_Others")] public long Exp_Others { get; set; }
        [JsonPropertyName("exp_Total")] public long Exp_Total { get; set; }
        [JsonPropertyName("cost_Per_Prisoner")] public long Cost_Per_Prisoner { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public int? CreatedBy { get; set; }
        public string? CreatedByIPAddress { get; set; }
        public int? ModifiedBy { get; set; }
        public string? ModifiedByIPAddress { get; set; }
        public string? CreatedByUserName { get; set; }
        public string? ModifiedByUserName { get; set; }
    }

    public class CreateTable7_7PrisonerMaintenanceExpenditureDto
    {
        [Required][MaxLength(20)] public string Year { get; set; } = string.Empty;
        [JsonPropertyName("avg_Prisoners")][Required][Range(0, long.MaxValue)] public long Avg_Prisoners { get; set; }
        [JsonPropertyName("exp_Establishment")][Required][Range(0, long.MaxValue)] public long Exp_Establishment { get; set; }
        [JsonPropertyName("exp_Diet")][Required][Range(0, long.MaxValue)] public long Exp_Diet { get; set; }
        [JsonPropertyName("exp_Others")][Required][Range(0, long.MaxValue)] public long Exp_Others { get; set; }
    }

    public class UpdateTable7_7PrisonerMaintenanceExpenditureDto
    {
        [MaxLength(20)] public string? Year { get; set; }
        [JsonPropertyName("avg_Prisoners")][Range(0, long.MaxValue)] public long? Avg_Prisoners { get; set; }
        [JsonPropertyName("exp_Establishment")][Range(0, long.MaxValue)] public long? Exp_Establishment { get; set; }
        [JsonPropertyName("exp_Diet")][Range(0, long.MaxValue)] public long? Exp_Diet { get; set; }
        [JsonPropertyName("exp_Others")][Range(0, long.MaxValue)] public long? Exp_Others { get; set; }
    }
}
