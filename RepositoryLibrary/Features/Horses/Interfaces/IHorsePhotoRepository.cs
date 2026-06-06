using RepositoryLibrary.Features.Horses.Entities;
using RepositoryLibrary.Features.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Horses.Interfaces
{
    public interface IHorsePhotoRepository
    {
        //V2 Implemented
        Task<HorseFoto?> GetByHorseIdAsync(int horseId);
        //V2 Implemented
        Task SaveChangesAsync();
        //V2 Implemented
        Task AddAsync(HorseFoto entity);
        //V2 Implemented
        Task DeleteAsync(HorseFoto entity);
    }
}
