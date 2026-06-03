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
    public class UserPhotoRepository : IUserPhotoRepository
    {
        private readonly RideReadyDbContext _context;

        public UserPhotoRepository(RideReadyDbContext context)
        {
            _context = context;
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public async Task<UserPhoto?> GetByUserIdAsync(string userId)
        {
            return await _context.Set<UserPhoto>()
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task AddAsync(UserPhoto entity)
        {
            await _context.Set<UserPhoto>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(UserPhoto entity)
        {
            _context.Set<UserPhoto>().Remove(entity);
            await _context.SaveChangesAsync();
        }


    }
}
