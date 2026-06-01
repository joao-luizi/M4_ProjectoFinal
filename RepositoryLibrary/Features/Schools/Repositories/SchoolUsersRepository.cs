using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Users;
using RepositoryLibrary.IRepository;

using System.Data;
using System.Threading.Tasks;




namespace RepositoryLibrary.Features.Schools.Repositories
{
    public class UserPhotoRepository : IUserPhotoRepository
    {
        private readonly RideReadyDbContext _emContext;
        private readonly ILogger<UserPhotoRepository> _logger;

        public UserPhotoRepository(RideReadyDbContext context, ILogger<UserPhotoRepository> logger)
        {
            _emContext = context;
            _logger = logger;
        }




        public async Task<List<Photo>> GetAllPhotosAsync()
        {
            _logger.LogInformation("BD: a consultar todas as fotos.");
            var photos = await _emContext.Photos.ToListAsync();
            _logger.LogInformation("BD: obtidas {Count} fotos.", photos.Count);
            return photos;
        }

        public async Task<Photo?> GetPhotoByUserId(string id)
        {
            _logger.LogInformation("BD: a consultar foto do utilizador {UserId}.", id);
            var photo = await _emContext.Photos
                 .FirstOrDefaultAsync(x => x.UserId == id);

            if (photo == null)
            {
                _logger.LogWarning("Foto do utilizador {UserId} não encontrada.", id);
            }
            else
            {
                _logger.LogInformation("BD: foto do utilizador {UserId} obtida.", id);
            }

            return photo;
        }

        public async Task<int> DeletePhotoByUserIdAsync(string id)
        {
            _logger.LogInformation("BD: a eliminar foto do utilizador {UserId}.", id);
            var userToDelete = await _emContext.Photos
                .FirstOrDefaultAsync(x => x.UserId == id);

            if (userToDelete == null)
            {
                _logger.LogWarning("Foto do utilizador {UserId} não encontrada; nada a eliminar.", id);
                return 0;
            }

            _emContext.Photos.Remove(userToDelete);

            var rowsAffected = await _emContext.SaveChangesAsync();
            _logger.LogInformation("BD: foto do utilizador {UserId} eliminada ({Rows} linhas afetadas).", id, rowsAffected);
            return rowsAffected;
        }
    }
}