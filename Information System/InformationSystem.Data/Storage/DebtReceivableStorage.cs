using InformationSystem.Application.Interfaces;
using InformationSystem.Domain.Models;
using InformationSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Storage
{
    public class DebtReceivableStorage : IDebtReceivableStorage
    {
        private InformationSystemDbContext _dbContext;

        public DebtReceivableStorage(InformationSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DebtReceivable>> GetAllDebtsAsync()
        {
            return await _dbContext.DebtReceivables.ToListAsync();
        }

        public async Task<List<DebtReceivable>> GetDebtByPeriodAsync(DateTime period)
        {
            var toGet = await _dbContext.DebtReceivables
                .Where(d => d.DueDate == period)
                .ToListAsync();
            return toGet;
        }

        public async Task<DebtReceivable> GetAsync(Guid id)
        {
            var toGet = await _dbContext.DebtReceivables
                .Where(d => d.DebtReceivableId == id).FirstOrDefaultAsync();
            return toGet;
        }

        public async Task<Guid> AddAsync(DebtReceivable item)
        {
            if (item.DebtReceivableId == Guid.Empty)
            {
                item.DebtReceivableId = Guid.NewGuid();
            }

            await _dbContext.DebtReceivables.AddAsync(item);
            await _dbContext.SaveChangesAsync();
            return item.DebtReceivableId;
        }

        public async Task UpdateAsync(Guid id, DebtReceivable item)
        {
            var toUpdate = await _dbContext.DebtReceivables
               .FirstOrDefaultAsync(d => d.DebtReceivableId == id);

            if (toUpdate != null)
            {
                toUpdate.DebtorName = item.DebtorName;
                toUpdate.Amount = item.Amount;
                toUpdate.DueDate = item.DueDate;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var toDelete = await _dbContext.DebtReceivables
                .FirstOrDefaultAsync(d => d.DebtReceivableId == id);

            if (toDelete != null)
            {
                _dbContext.DebtReceivables.Remove(toDelete);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<List<DebtReceivable>> SearchAsync(string search, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = _dbContext.DebtReceivables.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                if (decimal.TryParse(search, out var value))
                {
                    query = query.Where(x => x.Amount == value);
                }
                else
                {
                    query = query.Where(x => x.DebtorName.Contains(search));
                }
            }

            if (dateFrom.HasValue)
                query = query.Where(x => x.DueDate >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(x => x.DueDate <= dateTo.Value);

            return await query.ToListAsync();
        }
    }
}
