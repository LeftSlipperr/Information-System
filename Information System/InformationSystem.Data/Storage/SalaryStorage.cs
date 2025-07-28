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
    public class SalaryStorage : ISalaryStorage
    {
        private InformationSystemDbContext _dbContext;

        public SalaryStorage(InformationSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Salary>> GetAllSalariesAsync()
        {
            return await _dbContext.Salaries.ToListAsync();
        }

        public async Task<List<Salary>> GetSalaryByPeriodAsync(DateTime period)
        {
            var toGet = await _dbContext.Salaries
                .Where(d => d.PaymentDate == period)
                .ToListAsync();
            return toGet;
        }
        public async Task<Salary> GetAsync(Guid id)
        {
            var toGet = await _dbContext.Salaries
                .Where(d => d.SalaryId == id).FirstOrDefaultAsync();

            return toGet;
        }
        public async Task<Guid> AddAsync(Salary item)
        {
            if (item.SalaryId == Guid.Empty)
            {
                item.SalaryId = Guid.NewGuid();
            }

            await _dbContext.Salaries.AddAsync(item);
            await _dbContext.SaveChangesAsync();
            return item.SalaryId;
        }

        public async Task UpdateAsync(Guid id, Salary item)
        {
            var toUpdate = await _dbContext.Salaries
               .FirstOrDefaultAsync(d => d.SalaryId == id);

            if (toUpdate != null)
            {
                toUpdate.Amount = item.Amount;
                toUpdate.PaymentDate = item.PaymentDate;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var toDelete = await _dbContext.Salaries
                .FirstOrDefaultAsync(d => d.SalaryId == id);

            if (toDelete != null)
            {
                _dbContext.Salaries.Remove(toDelete);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<Salary>> SearchAsync(string search, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = _dbContext.Salaries.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                if (decimal.TryParse(search, out var value))
                {
                    query = query.Where(x => x.Amount == value);
                }
            }

            if (dateFrom.HasValue)
                query = query.Where(x => x.PaymentDate >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(x => x.PaymentDate <= dateTo.Value);

            return await query.ToListAsync();
        }
    }
}
