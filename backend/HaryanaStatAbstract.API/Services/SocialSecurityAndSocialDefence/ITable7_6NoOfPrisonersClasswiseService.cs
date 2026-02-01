using HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence.Dtos;

namespace HaryanaStatAbstract.API.Services.SocialSecurityAndSocialDefence
{
    public interface ITable7_6NoOfPrisonersClasswiseService
    {
        Task<IEnumerable<Table7_6NoOfPrisonersClasswiseDto>> GetAllAsync();
        Task<Table7_6NoOfPrisonersClasswiseDto?> GetByIdAsync(int id);
        Task<Table7_6NoOfPrisonersClasswiseDto?> GetByYearAsync(string year);
        Task<Table7_6NoOfPrisonersClasswiseDto> CreateAsync(CreateTable7_6NoOfPrisonersClasswiseDto dto, int? userId = null, string? ipAddress = null);
        Task<Table7_6NoOfPrisonersClasswiseDto?> UpdateAsync(int id, UpdateTable7_6NoOfPrisonersClasswiseDto dto, int? userId = null, string? ipAddress = null);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(string year);
    }
}
