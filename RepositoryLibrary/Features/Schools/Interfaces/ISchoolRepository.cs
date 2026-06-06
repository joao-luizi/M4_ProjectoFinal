using RepositoryLibrary.Features.Schools.Entities;

namespace RepositoryLibrary.Features.Schools.Interfaces
{
    public interface ISchoolRepository
    {
        public Task CreateUserSchoolAsync(string userId, int schoolId);
        //public Task<SchoolPhoto> AddSchoolLogoAsync(int schoolId, string logoName, string filepath);
        //public Task<SchoolPhoto> UpdateSchoolLogoAsync(SchoolPhoto logoToChange, string filepath);
        //public Task<SchoolPhoto> DeleteSchoolLogoAsync(int schoolId, string logoName);
        //public Task<SchoolPhoto> GetSchoolLogoAsync(int schoolId, string logoName);
        //public Task<List<SchoolPhoto>> GetAllSchoolLogosAsync(int schoolId);

        public Task<School> CreateSchoolAsync(School school);
        public Task<School> GetSchoolAsync(int schoolId);
        public Task<School> EditSchoolAsync(School school);
        public Task<School> DeleteSchoolAsync(School school);
        public Task<List<School>> GetUserSchoolsAsync(string userId);
        public Task<List<School>> GetSchoolsAsync();
    }
}
