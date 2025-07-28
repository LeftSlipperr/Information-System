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
    public class DebtPayableStorage : IDebtPayableStorage
    {
        private
            InformationSystemDbContext _dbContext;

        public DebtPayableStorage(InformationSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DebtPayable>> GetAllDebtsAsync()
        {
            return await _dbContext.DebtPayables.ToListAsync();
        }

        public async Task<List<DebtPayable>> GetDebtByPeriodAsync(DateTime period)
        {
            var toGet = await _dbContext.DebtPayables
                .Where(d => d.DueDate == period)
                .ToListAsync();
            return toGet;
        }

        public async Task<DebtPayable> GetAsync(Guid id)
        {
            var toGet = await _dbContext.DebtPayables
                .Where(d => d.DebtPayableId == id).FirstOrDefaultAsync();
            return toGet;
        }

        public async Task<Guid> AddAsync(DebtPayable item)
        {
            if (item.DebtPayableId == Guid.Empty)
            {
                item.DebtPayableId = Guid.NewGuid();
            }

            await _dbContext.DebtPayables.AddAsync(item);
            await _dbContext.SaveChangesAsync();
            return item.DebtPayableId;
        }

        public async Task UpdateAsync(Guid id, DebtPayable item)
        {
            DebtPayable toUpdate = await _dbContext.DebtPayables
               .FirstOrDefaultAsync(d => d.DebtPayableId == id);

            if (toUpdate != null)
            {
                toUpdate.CreditorName = item.CreditorName;
                toUpdate.DueDate = item.DueDate;
                toUpdate.Amount = item.Amount;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var toDelete = await _dbContext.DebtPayables
                .FirstOrDefaultAsync(d => d.DebtPayableId == id);

            if (toDelete != null)
            {
                _dbContext.DebtPayables.Remove(toDelete);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<DebtPayable>> SearchAsync(string search, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = _dbContext.DebtPayables.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                if (decimal.TryParse(search, out var value))
                {
                    query = query.Where(x => x.Amount == value);
                }
                else
                {
                    query = query.Where(x => x.CreditorName.Contains(search));
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
