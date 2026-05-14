using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RepositoryLibrary.IRepository;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using RepositoryLibrary.Models.DTOs;
using SharedLibrary;
using System.Data;
using System.Threading.Tasks;




namespace RepositoryLibrary.Repository
{
    public class SchoolUsersRepository : ISchoolUsersRepository
    {
        private readonly EM_DbContext _emContext;

        public SchoolUsersRepository(EM_DbContext context)
        {
            _emContext = context;
        }

        public async Task<List<SchoolUser>> GetAllUsers(int schoolId)
        {
            return await _emContext.SchoolUsers.ToListAsync();

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
