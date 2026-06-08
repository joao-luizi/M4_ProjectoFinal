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

        //V2 Implemented
         Task AddAsync(School school);

        //V2 Implemented
        Task AddAsync(SchoolPhoto foto);
        //V2 Implemented
        Task UpdateAsync(SchoolPhoto foto);
        //V2 Implemented
        Task UpdateAsync(School school);
        Task<SchoolPhoto?> GetSchoolPhotoByIsAsync(int schoolId);
        Task<School> CreateSchoolAsync(School school);
        Task<School> GetSchoolAsync(int schoolId);
        Task<School> EditSchoolAsync(School school);
        Task<School> DeleteSchoolAsync(School school);
        Task<List<School>> GetUserSchoolsAsync(string userId);
        Task<List<School>> GetSchoolsAsync();
    }
}
