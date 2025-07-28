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
    public class IncidentStorage : IIncidentStorage
    {
        private InformationSystemDbContext _dbContext;

        public IncidentStorage(InformationSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Incident>> GetAllIncidentsAsync()
        {
            return await _dbContext.Incidents.ToListAsync();
        }

        public async Task<Incident> GetAsync(Guid id)
        {
            var toGet = await _dbContext.Incidents
                .Where(d => d.IncidentId == id).FirstOrDefaultAsync();
            return toGet;
        }

        public async Task<List<Incident>> GetIncidentByDateAsync(DateTime period)
        {
            var toGet = await _dbContext.Incidents
                .Where(d => d.IncidentDate == period)
                .ToListAsync();
            return toGet;
        }

        public async Task<Guid> AddAsync(Incident item)
        {
            if (item.IncidentId == Guid.Empty)
            {
                item.IncidentId = Guid.NewGuid();
            }

            await _dbContext.Incidents.AddAsync(item);
            await _dbContext.SaveChangesAsync();
            return item.IncidentId;
        }

        public async Task UpdateAsync(Guid id, Incident item)
        {
            Incident toUpdate = await _dbContext.Incidents
               .FirstOrDefaultAsync(d => d.IncidentId == id);

            if (toUpdate != null)
            {
                toUpdate.IncidentDate = item.IncidentDate;
                toUpdate.IncidentDescription = item.IncidentDescription;
                toUpdate.IncidentName = item.IncidentName;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var toDelete = await _dbContext.Incidents
                .FirstOrDefaultAsync(d => d.IncidentId == id);

            if (toDelete != null)
            {
                _dbContext.Incidents.Remove(toDelete);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<Incident>> SearchAsync(string search, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = _dbContext.Incidents.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                
                    query = query.Where(x => x.IncidentName.Contains(search) ||
                    x.IncidentDescription.Contains(search));
            }

            if (dateFrom.HasValue)
                query = query.Where(x => x.IncidentDate >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(x => x.IncidentDate <= dateTo.Value);

            return await query.ToListAsync();
        }

    }
}
