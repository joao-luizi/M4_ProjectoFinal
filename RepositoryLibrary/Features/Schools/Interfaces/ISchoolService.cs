using RepositoryLibrary.Features.Schools.Entities;
using System;

namespace RepositoryLibrary.Features.Schools.Interfaces;

public interface ISchoolService
{
    public Task CreateUserSchoolAsync(string userId, int schoolId);
    public Task<School> GetSchoolByIdAsync(int schoolId);
    public Task<School> EditSchoolByIdAsync(School school);
    public Task<School> DeleteSchoolByIdAsync(int schoolId);
    public Task<List<School>> GetSchoolsAsync();
    public Task<List<School>> GetUserSchoolsAsync(string userId);

}
