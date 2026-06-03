using Microsoft.EntityFrameworkCore;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Users.DTOs;
using RepositoryLibrary.Features.Users.Entities;
using RepositoryLibrary.Features.Users.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Users.Repositories
{
    public class SchoolUsersRepository : ISchoolUsersRepository
    {
        private readonly RideReadyDbContext _context;

        public SchoolUsersRepository(RideReadyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(string userId, int schoolId)
        {
            return await _context.Set<SchoolUser>()
                .AsNoTracking()
                .AnyAsync(x => x.UserId == userId && x.SchoolId == schoolId);
        }

        public async Task AddAsync(SchoolUser entity)
        {
            await _context.Set<SchoolUser>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<SchoolUser?> GetAsync(string userId, int schoolId)
        {
            return await _context.Set<SchoolUser>()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.SchoolId == schoolId);
        }

        public async Task DeleteAsync(SchoolUser entity)
        {
            _context.Set<SchoolUser>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserListDto>> GetUsersBySchoolAsync(int schoolId)
        {
            return await _context.Set<SchoolUser>()
                .AsNoTracking()
                .Where(su => su.SchoolId == schoolId)
                .Select(su => new UserListDto
                {
                    Id = su.User.Id,
                    FirstName = su.User.FirstName,
                    LastName = su.User.LastName,
                    Email = su.User.Email,
                    IsActive = su.User.IsActive
                })
                .ToListAsync();
        }

        public async Task<List<UserSchoolDto>> GetSchoolsByUserAsync(string userId)
        {
            return await _context.Set<SchoolUser>()
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => new UserSchoolDto
                {
                    SchoolId = x.SchoolId,
                    SchoolName = x.School.SchoolName
                })
                .ToListAsync();
        }

    }
}
