using HaryanaStatAbstract.API.Data;
using HaryanaStatAbstract.API.Models;
using HaryanaStatAbstract.API.Models.AreaAndPopulation;
using HaryanaStatAbstract.API.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HaryanaStatAbstract.API.Services
{
    /// <summary>
    /// Service implementation for Census Population operations
    /// </summary>
    public class CensusPopulationService : ICensusPopulationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CensusPopulationService> _logger;

        public CensusPopulationService(
            ApplicationDbContext context,
            ILogger<CensusPopulationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<CensusPopulationDto>> GetAllAsync()
        {
            try
            {
                var entities = await _context.Table3_2CensusPopulations
                    .OrderBy(c => c.Year)
                    .ToListAsync();

                return entities.Select(e => MapToDto(e));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all census population records");
                throw;
            }
        }

        public async Task<CensusPopulationDto?> GetByYearAsync(int year)
        {
            try
            {
                var entity = await _context.Table3_2CensusPopulations
                    .FirstOrDefaultAsync(c => c.Year == year);

                return entity == null ? null : MapToDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving census population record for year {Year}", year);
                throw;
            }
        }

        public async Task<CensusPopulationDto> CreateAsync(CreateCensusPopulationDto createDto)
        {
            try
            {
                // Validate that total equals sum of male and female
                if (createDto.TotalPopulation != createDto.MalePopulation + createDto.FemalePopulation)
                {
                    throw new ArgumentException(
                        "Total population must equal the sum of male and female population");
                }

                // Check if year already exists
                if (await ExistsAsync(createDto.Year))
                {
                    throw new InvalidOperationException(
                        $"Census record for year {createDto.Year} already exists");
                }

                var entity = new Table3_2CensusPopulation
                {
                    Year = createDto.Year,
                    TotalPopulation = createDto.TotalPopulation,
                    VariationInPopulation = createDto.VariationInPopulation,
                    DecennialPercentageIncrease = createDto.DecennialPercentageIncrease,
                    MalePopulation = createDto.MalePopulation,
                    FemalePopulation = createDto.FemalePopulation,
                    SexRatio = createDto.SexRatio,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow
                };

                _context.Table3_2CensusPopulations.Add(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created census population record for year {Year}", createDto.Year);

                return MapToDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating census population record");
                throw;
            }
        }

        public async Task<CensusPopulationDto?> UpdateAsync(int censusId, UpdateCensusPopulationDto updateDto)
        {
            try
            {
                var entity = await _context.Table3_2CensusPopulations
                    .FirstOrDefaultAsync(c => c.CensusID == censusId);

                if (entity == null)
                {
                    return null;
                }

                // Build SQL update statement to avoid OUTPUT clause conflict with triggers
                var updateParts = new List<string>();
                var parameters = new List<object>();

                // Update year if provided (check for uniqueness)
                if (updateDto.Year.HasValue)
                {
                    var newYear = updateDto.Year.Value;
                    // Check if the new year already exists (excluding current record)
                    var existingYear = await _context.Table3_2CensusPopulations
                        .AnyAsync(c => c.Year == newYear && c.CensusID != censusId);
                    
                    if (existingYear)
                    {
                        throw new InvalidOperationException(
                            $"Census record for year {newYear} already exists");
                    }

                    updateParts.Add($"census_year = {{{parameters.Count}}}");
                    parameters.Add(newYear);
                    entity.Year = newYear;
                }

                // Update properties if provided
                if (updateDto.TotalPopulation.HasValue)
                {
                    updateParts.Add($"total_population = {{{parameters.Count}}}");
                    parameters.Add(updateDto.TotalPopulation.Value);
                    entity.TotalPopulation = updateDto.TotalPopulation.Value;
                }

                if (updateDto.MalePopulation.HasValue)
                {
                    updateParts.Add($"male_population = {{{parameters.Count}}}");
                    parameters.Add(updateDto.MalePopulation.Value);
                    entity.MalePopulation = updateDto.MalePopulation.Value;
                }

                if (updateDto.FemalePopulation.HasValue)
                {
                    updateParts.Add($"female_population = {{{parameters.Count}}}");
                    parameters.Add(updateDto.FemalePopulation.Value);
                    entity.FemalePopulation = updateDto.FemalePopulation.Value;
                }

                if (updateDto.VariationInPopulation.HasValue)
                {
                    updateParts.Add($"variation_in_population = {{{parameters.Count}}}");
                    parameters.Add(updateDto.VariationInPopulation.Value);
                    entity.VariationInPopulation = updateDto.VariationInPopulation.Value;
                }
                else if (updateDto.TotalPopulation.HasValue)
                {
                    // Allow setting to NULL explicitly when total population is being updated
                    updateParts.Add("variation_in_population = NULL");
                }

                if (updateDto.DecennialPercentageIncrease.HasValue)
                {
                    updateParts.Add($"decennial_percentage_increase = {{{parameters.Count}}}");
                    parameters.Add(updateDto.DecennialPercentageIncrease.Value);
                    entity.DecennialPercentageIncrease = updateDto.DecennialPercentageIncrease.Value;
                }
                else if (updateDto.TotalPopulation.HasValue)
                {
                    // Allow setting to NULL explicitly when total population is being updated
                    updateParts.Add("decennial_percentage_increase = NULL");
                }

                if (updateDto.SexRatio.HasValue)
                {
                    updateParts.Add($"sex_ratio = {{{parameters.Count}}}");
                    parameters.Add(updateDto.SexRatio.Value);
                    entity.SexRatio = updateDto.SexRatio.Value;
                }

                // Always update ModifiedDateTime
                updateParts.Add($"ModifiedDateTime = {{{parameters.Count}}}");
                parameters.Add(DateTime.UtcNow);
                entity.ModifiedDateTime = DateTime.UtcNow;

                // Validate that total equals sum of male and female
                var totalPop = updateDto.TotalPopulation ?? entity.TotalPopulation;
                var malePop = updateDto.MalePopulation ?? entity.MalePopulation;
                var femalePop = updateDto.FemalePopulation ?? entity.FemalePopulation;

                if (totalPop != malePop + femalePop)
                {
                    throw new ArgumentException(
                        "Total population must equal the sum of male and female population");
                }

                // Execute raw SQL update to avoid OUTPUT clause conflict with triggers
                // Use CensusID for internal operations
                if (updateParts.Count > 0)
                {
                    var sql = $"UPDATE AP_Table_3_2_CensusPopulation SET {string.Join(", ", updateParts)} WHERE CensusID = {{{parameters.Count}}}";
                    parameters.Add(entity.CensusID);

                    await _context.Database.ExecuteSqlRawAsync(sql, parameters.ToArray());
                }

                // Reload the entity to get any trigger-updated values
                entity = await _context.Table3_2CensusPopulations
                    .FirstOrDefaultAsync(c => c.CensusID == censusId);

                _logger.LogInformation("Updated census population record (CensusID: {CensusID}, Year: {Year})", 
                    censusId, entity?.Year);

                return entity == null ? null : MapToDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating census population record (CensusID: {CensusID})", censusId);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int censusId)
        {
            try
            {
                var entity = await _context.Table3_2CensusPopulations
                    .FirstOrDefaultAsync(c => c.CensusID == censusId);

                if (entity == null)
                {
                    return false;
                }

                // Use raw SQL to avoid OUTPUT clause conflict with triggers
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                    "DELETE FROM AP_Table_3_2_CensusPopulation WHERE CensusID = {0}",
                    entity.CensusID);

                _logger.LogInformation("Deleted census population record (CensusID: {CensusID}, Year: {Year})", 
                    censusId, entity.Year);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting census population record (CensusID: {CensusID})", censusId);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int year)
        {
            return await _context.Table3_2CensusPopulations
                .AnyAsync(c => c.Year == year);
        }

        public async Task<IEnumerable<CensusPopulationDto>> GetByYearRangeAsync(int startYear, int endYear)
        {
            try
            {
                var entities = await _context.Table3_2CensusPopulations
                    .Where(c => c.Year >= startYear && c.Year <= endYear)
                    .OrderBy(c => c.Year)
                    .ToListAsync();

                return entities.Select(e => MapToDto(e));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving census population records for range {StartYear}-{EndYear}", 
                    startYear, endYear);
                throw;
            }
        }

        private static CensusPopulationDto MapToDto(Table3_2CensusPopulation entity)
        {
            return new CensusPopulationDto
            {
                CensusID = entity.CensusID,
                Year = entity.Year,
                TotalPopulation = entity.TotalPopulation,
                VariationInPopulation = entity.VariationInPopulation,
                DecennialPercentageIncrease = entity.DecennialPercentageIncrease,
                MalePopulation = entity.MalePopulation,
                FemalePopulation = entity.FemalePopulation,
                SexRatio = entity.SexRatio
            };
        }
    }
}