using Microsoft.EntityFrameworkCore;
using InformationSystem.Domain.Models;
using InformationSystem.Application.Interfaces;

namespace Infrastructure.Storage
{
    public class BalanseAnalisysStorage : IBalanceAnalysisStorage
    {
        private InformationSystemDbContext _dbContext;

        public BalanseAnalisysStorage(InformationSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<BalanceAnalysis>> GetAllBalanceAnalysesAsync()
        {
           var toGet = await _dbContext.BalanceAnalysis.ToListAsync();
            return toGet;
        }

        public async Task<List<BalanceAnalysis>> GetBalanceAnalysisByPeriodAsync(DateTime period)
        {
            var toGet = await _dbContext.BalanceAnalysis
                .Where(d => d.Period == period)
                .ToListAsync();
            return toGet;
        }

        public async Task<BalanceAnalysis> GetAsync(Guid id)
        {
            var toGet = await _dbContext.BalanceAnalysis
                .Where(d => d.BalanceId == id).FirstOrDefaultAsync();
            return toGet;
        }

        public async Task<Guid> AddAsync(BalanceAnalysis item)
        {
            if (item.BalanceId == Guid.Empty)
            {
                item.BalanceId = Guid.NewGuid();
            }

            await _dbContext.BalanceAnalysis.AddAsync(item);
            await _dbContext.SaveChangesAsync();
            return item.BalanceId;
        }

        public async Task UpdateAsync(Guid id, BalanceAnalysis item)
        {
            BalanceAnalysis toUpdate = await _dbContext.BalanceAnalysis
               .FirstOrDefaultAsync(d => d.BalanceId == id);
            
            if (toUpdate != null)
            {
                toUpdate.TotalIncome = item.TotalIncome;
                toUpdate.TotalExpense = item.TotalExpense;
                toUpdate.ProfitOrLoss = item.TotalIncome - item.TotalExpense;
                toUpdate.Period = item.Period;

                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var toDelete = await _dbContext.BalanceAnalysis
                .FirstOrDefaultAsync(d => d.BalanceId == id);

            if (toDelete != null)
            {
                _dbContext.BalanceAnalysis.Remove(toDelete);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<BalanceAnalysis>> SearchAsync(string search, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = _dbContext.BalanceAnalysis.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                if (decimal.TryParse(search, out var value))
                {
                    query = query.Where(x => x.TotalIncome == value ||
                    x.TotalExpense == value ||
                    x.ProfitOrLoss == value);
                }
            }

            if (dateFrom.HasValue)
                query = query.Where(x => x.Period >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(x => x.Period <= dateTo.Value);

            return await query.ToListAsync();
        }
    }
}
