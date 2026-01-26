using HaryanaStatAbstract.API.Data;
using HaryanaStatAbstract.API.Helpers;
using HaryanaStatAbstract.API.Models.AreaAndPopulation;
using HaryanaStatAbstract.API.Models.AreaAndPopulation.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HaryanaStatAbstract.API.Services.AreaAndPopulation
{
    /// <summary>
    /// Service implementation for Census Population operations
    /// </summary>
    public class Table3_2CensusPopulationService : ITable3_2CensusPopulationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Table3_2CensusPopulationService> _logger;

        public Table3_2CensusPopulationService(
            ApplicationDbContext context,
            ILogger<Table3_2CensusPopulationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Table3_2CensusPopulationDto>> GetAllAsync()
        {
            try
            {
                var entities = await _context.Table3_2CensusPopulations
                    .Include(c => c.CreatedByUser)
                    .Include(c => c.ModifiedByUser)
                    .OrderBy(c => c.Year)
                    .ToListAsync();

                return entities.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all census population records");
                throw;
            }
        }

        public async Task<Table3_2CensusPopulationDto?> GetByYearAsync(int year)
        {
            try
            {
                var entity = await _context.Table3_2CensusPopulations
                    .Include(c => c.CreatedByUser)
                    .Include(c => c.ModifiedByUser)
                    .FirstOrDefaultAsync(c => c.Year == year);

                return entity == null ? null : MapToDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving census population record for year {Year}", year);
                throw;
            }
        }

        public async Task<Table3_2CensusPopulationDto> CreateAsync(CreateTable3_2CensusPopulationDto createDto, int? userId = null, string? ipAddress = null)
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

                var now = DateTimeHelper.GetISTNow();
                var entity = new Table3_2CensusPopulation
                {
                    Year = createDto.Year,
                    TotalPopulation = createDto.TotalPopulation,
                    VariationInPopulation = createDto.VariationInPopulation,
                    DecennialPercentageIncrease = createDto.DecennialPercentageIncrease,
                    MalePopulation = createDto.MalePopulation,
                    FemalePopulation = createDto.FemalePopulation,
                    SexRatio = createDto.SexRatio,
                    CreatedDateTime = now,
                    ModifiedDateTime = now,
                    CreatedBy = userId,
                    CreatedByIPAddress = ipAddress,
                    ModifiedBy = userId,
                    ModifiedByIPAddress = ipAddress
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

        public async Task<Table3_2CensusPopulationDto?> UpdateAsync(int censusId, UpdateTable3_2CensusPopulationDto updateDto, int? userId = null, string? ipAddress = null)
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

                // Always update ModifiedDateTime, ModifiedBy, ModifiedByIPAddress
                var now = DateTimeHelper.GetISTNow();
                updateParts.Add($"ModifiedDateTime = {{{parameters.Count}}}");
                parameters.Add(now);
                entity.ModifiedDateTime = now;

                if (userId.HasValue)
                {
                    updateParts.Add($"ModifiedBy = {{{parameters.Count}}}");
                    parameters.Add(userId.Value);
                    entity.ModifiedBy = userId.Value;
                }

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    updateParts.Add($"ModifiedByIPAddress = {{{parameters.Count}}}");
                    parameters.Add(ipAddress);
                    entity.ModifiedByIPAddress = ipAddress;
                }

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

        public async Task<IEnumerable<Table3_2CensusPopulationDto>> GetByYearRangeAsync(int startYear, int endYear)
        {
            try
            {
                var entities = await _context.Table3_2CensusPopulations
                    .Where(c => c.Year >= startYear && c.Year <= endYear)
                    .OrderBy(c => c.Year)
                    .ToListAsync();

                return entities.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving census population records for range {StartYear}-{EndYear}", 
                    startYear, endYear);
                throw;
            }
        }

        private Table3_2CensusPopulationDto MapToDto(Table3_2CensusPopulation entity)
        {
            return new Table3_2CensusPopulationDto
            {
                CensusID = entity.CensusID,
                Year = entity.Year,
                TotalPopulation = entity.TotalPopulation,
                VariationInPopulation = entity.VariationInPopulation,
                DecennialPercentageIncrease = entity.DecennialPercentageIncrease,
                MalePopulation = entity.MalePopulation,
                FemalePopulation = entity.FemalePopulation,
                SexRatio = entity.SexRatio,
                CreatedDateTime = entity.CreatedDateTime,
                ModifiedDateTime = entity.ModifiedDateTime,
                CreatedBy = entity.CreatedBy,
                CreatedByIPAddress = entity.CreatedByIPAddress,
                ModifiedBy = entity.ModifiedBy,
                ModifiedByIPAddress = entity.ModifiedByIPAddress,
                CreatedByUserName = entity.CreatedByUser?.FullName ?? null,
                ModifiedByUserName = entity.ModifiedByUser?.FullName ?? null
            };
        }
    }
}
