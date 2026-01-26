using HaryanaStatAbstract.API.Models.Education.Dtos;

namespace HaryanaStatAbstract.API.Services.Education
{
    /// <summary>
    /// Service interface for Table 6.1 Institutions operations
    /// </summary>
    public interface ITable6_1InstitutionsService
    {
        Task<IEnumerable<Table6_1InstitutionsDto>> GetAllAsync();
        Task<Table6_1InstitutionsDto?> GetByIdAsync(int institutionId);
        Task<Table6_1InstitutionsDto?> GetByInstitutionTypeAsync(string institutionType);
        Task<Table6_1InstitutionsDto> CreateAsync(CreateTable6_1InstitutionsDto createDto, int? userId = null, string? ipAddress = null);
        Task<Table6_1InstitutionsDto?> UpdateAsync(int institutionId, UpdateTable6_1InstitutionsDto updateDto, int? userId = null, string? ipAddress = null);
        Task<bool> DeleteAsync(int institutionId);
        Task<bool> ExistsAsync(string institutionType);
    }
}
