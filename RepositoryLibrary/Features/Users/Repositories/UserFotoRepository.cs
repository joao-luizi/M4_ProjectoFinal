using Microsoft.EntityFrameworkCore;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Users.Entities;
using RepositoryLibrary.Features.Users.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Users.Repositories
{
    public class UserFotoRepository : IUserFotoRepository
    {
        private readonly RideReadyDbContext _context;

        public UserFotoRepository(RideReadyDbContext context)
        {
            _context = context;
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public async Task<UserFoto?> GetByUserIdAsync(string userId)
        {
            return await _context.Set<UserFoto>()
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task AddAsync(UserFoto entity)
        {
            await _context.Set<UserFoto>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(UserFoto entity)
        {
            _context.Set<UserFoto>().Remove(entity);
            await _context.SaveChangesAsync();
        }


    }
}
