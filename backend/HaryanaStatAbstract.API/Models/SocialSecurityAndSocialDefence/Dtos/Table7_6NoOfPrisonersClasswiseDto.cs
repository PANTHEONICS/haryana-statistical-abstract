using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence.Dtos
{
    public class Table7_6NoOfPrisonersClasswiseDto
    {
        public int Id { get; set; }
        public string Year { get; set; } = string.Empty;
        [JsonPropertyName("beg_Convicted")] public int Beg_Convicted { get; set; }
        [JsonPropertyName("beg_UnderTrial")] public int Beg_UnderTrial { get; set; }
        [JsonPropertyName("beg_Civil")] public int Beg_Civil { get; set; }
        [JsonPropertyName("adm_Convicted")] public int Adm_Convicted { get; set; }
        [JsonPropertyName("adm_UnderTrial")] public int Adm_UnderTrial { get; set; }
        [JsonPropertyName("adm_Civil")] public int Adm_Civil { get; set; }
        [JsonPropertyName("dis_Convicted")] public int Dis_Convicted { get; set; }
        [JsonPropertyName("dis_UnderTrial")] public int Dis_UnderTrial { get; set; }
        [JsonPropertyName("dis_Civil")] public int Dis_Civil { get; set; }
        [JsonPropertyName("rem_Convicted")] public int Rem_Convicted { get; set; }
        [JsonPropertyName("rem_UnderTrial")] public int Rem_UnderTrial { get; set; }
        [JsonPropertyName("rem_Civil")] public int Rem_Civil { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public int? CreatedBy { get; set; }
        public string? CreatedByIPAddress { get; set; }
        public int? ModifiedBy { get; set; }
        public string? ModifiedByIPAddress { get; set; }
        public string? CreatedByUserName { get; set; }
        public string? ModifiedByUserName { get; set; }
    }

    public class CreateTable7_6NoOfPrisonersClasswiseDto
    {
        [Required][MaxLength(20)] public string Year { get; set; } = string.Empty;
        [JsonPropertyName("beg_Convicted")][Required][Range(0, int.MaxValue)] public int Beg_Convicted { get; set; }
        [JsonPropertyName("beg_UnderTrial")][Required][Range(0, int.MaxValue)] public int Beg_UnderTrial { get; set; }
        [JsonPropertyName("beg_Civil")][Required][Range(0, int.MaxValue)] public int Beg_Civil { get; set; }
        [JsonPropertyName("adm_Convicted")][Required][Range(0, int.MaxValue)] public int Adm_Convicted { get; set; }
        [JsonPropertyName("adm_UnderTrial")][Required][Range(0, int.MaxValue)] public int Adm_UnderTrial { get; set; }
        [JsonPropertyName("adm_Civil")][Required][Range(0, int.MaxValue)] public int Adm_Civil { get; set; }
        [JsonPropertyName("dis_Convicted")][Required][Range(0, int.MaxValue)] public int Dis_Convicted { get; set; }
        [JsonPropertyName("dis_UnderTrial")][Required][Range(0, int.MaxValue)] public int Dis_UnderTrial { get; set; }
        [JsonPropertyName("dis_Civil")][Required][Range(0, int.MaxValue)] public int Dis_Civil { get; set; }
    }

    public class UpdateTable7_6NoOfPrisonersClasswiseDto
    {
        [MaxLength(20)] public string? Year { get; set; }
        [JsonPropertyName("beg_Convicted")][Range(0, int.MaxValue)] public int? Beg_Convicted { get; set; }
        [JsonPropertyName("beg_UnderTrial")][Range(0, int.MaxValue)] public int? Beg_UnderTrial { get; set; }
        [JsonPropertyName("beg_Civil")][Range(0, int.MaxValue)] public int? Beg_Civil { get; set; }
        [JsonPropertyName("adm_Convicted")][Range(0, int.MaxValue)] public int? Adm_Convicted { get; set; }
        [JsonPropertyName("adm_UnderTrial")][Range(0, int.MaxValue)] public int? Adm_UnderTrial { get; set; }
        [JsonPropertyName("adm_Civil")][Range(0, int.MaxValue)] public int? Adm_Civil { get; set; }
        [JsonPropertyName("dis_Convicted")][Range(0, int.MaxValue)] public int? Dis_Convicted { get; set; }
        [JsonPropertyName("dis_UnderTrial")][Range(0, int.MaxValue)] public int? Dis_UnderTrial { get; set; }
        [JsonPropertyName("dis_Civil")][Range(0, int.MaxValue)] public int? Dis_Civil { get; set; }
    }
}
