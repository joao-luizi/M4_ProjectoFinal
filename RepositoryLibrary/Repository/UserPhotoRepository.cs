using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<SchoolUsersRepository> _logger;

        public SchoolUsersRepository(RideReadyDbContext context, ILogger<SchoolUsersRepository> logger)
        {
            _emContext = context;
            _logger = logger;
        }

        public async Task<List<SchoolUser>> GetAllUsers(int schoolId)
        {
            _logger.LogInformation("BD: a consultar SchoolUsers da escola {SchoolId}.", schoolId);
            var users = await _emContext.SchoolUsers.Where(x => x.SchoolId == schoolId).ToListAsync();
            _logger.LogInformation("BD: obtidos {Count} SchoolUsers para a escola {SchoolId}.", users.Count, schoolId);
            return users;
        }

        public async Task<SchoolUser?> GetUserById(string id)
        {
            _logger.LogInformation("BD: a consultar SchoolUser {UserId}.", id);
            var user = await _emContext.SchoolUsers
                 .FirstOrDefaultAsync(x => x.UserId == id);

            if (user == null)
            {
                _logger.LogWarning("SchoolUser {UserId} não encontrado.", id);
            }
            else
            {
                _logger.LogInformation("BD: SchoolUser {UserId} obtido.", id);
            }

            return user;
        }


        public async Task<int> DeleteUserAsync(string id)
        {
            _logger.LogInformation("BD: a eliminar SchoolUser {UserId}.", id);
            var userToDelete = await _emContext.SchoolUsers
                .FirstOrDefaultAsync(x => x.UserId == id);

            if (userToDelete == null)
            {
                _logger.LogWarning("SchoolUser {UserId} não encontrado; nada a eliminar.", id);
                return 0;
            }

            _emContext.SchoolUsers.Remove(userToDelete);

            var rowsAffected = await _emContext.SaveChangesAsync();
            _logger.LogInformation("BD: SchoolUser {UserId} eliminado ({Rows} linhas afetadas).", id, rowsAffected);
            return rowsAffected;
        }
    }
}