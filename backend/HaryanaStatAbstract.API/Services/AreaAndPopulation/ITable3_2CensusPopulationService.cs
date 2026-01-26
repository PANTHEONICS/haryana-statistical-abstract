using HaryanaStatAbstract.API.Models.AreaAndPopulation.Dtos;

namespace HaryanaStatAbstract.API.Services.AreaAndPopulation
{
    /// <summary>
    /// Service interface for Census Population operations
    /// </summary>
    public interface ITable3_2CensusPopulationService
    {
        Task<IEnumerable<Table3_2CensusPopulationDto>> GetAllAsync();
        Task<Table3_2CensusPopulationDto?> GetByYearAsync(int year);
        Task<Table3_2CensusPopulationDto> CreateAsync(CreateTable3_2CensusPopulationDto createDto, int? userId = null, string? ipAddress = null);
        Task<Table3_2CensusPopulationDto?> UpdateAsync(int censusId, UpdateTable3_2CensusPopulationDto updateDto, int? userId = null, string? ipAddress = null);
        Task<bool> DeleteAsync(int censusId);
        Task<bool> ExistsAsync(int year);
        Task<IEnumerable<Table3_2CensusPopulationDto>> GetByYearRangeAsync(int startYear, int endYear);
    }
}
