using InformationSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InformationSystem.Application.Interfaces;

namespace Infrastructure.Storage
{
    public class ExpenseStorage : IExpenseStorage
    {
        private InformationSystemDbContext _dbContext;

        public ExpenseStorage(InformationSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Expense>> GetAllExpensesAsync()
        {
            return await _dbContext.Expenses.ToListAsync();
        }

        public async Task<List<Expense>> GetExpenseByPeriodAsync(DateTime period)
        {
            var toGet = await _dbContext.Expenses
                .Where(d => d.Date == period)
                .ToListAsync();
            return toGet;
        }

        public async Task<Expense> GetAsync(Guid id)
        {
            var toGet = await _dbContext.Expenses
                .Where(d => d.ExpenseId == id).FirstOrDefaultAsync();
            
            return toGet;
        }

        public async Task<Guid> AddAsync(Expense expense)
        {
            if (expense.ExpenseId == Guid.Empty)
            {
                expense.ExpenseId = Guid.NewGuid();
            }
            
            var balanceAnalysis = await _dbContext.BalanceAnalysis
                .FirstOrDefaultAsync(b => b.Period.Date == expense.Date.Date);

            if (balanceAnalysis == null)
            {
                balanceAnalysis = new BalanceAnalysis
                {
                    BalanceId = Guid.NewGuid(),
                    Period = expense.Date,
                    TotalExpense = expense.Amount,
                    TotalIncome = 0, // чтобы избежать null
                    ProfitOrLoss = 0 - expense.Amount
                };

                _dbContext.BalanceAnalysis.Add(balanceAnalysis);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                balanceAnalysis.TotalExpense += expense.Amount;
                balanceAnalysis.ProfitOrLoss = balanceAnalysis.TotalIncome - balanceAnalysis.TotalExpense;
            }


            // Теперь присваиваем правильный BalanceAnalysisId
            expense.BalanceAnalysisId = balanceAnalysis.BalanceId;

            await _dbContext.Expenses.AddAsync(expense);
            await _dbContext.SaveChangesAsync();
            return expense.ExpenseId;
        }

        public async Task UpdateAsync(Guid id, Expense item)
        {
            Expense toUpdate = await _dbContext.Expenses
                .FirstOrDefaultAsync(d => d.ExpenseId == id);
            
            var balanceAnalysis = await _dbContext.BalanceAnalysis
                .FirstOrDefaultAsync(b => b.Period.Date == toUpdate.Date.Date);
            
            if (toUpdate != null)
            {
                
                toUpdate.ExpenseName = item.ExpenseName;
                toUpdate.Amount = item.Amount;
                toUpdate.Date = item.Date;
                toUpdate.Category = item.Category;
                
                balanceAnalysis.TotalExpense = toUpdate.Amount;
                balanceAnalysis.ProfitOrLoss = balanceAnalysis.TotalIncome - balanceAnalysis.TotalExpense;

                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var toDelete = await _dbContext.Expenses
                .FirstOrDefaultAsync(d => d.ExpenseId == id);

            if (toDelete == null)
            {
                throw new InvalidOperationException($"Доход с ID {id} не найден.");
            }

            var balanceAnalysis = await _dbContext.BalanceAnalysis
                .FirstOrDefaultAsync(b => b.Period.Date == toDelete.Date.Date);

            if (balanceAnalysis != null)
            {
                balanceAnalysis.TotalExpense -= toDelete.Amount;
                balanceAnalysis.ProfitOrLoss = balanceAnalysis.TotalIncome - balanceAnalysis.TotalExpense;
                
                _dbContext.BalanceAnalysis.Update(balanceAnalysis);
            }

            _dbContext.Expenses.Remove(toDelete);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Expense>> SearchAsync(string search, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = _dbContext.Expenses.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                if (decimal.TryParse(search, out var value))
                {
                    query = query.Where(x => x.Amount == value);
                }
                else
                {
                    query = query.Where(x => x.ExpenseName.Contains(search) ||
                    x.Category.Contains(search));
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
