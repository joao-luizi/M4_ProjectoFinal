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

        //V2 Implemented
        Task<UserHorse?> GetUserHorseByHorseId(int horseId);

        //V2 Implemented
        Task<UserHorse?> GetUserHorseByUserId(string userId);
        //V2 Implemented
        Task AddAsync(UserHorse userHorse);

        //V2 Implemented
        Task Delete(UserHorse userHorse);

        //V2 Implemented
        Task<List<Horse>> GetAllBySchoolAsync(int schoolId);


    }
}
