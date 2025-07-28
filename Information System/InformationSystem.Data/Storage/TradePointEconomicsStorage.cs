using InformationSystem.Application.Interfaces;
using InformationSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Storage
{
    public class TradePointEconomicsStorage : ITradePointEconomicsStorage
    {
        private InformationSystemDbContext _dbContext;

        public TradePointEconomicsStorage(InformationSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<TradePointEconomics>> GetAllTradePointEconomicsAsync()
        {
            return await _dbContext.TradePointEconomics.ToListAsync();
        }

        public async Task<List<TradePointEconomics>> GetTradePointEconomicsByPeriodAsync(DateTime period)
        {
            var toGet = await _dbContext.TradePointEconomics
                .Where(d => d.Period == period)
                .ToListAsync();
            return toGet;
        }
        public async Task<TradePointEconomics> GetAsync(Guid id)
        {
            var toGet = await _dbContext.TradePointEconomics
                .Where(d => d.TradePointEconomicsId == id).FirstOrDefaultAsync();

            return toGet;
        }
        public async Task<Guid> AddAsync(TradePointEconomics item)
        {
            if (item.TradePointEconomicsId == Guid.Empty)
            {
                item.TradePointEconomicsId = Guid.NewGuid();
            }

            await _dbContext.TradePointEconomics.AddAsync(item);
            await _dbContext.SaveChangesAsync();
            return item.TradePointEconomicsId;
        }

        public async Task UpdateAsync(Guid id, TradePointEconomics item)
        {
            var toUpdate = await _dbContext.TradePointEconomics
               .FirstOrDefaultAsync(d => d.TradePointEconomicsId == id);

            if (toUpdate != null)
            {
                toUpdate.Revenue = item.Revenue;
                toUpdate.Expenses = item.Expenses;
                toUpdate.Period = item.Period;
                var profit = item.Revenue - item.Expenses;
                toUpdate.Profit = profit;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var toDelete = await _dbContext.TradePointEconomics
                .FirstOrDefaultAsync(d => d.TradePointEconomicsId == id);

            if (toDelete != null)
            {
                _dbContext.TradePointEconomics.Remove(toDelete);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<List<TradePointEconomics>> SearchAsync(string search, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = _dbContext.TradePointEconomics
            .Include(e => e.TradePoint) // Включаем связанную сущность
            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                if (decimal.TryParse(search, out var value))
                {
                    query = query.Where(x => x.Profit == value
                    || x.Revenue == value || x.Expenses == value);
                }
                else
                {
                    query = query.Where(x => x.TradePoint.Name.Contains(search));
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
