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
    public class RawMaterialWarehouseStorage : IRawMaterialWarehouseStorage
    {
        private InformationSystemDbContext _dbContext;

        public RawMaterialWarehouseStorage(InformationSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<RawMaterialWarehouse>> GetAllRawMaterialsAsync()
        {
            return await _dbContext.RawMaterialWarehouse.ToListAsync();
        }

        public async Task<List<RawMaterialWarehouse>> GetRawMaterialWarehousesByPeriodAsync(DateTime period)
        {
            var toGet = await _dbContext.RawMaterialWarehouse
                .Where(d => d.LastUpdated == period)
                .ToListAsync();
            return toGet;
        }
        public async Task<RawMaterialWarehouse> GetAsync(Guid id)
        {
            var toGet = await _dbContext.RawMaterialWarehouse
                .Where(d => d.MaterialId == id).FirstOrDefaultAsync();

            return toGet;
        }
        public async Task<Guid> AddAsync(RawMaterialWarehouse item)
        {
            if (item.MaterialId == Guid.Empty)
            {
                item.MaterialId = Guid.NewGuid();
            }

            await _dbContext.RawMaterialWarehouse.AddAsync(item);
            await _dbContext.SaveChangesAsync();
            return item.MaterialId;
        }

        public async Task UpdateAsync(Guid id, RawMaterialWarehouse item)
        {
            var toUpdate = await _dbContext.RawMaterialWarehouse
               .FirstOrDefaultAsync(d => d.MaterialId == id);

            if (toUpdate != null)
            {
                toUpdate.MaterialName = item.MaterialName;
                toUpdate.Quantity = item.Quantity;
                toUpdate.PurchaseCost = item.PurchaseCost;
                toUpdate.LastUpdated = item.LastUpdated;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var toDelete = await _dbContext.RawMaterialWarehouse
                .FirstOrDefaultAsync(d => d.MaterialId == id);

            if (toDelete != null)
            {
                _dbContext.RawMaterialWarehouse.Remove(toDelete);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<RawMaterialWarehouse>> SearchAsync(string search, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = _dbContext.RawMaterialWarehouse.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                if (decimal.TryParse(search, out var value))
                {
                    query = query.Where(x => x.PurchaseCost == value);
                }
                else if(int.TryParse(search, out var intvalue))
                {
                    query = query.Where(x => x.Quantity == intvalue);
                }
                else
                {
                    query = query.Where(x => x.MaterialName.Contains(search));
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
