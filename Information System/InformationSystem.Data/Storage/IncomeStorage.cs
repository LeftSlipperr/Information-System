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
    public class IncomeStorage : IIncomeStorage
    {
        private InformationSystemDbContext _dbContext;

        public IncomeStorage(InformationSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Income>> GetAllIncomesAsync()
        {
            return await _dbContext.Incomes.ToListAsync();
        }

        public async Task<Guid> AddAsync(Income income)
        {
            if (income.IncomeId == Guid.Empty)
            {
                income.IncomeId = Guid.NewGuid();
            }
            
            var balanceAnalysis = await _dbContext.BalanceAnalysis
                .FirstOrDefaultAsync(b => b.Period.Date == income.Date.Date);
            
            if (balanceAnalysis == null)
            {
                balanceAnalysis = new BalanceAnalysis
                {
                    BalanceId = Guid.NewGuid(),  // Добавляем новый ID
                    Period = income.Date,
                    TotalIncome = income.Amount,
                    ProfitOrLoss = income.Amount
                };

                _dbContext.BalanceAnalysis.Add(balanceAnalysis);
                await _dbContext.SaveChangesAsync();  // Создаем запись в BalanceAnalysis
            }
            else
            {
                balanceAnalysis.TotalIncome += income.Amount;
                balanceAnalysis.ProfitOrLoss = balanceAnalysis.TotalIncome - balanceAnalysis.TotalExpense;
            }

            // Теперь присваиваем правильный BalanceAnalysisId
            income.BalanceAnalysisId = balanceAnalysis.BalanceId;

            await _dbContext.Incomes.AddAsync(income);
            await _dbContext.SaveChangesAsync();
            return income.IncomeId;
        }

        public async Task UpdateAsync(Guid id, Income income)
        {
            Income toUpdate = await _dbContext.Incomes
               .FirstOrDefaultAsync(d => d.IncomeId == id);
            
            var balanceAnalysis = await _dbContext.BalanceAnalysis
                .FirstOrDefaultAsync(b => b.Period.Date == toUpdate.Date.Date);
            
            
            if (toUpdate != null)
            {
                toUpdate.Source = income.Source;
                toUpdate.Amount = income.Amount;
                toUpdate.Date = income.Date;
                
                balanceAnalysis.TotalIncome = toUpdate.Amount;
                balanceAnalysis.ProfitOrLoss = balanceAnalysis.TotalIncome - balanceAnalysis.TotalExpense;

                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var toDelete = await _dbContext.Incomes
                .FirstOrDefaultAsync(d => d.IncomeId == id);

            if (toDelete == null)
            {
                throw new InvalidOperationException($"Доход с ID {id} не найден.");
            }

            var balanceAnalysis = await _dbContext.BalanceAnalysis
                .FirstOrDefaultAsync(b => b.Period.Date == toDelete.Date.Date);

            if (balanceAnalysis != null)
            {
                balanceAnalysis.TotalIncome -= toDelete.Amount;
                balanceAnalysis.ProfitOrLoss = balanceAnalysis.TotalIncome - balanceAnalysis.TotalExpense;
                _dbContext.BalanceAnalysis.Update(balanceAnalysis);
            }

            _dbContext.Incomes.Remove(toDelete);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Income>> GetIncomeByPeriodAsync(DateTime period)
        {
            var toGet = await _dbContext.Incomes
                .Where(d => d.Date == period)
                .ToListAsync();
            return toGet;
        }

        public async Task<Income> GetAsync(Guid id)
        {
            var toGet = await _dbContext.Incomes
                .Where(d => d.IncomeId == id).FirstOrDefaultAsync();
            
            return toGet;
        }

        public async Task<List<Income>> SearchAsync(string search, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = _dbContext.Incomes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                if (decimal.TryParse(search, out var value))
                {
                    query = query.Where(x => x.Amount == value);
                }
                else
                {
                    query = query.Where(x => x.Source.Contains(search));
                }
            }

            if (dateFrom.HasValue)
                query = query.Where(x => x.Date >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(x => x.Date <= dateTo.Value);

            return await query.ToListAsync();
        }
    }
}
