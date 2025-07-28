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
    public class TradePointStorage : ITradePointStorage
    {
        private InformationSystemDbContext _dbContext;

        public TradePointStorage(InformationSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<TradePoint>> GetAllTradePointsAsync()
        {
            return await _dbContext.TradePoints.ToListAsync();
        }

        public async Task<TradePoint> GetAsync(Guid id)
        {
            var toGet = await _dbContext.TradePoints
                .Where(d => d.TradePointId == id).FirstOrDefaultAsync();

            return toGet;
        }
        public async Task<Guid> AddAsync(TradePoint item)
        {
            if (item.TradePointId == Guid.Empty)
            {
                item.TradePointId = Guid.NewGuid();
            }

            await _dbContext.TradePoints.AddAsync(item);
            await _dbContext.SaveChangesAsync();
            return item.TradePointId;
        }

        public async Task UpdateAsync(Guid id, TradePoint item)
        {
            var toUpdate = await _dbContext.TradePoints
               .FirstOrDefaultAsync(d => d.TradePointId == id);

            if (toUpdate != null)
            {
                toUpdate.Name = item.Name;
                toUpdate.Address = item.Address;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var toDelete = await _dbContext.TradePoints
                .FirstOrDefaultAsync(d => d.TradePointId == id);

            if (toDelete != null)
            {
                _dbContext.TradePoints.Remove(toDelete);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<TradePoint>> SearchAsync(string search, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = _dbContext.TradePoints.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {

                query = query.Where(x => x.Name.Contains(search) ||
                x.Address.Contains(search));
            }

            return await query.ToListAsync();
        }
    }
}
