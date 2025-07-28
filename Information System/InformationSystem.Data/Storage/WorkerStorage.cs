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
    public class WorkerStorage : IWorkerStorage
    {
        private InformationSystemDbContext _dbContext;

        public WorkerStorage(InformationSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Worker>> GetAllWorkersAsync()
        {
            return await _dbContext.Workers
            .Include(w => w.Salary)
            .ToListAsync();
        }

        public async Task<Worker> GetAsync(Guid id)
        {
            var toGet = await _dbContext.Workers
                .Where(d => d.WorkerId == id).FirstOrDefaultAsync();

            return toGet;
        }
        public async Task<Guid> AddAsync(Worker item)
        {
            if (item.WorkerId == Guid.Empty)
            {
                item.WorkerId = Guid.NewGuid();
            }

            await _dbContext.Workers.AddAsync(item);
            await _dbContext.SaveChangesAsync();
            return item.WorkerId;
        }

        public async Task UpdateAsync(Guid id, Worker item)
        {
            var toUpdate = await _dbContext.Workers
               .FirstOrDefaultAsync(d => d.WorkerId == id);

            if (toUpdate != null)
            {
                toUpdate.FirstName = item.FirstName;
                toUpdate.SecondName = item.SecondName;
                toUpdate.ThirdName = item.ThirdName;
                toUpdate.Position = item.Position;
                toUpdate.Salary = item.Salary;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var toDelete = await _dbContext.Workers
                .FirstOrDefaultAsync(d => d.WorkerId == id);

            if (toDelete != null)
            {
                _dbContext.Workers.Remove(toDelete);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<Worker>> SearchAsync(string search, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = _dbContext.Workers
            .Include(e => e.Salary) // Включаем связанную сущность
            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                if (decimal.TryParse(search, out var value))
                {
                    query = query.Where(x => x.Salary.Amount == value);
                }
                else
                {
                    query = query.Where(x => x.Position.Contains(search) ||
                    x.FirstName.Contains(search) || x.SecondName.Contains(search) || x.ThirdName.Contains(search));
                }
            }

            return await query.ToListAsync();
        }
    }
}
