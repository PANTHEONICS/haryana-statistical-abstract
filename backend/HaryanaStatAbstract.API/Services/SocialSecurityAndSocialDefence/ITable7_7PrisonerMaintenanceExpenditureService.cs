using HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence.Dtos;

namespace HaryanaStatAbstract.API.Services.SocialSecurityAndSocialDefence
{
    public interface ITable7_7PrisonerMaintenanceExpenditureService
    {
        Task<IEnumerable<Table7_7PrisonerMaintenanceExpenditureDto>> GetAllAsync();
        Task<Table7_7PrisonerMaintenanceExpenditureDto?> GetByIdAsync(int id);
        Task<Table7_7PrisonerMaintenanceExpenditureDto?> GetByYearAsync(string year);
        Task<Table7_7PrisonerMaintenanceExpenditureDto> CreateAsync(CreateTable7_7PrisonerMaintenanceExpenditureDto dto, int? userId = null, string? ipAddress = null);
        Task<Table7_7PrisonerMaintenanceExpenditureDto?> UpdateAsync(int id, UpdateTable7_7PrisonerMaintenanceExpenditureDto dto, int? userId = null, string? ipAddress = null);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(string year);
    }
}
