using Microsoft.EntityFrameworkCore;
using RepositoryLibrary.Features.Horses.Entities;

namespace RepositoryLibrary.Features.Horses.Interfaces
{
    public interface IHorseRepository
    {
        //V2 Implelmented
        Task<Horse?> GetByIdAsync(int horseId);
        //V2 Implelmented
        Task<List<Horse>> GetAllAsync();
        //V2 Implelmented
        Task AddAsync(Horse horse);
        //V2 Implelmented
        Task SaveChangesAsync();
        //V2 Implelmented
        Task<List<Horse>> GetByIdsAsync(List<int> ids);
        //V2 Implelmented
        Task<int> CountAsync();





    }
}
