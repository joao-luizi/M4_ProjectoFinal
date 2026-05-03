using RepositoryLibrary.Models;
using RepositoryLibrary.Models.DTOs;

namespace RepositoryLibrary.IRepository
{
    public interface ISchoolRepository
    {
        public Task CreateUserSchoolAsync(string userId, int schoolId);
        public Task<Logo> AddSchoolLogoAsync(int schoolId, string logoName, string filepath);
        public Task<Logo> UpdateSchoolLogoAsync(Logo logoToChange, string filepath);
        public Task<Logo> DeleteSchoolLogoAsync(int schoolId, string logoName);
        public Task<Logo> GetSchoolLogoAsync(int schoolId, string logoName);
        public Task<List<Logo>> GetAllSchoolLogosAsync(int schoolId);
        public Task<School> GetSchoolAsync(int schoolId);
        public Task<School> EditSchoolAsync(School school);
        public Task<School> DeleteSchoolAsync(School school);
        public Task<List<School>> GetUserSchoolsAsync(string userId);
        public Task<List<School>> GetSchoolsAsync();

    }
}
