using HaryanaStatAbstract.API.Data;
using HaryanaStatAbstract.API.Helpers;
using HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence;
using HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HaryanaStatAbstract.API.Services.SocialSecurityAndSocialDefence
{
    public class Table7_8JailIndustryProductionProgressService : ITable7_8JailIndustryProductionProgressService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Table7_8JailIndustryProductionProgressService> _logger;

        public Table7_8JailIndustryProductionProgressService(ApplicationDbContext context, ILogger<Table7_8JailIndustryProductionProgressService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private static long CalcTotal(CreateTable7_8JailIndustryProductionProgressDto d) =>
            d.Carpentry + d.Textile + d.Leather + d.Durries + d.Tailoring + d.Munj + d.Chicks + d.Oil_OilCake + d.Tents + d.Blankets + d.Smithy + d.Niwar_Tapes + d.Misc;

        public async Task<IEnumerable<Table7_8JailIndustryProductionProgressDto>> GetAllAsync()
        {
            var entities = await _context.Table7_8JailIndustryProductionProgress
                .Include(e => e.CreatedByUser).Include(e => e.ModifiedByUser)
                .OrderBy(e => e.Year).ToListAsync();
            return entities.Select(MapToDto);
        }

        public async Task<Table7_8JailIndustryProductionProgressDto?> GetByIdAsync(int id)
        {
            var e = await _context.Table7_8JailIndustryProductionProgress
                .Include(x => x.CreatedByUser).Include(x => x.ModifiedByUser)
                .FirstOrDefaultAsync(x => x.Id == id);
            return e == null ? null : MapToDto(e);
        }

        public async Task<Table7_8JailIndustryProductionProgressDto?> GetByYearAsync(string year)
        {
            var e = await _context.Table7_8JailIndustryProductionProgress
                .Include(x => x.CreatedByUser).Include(x => x.ModifiedByUser)
                .FirstOrDefaultAsync(x => x.Year == year);
            return e == null ? null : MapToDto(e);
        }

        public async Task<Table7_8JailIndustryProductionProgressDto> CreateAsync(CreateTable7_8JailIndustryProductionProgressDto dto, int? userId = null, string? ipAddress = null)
        {
            if (await ExistsAsync(dto.Year))
                throw new InvalidOperationException($"Record for year {dto.Year} already exists");
            var total = CalcTotal(dto);
            var now = DateTimeHelper.GetISTNow();
            var entity = new Table7_8JailIndustryProductionProgress
            {
                Year = dto.Year,
                Carpentry = dto.Carpentry, Textile = dto.Textile, Leather = dto.Leather, Durries = dto.Durries,
                Tailoring = dto.Tailoring, Munj = dto.Munj, Chicks = dto.Chicks, Oil_OilCake = dto.Oil_OilCake,
                Tents = dto.Tents, Blankets = dto.Blankets, Smithy = dto.Smithy, Niwar_Tapes = dto.Niwar_Tapes, Misc = dto.Misc,
                Total = total,
                CreatedDateTime = now, ModifiedDateTime = now,
                CreatedBy = userId, CreatedByIPAddress = ipAddress,
                ModifiedBy = userId, ModifiedByIPAddress = ipAddress
            };
            _context.Table7_8JailIndustryProductionProgress.Add(entity);
            await _context.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task<Table7_8JailIndustryProductionProgressDto?> UpdateAsync(int id, UpdateTable7_8JailIndustryProductionProgressDto dto, int? userId = null, string? ipAddress = null)
        {
            var entity = await _context.Table7_8JailIndustryProductionProgress.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) return null;
            if (!string.IsNullOrEmpty(dto.Year)) { if (await _context.Table7_8JailIndustryProductionProgress.AnyAsync(e => e.Year == dto.Year && e.Id != id)) throw new InvalidOperationException($"Year {dto.Year} exists"); entity.Year = dto.Year; }
            if (dto.Carpentry.HasValue) entity.Carpentry = dto.Carpentry.Value;
            if (dto.Textile.HasValue) entity.Textile = dto.Textile.Value;
            if (dto.Leather.HasValue) entity.Leather = dto.Leather.Value;
            if (dto.Durries.HasValue) entity.Durries = dto.Durries.Value;
            if (dto.Tailoring.HasValue) entity.Tailoring = dto.Tailoring.Value;
            if (dto.Munj.HasValue) entity.Munj = dto.Munj.Value;
            if (dto.Chicks.HasValue) entity.Chicks = dto.Chicks.Value;
            if (dto.Oil_OilCake.HasValue) entity.Oil_OilCake = dto.Oil_OilCake.Value;
            if (dto.Tents.HasValue) entity.Tents = dto.Tents.Value;
            if (dto.Blankets.HasValue) entity.Blankets = dto.Blankets.Value;
            if (dto.Smithy.HasValue) entity.Smithy = dto.Smithy.Value;
            if (dto.Niwar_Tapes.HasValue) entity.Niwar_Tapes = dto.Niwar_Tapes.Value;
            if (dto.Misc.HasValue) entity.Misc = dto.Misc.Value;
            entity.Total = entity.Carpentry + entity.Textile + entity.Leather + entity.Durries + entity.Tailoring + entity.Munj + entity.Chicks + entity.Oil_OilCake + entity.Tents + entity.Blankets + entity.Smithy + entity.Niwar_Tapes + entity.Misc;
            entity.ModifiedDateTime = DateTimeHelper.GetISTNow();
            entity.ModifiedBy = userId;
            entity.ModifiedByIPAddress = ipAddress;
            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Table7_8JailIndustryProductionProgress.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) return false;
            _context.Table7_8JailIndustryProductionProgress.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string year) => await _context.Table7_8JailIndustryProductionProgress.AnyAsync(e => e.Year == year);

        private static Table7_8JailIndustryProductionProgressDto MapToDto(Table7_8JailIndustryProductionProgress e) => new()
        {
            Id = e.Id, Year = e.Year,
            Carpentry = e.Carpentry, Textile = e.Textile, Leather = e.Leather, Durries = e.Durries,
            Tailoring = e.Tailoring, Munj = e.Munj, Chicks = e.Chicks, Oil_OilCake = e.Oil_OilCake,
            Tents = e.Tents, Blankets = e.Blankets, Smithy = e.Smithy, Niwar_Tapes = e.Niwar_Tapes, Misc = e.Misc,
            Total = e.Total,
            CreatedDateTime = e.CreatedDateTime, ModifiedDateTime = e.ModifiedDateTime,
            CreatedBy = e.CreatedBy, CreatedByIPAddress = e.CreatedByIPAddress,
            ModifiedBy = e.ModifiedBy, ModifiedByIPAddress = e.ModifiedByIPAddress,
            CreatedByUserName = e.CreatedByUser?.FullName, ModifiedByUserName = e.ModifiedByUser?.FullName
        };
    }
}
