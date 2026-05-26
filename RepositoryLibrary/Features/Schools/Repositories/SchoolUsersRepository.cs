using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly EM_DbContext _emContext;

        public UserPhotoRepository(EM_DbContext context)
        {
            _emContext = context;
        }

       
  

        public async Task<List<Photo>> GetAllPhotosAsync()
        {
            return await _emContext.Photos.ToListAsync();

        }
        public async Task<Photo?> GetPhotoByUserId(string id)
        {
            return await _emContext.Photos
                 .FirstOrDefaultAsync(x => x.UserId == id);
        }
        public async Task<int> DeletePhotoByUserIdAsync(string id)
        {
            var userToDelete = await _emContext.Photos
                .FirstOrDefaultAsync(x => x.UserId == id);

            if (userToDelete == null)
                return 0;

            _emContext.Photos.Remove(userToDelete);

            return await _emContext.SaveChangesAsync();
        }
    }
}
