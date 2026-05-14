using Microsoft.EntityFrameworkCore;
using RepositoryLibrary.Models;

namespace RepositoryLibrary.IRepository
{
    public interface IHorseRepository
    {
        public Task<List<Horse>> GetHorsesByUser(string userId);
        public Task<List<Horse>> GetHorsesBySchool(int schoolId);
        public Task<List<Horse>> GetAllHorses();
        public Task<Horse> CreateHorse(Horse horse);
        public Task<Horse> EditHorse(Horse horse);
        public Task<Horse> DeleteHorseById(int horseId);

        public Task<Horse?> GetHorseByIdAsync(int horseId);

        Task<int> DeleteByUserIdAsync(string userId);


    }
}
