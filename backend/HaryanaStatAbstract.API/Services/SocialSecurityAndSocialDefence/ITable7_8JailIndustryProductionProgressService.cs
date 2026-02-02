using HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence.Dtos;

namespace HaryanaStatAbstract.API.Services.SocialSecurityAndSocialDefence
{
    public interface ITable7_8JailIndustryProductionProgressService
    {
        Task<IEnumerable<Table7_8JailIndustryProductionProgressDto>> GetAllAsync();
        Task<Table7_8JailIndustryProductionProgressDto?> GetByIdAsync(int id);
        Task<Table7_8JailIndustryProductionProgressDto?> GetByYearAsync(string year);
        Task<Table7_8JailIndustryProductionProgressDto> CreateAsync(CreateTable7_8JailIndustryProductionProgressDto dto, int? userId = null, string? ipAddress = null);
        Task<Table7_8JailIndustryProductionProgressDto?> UpdateAsync(int id, UpdateTable7_8JailIndustryProductionProgressDto dto, int? userId = null, string? ipAddress = null);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(string year);
    }
}
