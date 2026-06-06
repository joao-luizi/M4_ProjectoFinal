using RepositoryLibrary.Features.Schools.DTOs;
using RepositoryLibrary.Features.Schools.Entities;
using System;

namespace RepositoryLibrary.Features.Schools.Interfaces;

public interface ISchoolService
{

    public Task<School> CreateSchoolAsync(School school);
    public Task CreateUserSchoolAsync(string userId, int schoolId);
    public Task<School> GetSchoolByIdAsync(int schoolId);
    public Task<School> EditSchoolByIdAsync(School school);
    public Task<School> DeleteSchoolByIdAsync(int schoolId);
    //V2 Implemented
    Task<List<School>> GetSchoolsAsync();
    //V2 Implemented
    public Task<List<SchoolListDto>> GetSchoolsListAsync();
    public Task<List<School>> GetUserSchoolsAsync(string userId);

}
