using RepositoryLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.IRepository
{
    public interface IUserPhotoRepository
    {

        public Task<List<Photo>> GetAllPhotosAsync();
        public Task<Photo> GetPhotoByUserId(string id);
        public Task<int> DeletePhotoByUserIdAsync(string id);

        
    }
}
