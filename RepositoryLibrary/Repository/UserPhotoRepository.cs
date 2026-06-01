using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Schools.Interfaces;
using RepositoryLibrary.Features.Users.Entities;
using System.Data;




namespace RepositoryLibrary.Repository
{
    public class SchoolUsersRepository : ISchoolUsersRepository
    {
        private readonly RideReadyDbContext _emContext;

        public SchoolUsersRepository(RideReadyDbContext context)
        {
            _emContext = context;
        }

        public async Task<List<SchoolUser>> GetAllUsers(int schoolId)
        {
            return await _emContext.SchoolUsers.Where(x => x.SchoolId == schoolId).ToListAsync();

        }
        public async Task<SchoolUser?> GetUserById(string id)
        {
            return await _emContext.SchoolUsers
                 .FirstOrDefaultAsync(x => x.UserId == id);
        }

        
        public async Task<int> DeleteUserAsync(string id)
        {
            var userToDelete = await _emContext.SchoolUsers
                .FirstOrDefaultAsync(x => x.UserId == id);

            if (userToDelete == null)
                return 0;

            _emContext.SchoolUsers.Remove(userToDelete);

            return await _emContext.SaveChangesAsync();
        }
    }
}
