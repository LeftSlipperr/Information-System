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
    public class LogStorage : ILogStorage
    {
        private readonly InformationSystemDbContext _dbContext;

        public LogStorage(InformationSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddLogAsync(Log log)
        {
            await _dbContext.Logs.AddAsync(log);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Log>> GetAllLogsAsync()
        {
            return await _dbContext.Logs.OrderByDescending(x => x.Timestamp).ToListAsync();
        }

        public async Task<List<Log>> SearchAsync(string search, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = _dbContext.Logs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                    query = query.Where(x => x.Action.Contains(search));
            }

            if (dateFrom.HasValue)
                query = query.Where(x => x.Timestamp >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(x => x.Timestamp <= dateTo.Value);

            return await query.ToListAsync();
        }
    }
}
