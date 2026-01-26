using HaryanaStatAbstract.API.Models.Dtos;

namespace HaryanaStatAbstract.API.Services
{
    /// <summary>
    /// Service interface for Census Population operations
    /// </summary>
    public interface ICensusPopulationService
    {
        Task<IEnumerable<CensusPopulationDto>> GetAllAsync();
        Task<CensusPopulationDto?> GetByYearAsync(int year);
        Task<CensusPopulationDto> CreateAsync(CreateCensusPopulationDto createDto);
        Task<CensusPopulationDto?> UpdateAsync(int censusId, UpdateCensusPopulationDto updateDto);
        Task<bool> DeleteAsync(int censusId);
        Task<bool> ExistsAsync(int year);
        Task<IEnumerable<CensusPopulationDto>> GetByYearRangeAsync(int startYear, int endYear);
    }
}