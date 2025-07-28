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
    public class FinishedGoodsWarehouseStorage : IFinishedGoodsWarehouseStorage
    {
        private InformationSystemDbContext _dbContext;

        public FinishedGoodsWarehouseStorage(InformationSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<FinishedGoodsWarehouse>> GetAllFinishedGoodsAsync()
        {
            return await _dbContext.FinishedGoodsWarehouse.ToListAsync();
        }

        public async Task<Guid> AddAsync(FinishedGoodsWarehouse item)
        {

            if (item.ProductId == Guid.Empty)
            {
                item.ProductId = Guid.NewGuid();
            }

            await _dbContext.FinishedGoodsWarehouse.AddAsync(item);
            await _dbContext.SaveChangesAsync();
            return item.ProductId;
        }

        public async Task UpdateAsync(Guid id, FinishedGoodsWarehouse finishedGoodsWarehouse)
        {
            var toUpdate = await _dbContext.FinishedGoodsWarehouse
                .FirstOrDefaultAsync(d => d.ProductId == id);

            if (toUpdate != null)
            {
                toUpdate.Name = finishedGoodsWarehouse.Name;
                toUpdate.Quantity = finishedGoodsWarehouse.Quantity;
                toUpdate.PotentialRevenue = finishedGoodsWarehouse.PotentialRevenue;
                toUpdate.LastUpdated = finishedGoodsWarehouse.LastUpdated;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var toDelete = await _dbContext.FinishedGoodsWarehouse
               .FirstOrDefaultAsync(d => d.ProductId == id);

            if (toDelete != null)
            {
                _dbContext.FinishedGoodsWarehouse.Remove(toDelete);
                await _dbContext.SaveChangesAsync();
            }
        }
    public async Task<FinishedGoodsWarehouse> GetAsync(Guid id)
        {
            var toGet = await _dbContext.FinishedGoodsWarehouse
                .Where(d => d.ProductId == id).FirstOrDefaultAsync();
            return toGet;
        }

        public async Task<List<FinishedGoodsWarehouse>> GetFinishedGoodsByPeriodAsync(DateTime period)
        {
            var toGet = await _dbContext.FinishedGoodsWarehouse
                .Where(d => d.LastUpdated == period)
                .ToListAsync();
            return toGet;
        }

        public async Task<List<FinishedGoodsWarehouse>> SearchAsync(string search, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = _dbContext.FinishedGoodsWarehouse.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                if (decimal.TryParse(search, out var value))
                {
                    query = query.Where(x => x.PotentialRevenue == value);
                }
                else if(int.TryParse(search, out var intvalue))
                {
                    query = query.Where(x => x.Quantity == intvalue);
                }
                else
                {
                    query = query.Where(x => x.Name.Contains(search));
                }
            }

            if (dateFrom.HasValue)
                query = query.Where(x => x.LastUpdated >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(x => x.LastUpdated <= dateTo.Value);

            return await query.ToListAsync();
        }
    }
}
