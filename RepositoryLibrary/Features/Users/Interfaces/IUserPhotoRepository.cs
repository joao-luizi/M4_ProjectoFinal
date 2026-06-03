using RepositoryLibrary.Features.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Users.Interfaces
{
    public interface IUserPhotoRepository
    {
        Task<UserPhoto?> GetByUserIdAsync(string userId);

        Task SaveChangesAsync();

        Task AddAsync(UserPhoto entity);

        Task DeleteAsync(UserPhoto entity);
    }
}
