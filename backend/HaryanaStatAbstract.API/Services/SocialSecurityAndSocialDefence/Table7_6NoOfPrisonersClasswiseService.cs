using HaryanaStatAbstract.API.Data;
using HaryanaStatAbstract.API.Helpers;
using HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence;
using HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HaryanaStatAbstract.API.Services.SocialSecurityAndSocialDefence
{
    public class Table7_6NoOfPrisonersClasswiseService : ITable7_6NoOfPrisonersClasswiseService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Table7_6NoOfPrisonersClasswiseService> _logger;

        public Table7_6NoOfPrisonersClasswiseService(ApplicationDbContext context, ILogger<Table7_6NoOfPrisonersClasswiseService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private static int CalcRem(int beg, int adm, int dis) => Math.Max(0, beg + adm - dis);

        public async Task<IEnumerable<Table7_6NoOfPrisonersClasswiseDto>> GetAllAsync()
        {
            var entities = await _context.Table7_6NoOfPrisonersClasswise
                .Include(e => e.CreatedByUser).Include(e => e.ModifiedByUser)
                .OrderBy(e => e.Year).ToListAsync();
            return entities.Select(MapToDto);
        }

        public async Task<Table7_6NoOfPrisonersClasswiseDto?> GetByIdAsync(int id)
        {
            var e = await _context.Table7_6NoOfPrisonersClasswise
                .Include(x => x.CreatedByUser).Include(x => x.ModifiedByUser)
                .FirstOrDefaultAsync(x => x.Id == id);
            return e == null ? null : MapToDto(e);
        }

        public async Task<Table7_6NoOfPrisonersClasswiseDto?> GetByYearAsync(string year)
        {
            var e = await _context.Table7_6NoOfPrisonersClasswise
                .Include(x => x.CreatedByUser).Include(x => x.ModifiedByUser)
                .FirstOrDefaultAsync(x => x.Year == year);
            return e == null ? null : MapToDto(e);
        }

        public async Task<Table7_6NoOfPrisonersClasswiseDto> CreateAsync(CreateTable7_6NoOfPrisonersClasswiseDto dto, int? userId = null, string? ipAddress = null)
        {
            if (await ExistsAsync(dto.Year))
                throw new InvalidOperationException($"Record for year {dto.Year} already exists");
            var remConv = CalcRem(dto.Beg_Convicted, dto.Adm_Convicted, dto.Dis_Convicted);
            var remUT = CalcRem(dto.Beg_UnderTrial, dto.Adm_UnderTrial, dto.Dis_UnderTrial);
            var remCivil = CalcRem(dto.Beg_Civil, dto.Adm_Civil, dto.Dis_Civil);
            var now = DateTimeHelper.GetISTNow();
            var entity = new Table7_6NoOfPrisonersClasswise
            {
                Year = dto.Year,
                Beg_Convicted = dto.Beg_Convicted, Beg_UnderTrial = dto.Beg_UnderTrial, Beg_Civil = dto.Beg_Civil,
                Adm_Convicted = dto.Adm_Convicted, Adm_UnderTrial = dto.Adm_UnderTrial, Adm_Civil = dto.Adm_Civil,
                Dis_Convicted = dto.Dis_Convicted, Dis_UnderTrial = dto.Dis_UnderTrial, Dis_Civil = dto.Dis_Civil,
                Rem_Convicted = remConv, Rem_UnderTrial = remUT, Rem_Civil = remCivil,
                CreatedDateTime = now, ModifiedDateTime = now,
                CreatedBy = userId, CreatedByIPAddress = ipAddress,
                ModifiedBy = userId, ModifiedByIPAddress = ipAddress
            };
            _context.Table7_6NoOfPrisonersClasswise.Add(entity);
            await _context.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task<Table7_6NoOfPrisonersClasswiseDto?> UpdateAsync(int id, UpdateTable7_6NoOfPrisonersClasswiseDto dto, int? userId = null, string? ipAddress = null)
        {
            var entity = await _context.Table7_6NoOfPrisonersClasswise.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) return null;
            if (!string.IsNullOrEmpty(dto.Year)) { if (await _context.Table7_6NoOfPrisonersClasswise.AnyAsync(e => e.Year == dto.Year && e.Id != id)) throw new InvalidOperationException($"Year {dto.Year} exists"); entity.Year = dto.Year; }
            if (dto.Beg_Convicted.HasValue) entity.Beg_Convicted = dto.Beg_Convicted.Value;
            if (dto.Beg_UnderTrial.HasValue) entity.Beg_UnderTrial = dto.Beg_UnderTrial.Value;
            if (dto.Beg_Civil.HasValue) entity.Beg_Civil = dto.Beg_Civil.Value;
            if (dto.Adm_Convicted.HasValue) entity.Adm_Convicted = dto.Adm_Convicted.Value;
            if (dto.Adm_UnderTrial.HasValue) entity.Adm_UnderTrial = dto.Adm_UnderTrial.Value;
            if (dto.Adm_Civil.HasValue) entity.Adm_Civil = dto.Adm_Civil.Value;
            if (dto.Dis_Convicted.HasValue) entity.Dis_Convicted = dto.Dis_Convicted.Value;
            if (dto.Dis_UnderTrial.HasValue) entity.Dis_UnderTrial = dto.Dis_UnderTrial.Value;
            if (dto.Dis_Civil.HasValue) entity.Dis_Civil = dto.Dis_Civil.Value;
            entity.Rem_Convicted = CalcRem(entity.Beg_Convicted, entity.Adm_Convicted, entity.Dis_Convicted);
            entity.Rem_UnderTrial = CalcRem(entity.Beg_UnderTrial, entity.Adm_UnderTrial, entity.Dis_UnderTrial);
            entity.Rem_Civil = CalcRem(entity.Beg_Civil, entity.Adm_Civil, entity.Dis_Civil);
            entity.ModifiedDateTime = DateTimeHelper.GetISTNow(); entity.ModifiedBy = userId; entity.ModifiedByIPAddress = ipAddress;
            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Table7_6NoOfPrisonersClasswise.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) return false;
            _context.Table7_6NoOfPrisonersClasswise.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string year) => await _context.Table7_6NoOfPrisonersClasswise.AnyAsync(e => e.Year == year);

        private static Table7_6NoOfPrisonersClasswiseDto MapToDto(Table7_6NoOfPrisonersClasswise e) => new()
        {
            Id = e.Id, Year = e.Year,
            Beg_Convicted = e.Beg_Convicted, Beg_UnderTrial = e.Beg_UnderTrial, Beg_Civil = e.Beg_Civil,
            Adm_Convicted = e.Adm_Convicted, Adm_UnderTrial = e.Adm_UnderTrial, Adm_Civil = e.Adm_Civil,
            Dis_Convicted = e.Dis_Convicted, Dis_UnderTrial = e.Dis_UnderTrial, Dis_Civil = e.Dis_Civil,
            Rem_Convicted = e.Rem_Convicted, Rem_UnderTrial = e.Rem_UnderTrial, Rem_Civil = e.Rem_Civil,
            CreatedDateTime = e.CreatedDateTime, ModifiedDateTime = e.ModifiedDateTime,
            CreatedBy = e.CreatedBy, CreatedByIPAddress = e.CreatedByIPAddress,
            ModifiedBy = e.ModifiedBy, ModifiedByIPAddress = e.ModifiedByIPAddress,
            CreatedByUserName = e.CreatedByUser?.FullName, ModifiedByUserName = e.ModifiedByUser?.FullName
        };
    }
}
