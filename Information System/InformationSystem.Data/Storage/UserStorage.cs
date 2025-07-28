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
    public class UserStorage : IUserStorage
    {
        private InformationSystemDbContext _dbContext;

        public UserStorage(InformationSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User> GetAsync(Guid id)
        {
            var toGet = await _dbContext.Users
                .Where(d => d.UserId == id).FirstOrDefaultAsync();
            return toGet;
        }

        public async Task<Guid> AddAsync(User item)
        {
            if (item.UserId == Guid.Empty)
            {
                item.UserId = Guid.NewGuid();
            }

            await _dbContext.Users.AddAsync(item);
            await _dbContext.SaveChangesAsync();
            return item.UserId;
        }

        public async Task UpdateAsync(Guid id, User item)
        {
            User toUpdate = await _dbContext.Users
               .FirstOrDefaultAsync(d => d.UserId == id);

            if (toUpdate != null)
            {
                toUpdate.Username = item.Username;
                toUpdate.PasswordHash = item.PasswordHash;
                toUpdate.Role = item.Role;

                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var toDelete = await _dbContext.Users
                .FirstOrDefaultAsync(d => d.UserId == id);

            if (toDelete != null)
            {
                _dbContext.Users.Remove(toDelete);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<User>> SearchAsync(string search, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = _dbContext.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                    query = query.Where(x => x.Username.Contains(search) ||
                    x.PasswordHash.Contains(search) ||
                    x.Role.Contains(search));
            }

            return await query.ToListAsync();
        }
    }
}
