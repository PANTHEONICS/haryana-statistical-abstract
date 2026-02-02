using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence.Dtos
{
    public class Table7_8JailIndustryProductionProgressDto
    {
        public int Id { get; set; }
        public string Year { get; set; } = string.Empty;
        [JsonPropertyName("carpentry")] public long Carpentry { get; set; }
        [JsonPropertyName("textile")] public long Textile { get; set; }
        [JsonPropertyName("leather")] public long Leather { get; set; }
        [JsonPropertyName("durries")] public long Durries { get; set; }
        [JsonPropertyName("tailoring")] public long Tailoring { get; set; }
        [JsonPropertyName("munj")] public long Munj { get; set; }
        [JsonPropertyName("chicks")] public long Chicks { get; set; }
        [JsonPropertyName("oil_OilCake")] public long Oil_OilCake { get; set; }
        [JsonPropertyName("tents")] public long Tents { get; set; }
        [JsonPropertyName("blankets")] public long Blankets { get; set; }
        [JsonPropertyName("smithy")] public long Smithy { get; set; }
        [JsonPropertyName("niwar_Tapes")] public long Niwar_Tapes { get; set; }
        [JsonPropertyName("misc")] public long Misc { get; set; }
        public long Total { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public int? CreatedBy { get; set; }
        public string? CreatedByIPAddress { get; set; }
        public int? ModifiedBy { get; set; }
        public string? ModifiedByIPAddress { get; set; }
        public string? CreatedByUserName { get; set; }
        public string? ModifiedByUserName { get; set; }
    }

    public class CreateTable7_8JailIndustryProductionProgressDto
    {
        [Required][MaxLength(20)] public string Year { get; set; } = string.Empty;
        [JsonPropertyName("carpentry")][Range(0, long.MaxValue)] public long Carpentry { get; set; }
        [JsonPropertyName("textile")][Range(0, long.MaxValue)] public long Textile { get; set; }
        [JsonPropertyName("leather")][Range(0, long.MaxValue)] public long Leather { get; set; }
        [JsonPropertyName("durries")][Range(0, long.MaxValue)] public long Durries { get; set; }
        [JsonPropertyName("tailoring")][Range(0, long.MaxValue)] public long Tailoring { get; set; }
        [JsonPropertyName("munj")][Range(0, long.MaxValue)] public long Munj { get; set; }
        [JsonPropertyName("chicks")][Range(0, long.MaxValue)] public long Chicks { get; set; }
        [JsonPropertyName("oil_OilCake")][Range(0, long.MaxValue)] public long Oil_OilCake { get; set; }
        [JsonPropertyName("tents")][Range(0, long.MaxValue)] public long Tents { get; set; }
        [JsonPropertyName("blankets")][Range(0, long.MaxValue)] public long Blankets { get; set; }
        [JsonPropertyName("smithy")][Range(0, long.MaxValue)] public long Smithy { get; set; }
        [JsonPropertyName("niwar_Tapes")][Range(0, long.MaxValue)] public long Niwar_Tapes { get; set; }
        [JsonPropertyName("misc")][Range(0, long.MaxValue)] public long Misc { get; set; }
    }

    public class UpdateTable7_8JailIndustryProductionProgressDto
    {
        [MaxLength(20)] public string? Year { get; set; }
        [JsonPropertyName("carpentry")][Range(0, long.MaxValue)] public long? Carpentry { get; set; }
        [JsonPropertyName("textile")][Range(0, long.MaxValue)] public long? Textile { get; set; }
        [JsonPropertyName("leather")][Range(0, long.MaxValue)] public long? Leather { get; set; }
        [JsonPropertyName("durries")][Range(0, long.MaxValue)] public long? Durries { get; set; }
        [JsonPropertyName("tailoring")][Range(0, long.MaxValue)] public long? Tailoring { get; set; }
        [JsonPropertyName("munj")][Range(0, long.MaxValue)] public long? Munj { get; set; }
        [JsonPropertyName("chicks")][Range(0, long.MaxValue)] public long? Chicks { get; set; }
        [JsonPropertyName("oil_OilCake")][Range(0, long.MaxValue)] public long? Oil_OilCake { get; set; }
        [JsonPropertyName("tents")][Range(0, long.MaxValue)] public long? Tents { get; set; }
        [JsonPropertyName("blankets")][Range(0, long.MaxValue)] public long? Blankets { get; set; }
        [JsonPropertyName("smithy")][Range(0, long.MaxValue)] public long? Smithy { get; set; }
        [JsonPropertyName("niwar_Tapes")][Range(0, long.MaxValue)] public long? Niwar_Tapes { get; set; }
        [JsonPropertyName("misc")][Range(0, long.MaxValue)] public long? Misc { get; set; }
    }
}
