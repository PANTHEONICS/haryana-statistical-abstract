using HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence.Dtos;

namespace HaryanaStatAbstract.API.Services.SocialSecurityAndSocialDefence
{
    public interface ITable7_1SanctionedStrengthPoliceService
    {
        Task<IEnumerable<Table7_1SanctionedStrengthPoliceDto>> GetAllAsync();
        Task<Table7_1SanctionedStrengthPoliceDto?> GetByIdAsync(int id);
        Task<Table7_1SanctionedStrengthPoliceDto?> GetByYearAsync(int year);
        Task<Table7_1SanctionedStrengthPoliceDto> CreateAsync(CreateTable7_1SanctionedStrengthPoliceDto dto, int? userId = null, string? ipAddress = null);
        Task<Table7_1SanctionedStrengthPoliceDto?> UpdateAsync(int id, UpdateTable7_1SanctionedStrengthPoliceDto dto, int? userId = null, string? ipAddress = null);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int year);
    }
}
