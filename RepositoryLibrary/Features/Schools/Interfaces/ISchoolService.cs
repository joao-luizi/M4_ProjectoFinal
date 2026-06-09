using RepositoryLibrary.Features.Schools.DTOs;
using RepositoryLibrary.Features.Schools.Entities;
using System;

namespace RepositoryLibrary.Features.Schools.Interfaces;

public interface ISchoolService
{

    
    Task CreateUserSchoolAsync(string userId, int schoolId);
    Task<School> EditSchoolByIdAsync(School school);
    Task<School> DeleteSchoolByIdAsync(int schoolId);
    //V2 Implemented
    Task<List<School>> GetSchoolsAsync();
    //V2 Implemented
    Task<List<SelectSchoolDto>> GetSchoolsListAsync();

    //V2 Implemented
    Task<AdminSchoolDetailsDto> GetAdminSchoolByIdAsync(int schoolId);
    //V2 Implemented
    Task<List<AdminSchoolListItemDto>> GetAdminSchoolsAsync();
    Task SaveSchoolAsync(AdminSchoolDetailsDto dto);
    Task<List<School>> GetUserSchoolsAsync(string userId);

}
