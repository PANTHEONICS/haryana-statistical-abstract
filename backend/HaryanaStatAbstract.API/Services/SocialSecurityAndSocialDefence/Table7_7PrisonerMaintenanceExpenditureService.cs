using HaryanaStatAbstract.API.Data;
using HaryanaStatAbstract.API.Helpers;
using HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence;
using HaryanaStatAbstract.API.Models.SocialSecurityAndSocialDefence.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HaryanaStatAbstract.API.Services.SocialSecurityAndSocialDefence
{
    public class Table7_7PrisonerMaintenanceExpenditureService : ITable7_7PrisonerMaintenanceExpenditureService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Table7_7PrisonerMaintenanceExpenditureService> _logger;

        public Table7_7PrisonerMaintenanceExpenditureService(ApplicationDbContext context, ILogger<Table7_7PrisonerMaintenanceExpenditureService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private static (long ExpTotal, long CostPerPrisoner) Calc(long avg, long est, long diet, long others)
        {
            var total = est + diet + others;
            var cost = avg > 0 ? total / avg : 0;
            return (total, cost);
        }

        public async Task<IEnumerable<Table7_7PrisonerMaintenanceExpenditureDto>> GetAllAsync()
        {
            var entities = await _context.Table7_7PrisonerMaintenanceExpenditure
                .Include(e => e.CreatedByUser).Include(e => e.ModifiedByUser)
                .OrderBy(e => e.Year).ToListAsync();
            return entities.Select(MapToDto);
        }

        public async Task<Table7_7PrisonerMaintenanceExpenditureDto?> GetByIdAsync(int id)
        {
            var e = await _context.Table7_7PrisonerMaintenanceExpenditure
                .Include(x => x.CreatedByUser).Include(x => x.ModifiedByUser)
                .FirstOrDefaultAsync(x => x.Id == id);
            return e == null ? null : MapToDto(e);
        }

        public async Task<Table7_7PrisonerMaintenanceExpenditureDto?> GetByYearAsync(string year)
        {
            var e = await _context.Table7_7PrisonerMaintenanceExpenditure
                .Include(x => x.CreatedByUser).Include(x => x.ModifiedByUser)
                .FirstOrDefaultAsync(x => x.Year == year);
            return e == null ? null : MapToDto(e);
        }

        public async Task<Table7_7PrisonerMaintenanceExpenditureDto> CreateAsync(CreateTable7_7PrisonerMaintenanceExpenditureDto dto, int? userId = null, string? ipAddress = null)
        {
            if (await ExistsAsync(dto.Year))
                throw new InvalidOperationException($"Record for year {dto.Year} already exists");
            var (expTotal, costPerPrisoner) = Calc(dto.Avg_Prisoners, dto.Exp_Establishment, dto.Exp_Diet, dto.Exp_Others);
            var now = DateTimeHelper.GetISTNow();
            var entity = new Table7_7PrisonerMaintenanceExpenditure
            {
                Year = dto.Year,
                Avg_Prisoners = dto.Avg_Prisoners,
                Exp_Establishment = dto.Exp_Establishment,
                Exp_Diet = dto.Exp_Diet,
                Exp_Others = dto.Exp_Others,
                Exp_Total = expTotal,
                Cost_Per_Prisoner = costPerPrisoner,
                CreatedDateTime = now, ModifiedDateTime = now,
                CreatedBy = userId, CreatedByIPAddress = ipAddress,
                ModifiedBy = userId, ModifiedByIPAddress = ipAddress
            };
            _context.Table7_7PrisonerMaintenanceExpenditure.Add(entity);
            await _context.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task<Table7_7PrisonerMaintenanceExpenditureDto?> UpdateAsync(int id, UpdateTable7_7PrisonerMaintenanceExpenditureDto dto, int? userId = null, string? ipAddress = null)
        {
            var entity = await _context.Table7_7PrisonerMaintenanceExpenditure.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) return null;
            if (!string.IsNullOrEmpty(dto.Year)) { if (await _context.Table7_7PrisonerMaintenanceExpenditure.AnyAsync(e => e.Year == dto.Year && e.Id != id)) throw new InvalidOperationException($"Year {dto.Year} exists"); entity.Year = dto.Year; }
            if (dto.Avg_Prisoners.HasValue) entity.Avg_Prisoners = dto.Avg_Prisoners.Value;
            if (dto.Exp_Establishment.HasValue) entity.Exp_Establishment = dto.Exp_Establishment.Value;
            if (dto.Exp_Diet.HasValue) entity.Exp_Diet = dto.Exp_Diet.Value;
            if (dto.Exp_Others.HasValue) entity.Exp_Others = dto.Exp_Others.Value;
            var (expTotal, costPerPrisoner) = Calc(entity.Avg_Prisoners, entity.Exp_Establishment, entity.Exp_Diet, entity.Exp_Others);
            entity.Exp_Total = expTotal;
            entity.Cost_Per_Prisoner = costPerPrisoner;
            entity.ModifiedDateTime = DateTimeHelper.GetISTNow();
            entity.ModifiedBy = userId;
            entity.ModifiedByIPAddress = ipAddress;
            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Table7_7PrisonerMaintenanceExpenditure.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) return false;
            _context.Table7_7PrisonerMaintenanceExpenditure.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string year) => await _context.Table7_7PrisonerMaintenanceExpenditure.AnyAsync(e => e.Year == year);

        private static Table7_7PrisonerMaintenanceExpenditureDto MapToDto(Table7_7PrisonerMaintenanceExpenditure e) => new()
        {
            Id = e.Id, Year = e.Year,
            Avg_Prisoners = e.Avg_Prisoners,
            Exp_Establishment = e.Exp_Establishment, Exp_Diet = e.Exp_Diet, Exp_Others = e.Exp_Others,
            Exp_Total = e.Exp_Total, Cost_Per_Prisoner = e.Cost_Per_Prisoner,
            CreatedDateTime = e.CreatedDateTime, ModifiedDateTime = e.ModifiedDateTime,
            CreatedBy = e.CreatedBy, CreatedByIPAddress = e.CreatedByIPAddress,
            ModifiedBy = e.ModifiedBy, ModifiedByIPAddress = e.ModifiedByIPAddress,
            CreatedByUserName = e.CreatedByUser?.FullName, ModifiedByUserName = e.ModifiedByUser?.FullName
        };
    }
}
