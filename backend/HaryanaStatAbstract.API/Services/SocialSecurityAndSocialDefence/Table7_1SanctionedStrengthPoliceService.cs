using HaryanaStatAbstract.API.Data;
using HaryanaStatAbstract.API.Helpers;
using HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence;
using HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HaryanaStatAbstract.API.Services.SocialSecurityAndSocialDefence
{
    public class Table7_1SanctionedStrengthPoliceService : ITable7_1SanctionedStrengthPoliceService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Table7_1SanctionedStrengthPoliceService> _logger;

        public Table7_1SanctionedStrengthPoliceService(ApplicationDbContext context, ILogger<Table7_1SanctionedStrengthPoliceService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Table7_1SanctionedStrengthPoliceDto>> GetAllAsync()
        {
            var entities = await _context.Table7_1SanctionedStrengthPolice
                .Include(e => e.CreatedByUser).Include(e => e.ModifiedByUser)
                .OrderBy(e => e.Year).ToListAsync();
            return entities.Select(MapToDto);
        }

        public async Task<Table7_1SanctionedStrengthPoliceDto?> GetByIdAsync(int id)
        {
            var e = await _context.Table7_1SanctionedStrengthPolice
                .Include(x => x.CreatedByUser).Include(x => x.ModifiedByUser)
                .FirstOrDefaultAsync(x => x.Id == id);
            return e == null ? null : MapToDto(e);
        }

        public async Task<Table7_1SanctionedStrengthPoliceDto?> GetByYearAsync(int year)
        {
            var e = await _context.Table7_1SanctionedStrengthPolice
                .Include(x => x.CreatedByUser).Include(x => x.ModifiedByUser)
                .FirstOrDefaultAsync(x => x.Year == year);
            return e == null ? null : MapToDto(e);
        }

        public async Task<Table7_1SanctionedStrengthPoliceDto> CreateAsync(CreateTable7_1SanctionedStrengthPoliceDto dto, int? userId = null, string? ipAddress = null)
        {
            if (await ExistsAsync(dto.Year))
                throw new InvalidOperationException($"Record for year {dto.Year} already exists");
            var total = dto.DG_ADG_IG_DyIG + dto.Asst_IG + dto.Superintendents_Addl_Dy_Asst + dto.Inspectors_SI_ASI + dto.Head_Constables_RC + dto.Mounted_Foot_Constables;
            var now = DateTimeHelper.GetISTNow();
            var entity = new Table7_1SanctionedStrengthPolice
            {
                Year = dto.Year,
                DG_ADG_IG_DyIG = dto.DG_ADG_IG_DyIG,
                Asst_IG = dto.Asst_IG,
                Superintendents_Addl_Dy_Asst = dto.Superintendents_Addl_Dy_Asst,
                Inspectors_SI_ASI = dto.Inspectors_SI_ASI,
                Head_Constables_RC = dto.Head_Constables_RC,
                Mounted_Foot_Constables = dto.Mounted_Foot_Constables,
                Total = total,
                CreatedDateTime = now, ModifiedDateTime = now,
                CreatedBy = userId, CreatedByIPAddress = ipAddress,
                ModifiedBy = userId, ModifiedByIPAddress = ipAddress
            };
            _context.Table7_1SanctionedStrengthPolice.Add(entity);
            await _context.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task<Table7_1SanctionedStrengthPoliceDto?> UpdateAsync(int id, UpdateTable7_1SanctionedStrengthPoliceDto dto, int? userId = null, string? ipAddress = null)
        {
            var entity = await _context.Table7_1SanctionedStrengthPolice.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) return null;
            if (dto.Year.HasValue) { if (await _context.Table7_1SanctionedStrengthPolice.AnyAsync(e => e.Year == dto.Year.Value && e.Id != id)) throw new InvalidOperationException($"Year {dto.Year} exists"); entity.Year = dto.Year.Value; }
            if (dto.DG_ADG_IG_DyIG.HasValue) entity.DG_ADG_IG_DyIG = dto.DG_ADG_IG_DyIG.Value;
            if (dto.Asst_IG.HasValue) entity.Asst_IG = dto.Asst_IG.Value;
            if (dto.Superintendents_Addl_Dy_Asst.HasValue) entity.Superintendents_Addl_Dy_Asst = dto.Superintendents_Addl_Dy_Asst.Value;
            if (dto.Inspectors_SI_ASI.HasValue) entity.Inspectors_SI_ASI = dto.Inspectors_SI_ASI.Value;
            if (dto.Head_Constables_RC.HasValue) entity.Head_Constables_RC = dto.Head_Constables_RC.Value;
            if (dto.Mounted_Foot_Constables.HasValue) entity.Mounted_Foot_Constables = dto.Mounted_Foot_Constables.Value;
            entity.Total = entity.DG_ADG_IG_DyIG + entity.Asst_IG + entity.Superintendents_Addl_Dy_Asst + entity.Inspectors_SI_ASI + entity.Head_Constables_RC + entity.Mounted_Foot_Constables;
            entity.ModifiedDateTime = DateTimeHelper.GetISTNow(); entity.ModifiedBy = userId; entity.ModifiedByIPAddress = ipAddress;
            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Table7_1SanctionedStrengthPolice.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) return false;
            _context.Table7_1SanctionedStrengthPolice.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int year) => await _context.Table7_1SanctionedStrengthPolice.AnyAsync(e => e.Year == year);

        private static Table7_1SanctionedStrengthPoliceDto MapToDto(Table7_1SanctionedStrengthPolice e) => new()
        {
            Id = e.Id, Year = e.Year,
            DG_ADG_IG_DyIG = e.DG_ADG_IG_DyIG, Asst_IG = e.Asst_IG,
            Superintendents_Addl_Dy_Asst = e.Superintendents_Addl_Dy_Asst,
            Inspectors_SI_ASI = e.Inspectors_SI_ASI, Head_Constables_RC = e.Head_Constables_RC,
            Mounted_Foot_Constables = e.Mounted_Foot_Constables, Total = e.Total,
            CreatedDateTime = e.CreatedDateTime, ModifiedDateTime = e.ModifiedDateTime,
            CreatedBy = e.CreatedBy, CreatedByIPAddress = e.CreatedByIPAddress,
            ModifiedBy = e.ModifiedBy, ModifiedByIPAddress = e.ModifiedByIPAddress,
            CreatedByUserName = e.CreatedByUser?.FullName, ModifiedByUserName = e.ModifiedByUser?.FullName
        };
    }
}
