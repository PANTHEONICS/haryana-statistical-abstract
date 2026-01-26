using HaryanaStatAbstract.API.Data;
using HaryanaStatAbstract.API.Helpers;
using HaryanaStatAbstract.API.Models.Education;
using HaryanaStatAbstract.API.Models.Education.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HaryanaStatAbstract.API.Services.Education
{
    /// <summary>
    /// Service implementation for Table 6.1 Institutions operations
    /// </summary>
    public class Table6_1InstitutionsService : ITable6_1InstitutionsService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Table6_1InstitutionsService> _logger;

        public Table6_1InstitutionsService(
            ApplicationDbContext context,
            ILogger<Table6_1InstitutionsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Table6_1InstitutionsDto>> GetAllAsync()
        {
            try
            {
                var entities = await _context.Table6_1Institutions
                    .OrderBy(i => i.InstitutionType)
                    .ToListAsync();

                return entities.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all institution records");
                throw;
            }
        }

        public async Task<Table6_1InstitutionsDto?> GetByIdAsync(int institutionId)
        {
            try
            {
                var entity = await _context.Table6_1Institutions
                    .Include(i => i.CreatedByUser)
                    .Include(i => i.ModifiedByUser)
                    .FirstOrDefaultAsync(i => i.InstitutionID == institutionId);

                return entity == null ? null : MapToDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving institution record for ID {InstitutionID}", institutionId);
                throw;
            }
        }

        public async Task<Table6_1InstitutionsDto?> GetByInstitutionTypeAsync(string institutionType)
        {
            try
            {
                var entity = await _context.Table6_1Institutions
                    .Include(i => i.CreatedByUser)
                    .Include(i => i.ModifiedByUser)
                    .FirstOrDefaultAsync(i => i.InstitutionType == institutionType);

                return entity == null ? null : MapToDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving institution record for type {InstitutionType}", institutionType);
                throw;
            }
        }

        public async Task<Table6_1InstitutionsDto> CreateAsync(CreateTable6_1InstitutionsDto createDto, int? userId = null, string? ipAddress = null)
        {
            try
            {
                // Check if institution type already exists
                if (await ExistsAsync(createDto.InstitutionType))
                {
                    throw new InvalidOperationException(
                        $"Institution record for type '{createDto.InstitutionType}' already exists");
                }

                var now = DateTimeHelper.GetISTNow();
                var entity = new Table6_1Institutions
                {
                    InstitutionType = createDto.InstitutionType,
                    Year196667 = createDto.Year196667,
                    Year197071 = createDto.Year197071,
                    Year198081 = createDto.Year198081,
                    Year199091 = createDto.Year199091,
                    Year200001 = createDto.Year200001,
                    Year201011 = createDto.Year201011,
                    Year201617 = createDto.Year201617,
                    Year201718 = createDto.Year201718,
                    Year201819 = createDto.Year201819,
                    Year201920 = createDto.Year201920,
                    Year202021 = createDto.Year202021,
                    Year202122 = createDto.Year202122,
                    Year202223 = createDto.Year202223,
                    Year202324 = createDto.Year202324,
                    Year202425 = createDto.Year202425,
                    CreatedDateTime = now,
                    ModifiedDateTime = now,
                    CreatedBy = userId,
                    CreatedByIPAddress = ipAddress,
                    ModifiedBy = userId,
                    ModifiedByIPAddress = ipAddress
                };

                _context.Table6_1Institutions.Add(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created institution record for type {InstitutionType}", createDto.InstitutionType);

                return MapToDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating institution record");
                throw;
            }
        }

        public async Task<Table6_1InstitutionsDto?> UpdateAsync(int institutionId, UpdateTable6_1InstitutionsDto updateDto, int? userId = null, string? ipAddress = null)
        {
            try
            {
                var entity = await _context.Table6_1Institutions
                    .FirstOrDefaultAsync(i => i.InstitutionID == institutionId);

                if (entity == null)
                {
                    return null;
                }

                // Build SQL update statement to avoid OUTPUT clause conflict with triggers
                var updateParts = new List<string>();
                var parameters = new List<object>();

                // Update institution type if provided (check for uniqueness)
                if (!string.IsNullOrEmpty(updateDto.InstitutionType) && updateDto.InstitutionType != entity.InstitutionType)
                {
                    var newType = updateDto.InstitutionType;
                    // Check if the new type already exists (excluding current record)
                    var existingType = await _context.Table6_1Institutions
                        .AnyAsync(i => i.InstitutionType == newType && i.InstitutionID != institutionId);
                    
                    if (existingType)
                    {
                        throw new InvalidOperationException(
                            $"Institution record for type '{newType}' already exists");
                    }

                    updateParts.Add($"InstitutionType = {{{parameters.Count}}}");
                    parameters.Add(newType);
                    entity.InstitutionType = newType;
                }

                // Update year columns if provided
                if (updateDto.Year196667.HasValue)
                {
                    updateParts.Add($"Year_1966_67 = {{{parameters.Count}}}");
                    parameters.Add(updateDto.Year196667.Value);
                    entity.Year196667 = updateDto.Year196667.Value;
                }

                if (updateDto.Year197071.HasValue)
                {
                    updateParts.Add($"Year_1970_71 = {{{parameters.Count}}}");
                    parameters.Add(updateDto.Year197071.Value);
                    entity.Year197071 = updateDto.Year197071.Value;
                }

                if (updateDto.Year198081.HasValue)
                {
                    updateParts.Add($"Year_1980_81 = {{{parameters.Count}}}");
                    parameters.Add(updateDto.Year198081.Value);
                    entity.Year198081 = updateDto.Year198081.Value;
                }

                if (updateDto.Year199091.HasValue)
                {
                    updateParts.Add($"Year_1990_91 = {{{parameters.Count}}}");
                    parameters.Add(updateDto.Year199091.Value);
                    entity.Year199091 = updateDto.Year199091.Value;
                }

                if (updateDto.Year200001.HasValue)
                {
                    updateParts.Add($"Year_2000_01 = {{{parameters.Count}}}");
                    parameters.Add(updateDto.Year200001.Value);
                    entity.Year200001 = updateDto.Year200001.Value;
                }

                if (updateDto.Year201011.HasValue)
                {
                    updateParts.Add($"Year_2010_11 = {{{parameters.Count}}}");
                    parameters.Add(updateDto.Year201011.Value);
                    entity.Year201011 = updateDto.Year201011.Value;
                }

                if (updateDto.Year201617.HasValue)
                {
                    updateParts.Add($"Year_2016_17 = {{{parameters.Count}}}");
                    parameters.Add(updateDto.Year201617.Value);
                    entity.Year201617 = updateDto.Year201617.Value;
                }

                if (updateDto.Year201718.HasValue)
                {
                    updateParts.Add($"Year_2017_18 = {{{parameters.Count}}}");
                    parameters.Add(updateDto.Year201718.Value);
                    entity.Year201718 = updateDto.Year201718.Value;
                }

                if (updateDto.Year201819.HasValue)
                {
                    updateParts.Add($"Year_2018_19 = {{{parameters.Count}}}");
                    parameters.Add(updateDto.Year201819.Value);
                    entity.Year201819 = updateDto.Year201819.Value;
                }

                if (updateDto.Year201920.HasValue)
                {
                    updateParts.Add($"Year_2019_20 = {{{parameters.Count}}}");
                    parameters.Add(updateDto.Year201920.Value);
                    entity.Year201920 = updateDto.Year201920.Value;
                }

                if (updateDto.Year202021.HasValue)
                {
                    updateParts.Add($"Year_2020_21 = {{{parameters.Count}}}");
                    parameters.Add(updateDto.Year202021.Value);
                    entity.Year202021 = updateDto.Year202021.Value;
                }

                if (updateDto.Year202122.HasValue)
                {
                    updateParts.Add($"Year_2021_22 = {{{parameters.Count}}}");
                    parameters.Add(updateDto.Year202122.Value);
                    entity.Year202122 = updateDto.Year202122.Value;
                }

                if (updateDto.Year202223.HasValue)
                {
                    updateParts.Add($"Year_2022_23 = {{{parameters.Count}}}");
                    parameters.Add(updateDto.Year202223.Value);
                    entity.Year202223 = updateDto.Year202223.Value;
                }

                if (updateDto.Year202324.HasValue)
                {
                    updateParts.Add($"Year_2023_24 = {{{parameters.Count}}}");
                    parameters.Add(updateDto.Year202324.Value);
                    entity.Year202324 = updateDto.Year202324.Value;
                }

                if (updateDto.Year202425.HasValue)
                {
                    updateParts.Add($"Year_2024_25 = {{{parameters.Count}}}");
                    parameters.Add(updateDto.Year202425.Value);
                    entity.Year202425 = updateDto.Year202425.Value;
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

                // Execute raw SQL update to avoid OUTPUT clause conflict with triggers
                if (updateParts.Count > 0)
                {
                    var sql = $"UPDATE Ed_Table_6_1_Institutions SET {string.Join(", ", updateParts)} WHERE InstitutionID = {{{parameters.Count}}}";
                    parameters.Add(entity.InstitutionID);

                    await _context.Database.ExecuteSqlRawAsync(sql, parameters.ToArray());
                }

                // Reload the entity to get any trigger-updated values
                entity = await _context.Table6_1Institutions
                    .FirstOrDefaultAsync(i => i.InstitutionID == institutionId);

                _logger.LogInformation("Updated institution record (InstitutionID: {InstitutionID}, Type: {InstitutionType})", 
                    institutionId, entity?.InstitutionType);

                return entity == null ? null : MapToDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating institution record (InstitutionID: {InstitutionID})", institutionId);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int institutionId)
        {
            try
            {
                var entity = await _context.Table6_1Institutions
                    .FirstOrDefaultAsync(i => i.InstitutionID == institutionId);

                if (entity == null)
                {
                    return false;
                }

                // Use raw SQL to avoid OUTPUT clause conflict with triggers
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                    "DELETE FROM Ed_Table_6_1_Institutions WHERE InstitutionID = {0}",
                    entity.InstitutionID);

                _logger.LogInformation("Deleted institution record (InstitutionID: {InstitutionID}, Type: {InstitutionType})", 
                    institutionId, entity.InstitutionType);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting institution record (InstitutionID: {InstitutionID})", institutionId);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(string institutionType)
        {
            return await _context.Table6_1Institutions
                .AnyAsync(i => i.InstitutionType == institutionType);
        }

        private static Table6_1InstitutionsDto MapToDto(Table6_1Institutions entity)
        {
            return new Table6_1InstitutionsDto
            {
                InstitutionID = entity.InstitutionID,
                InstitutionType = entity.InstitutionType,
                Year196667 = entity.Year196667,
                Year197071 = entity.Year197071,
                Year198081 = entity.Year198081,
                Year199091 = entity.Year199091,
                Year200001 = entity.Year200001,
                Year201011 = entity.Year201011,
                Year201617 = entity.Year201617,
                Year201718 = entity.Year201718,
                Year201819 = entity.Year201819,
                Year201920 = entity.Year201920,
                Year202021 = entity.Year202021,
                Year202122 = entity.Year202122,
                Year202223 = entity.Year202223,
                Year202324 = entity.Year202324,
                Year202425 = entity.Year202425,
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
