using RepositoryLibrary.IServices;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using RepositoryLibrary.Repository;

namespace RepositoryLibrary.Services;

public class SchoolService : ISchoolService
{
    private readonly SchoolRepository _schoolRepo;

    public SchoolService(EM_DbContext context)
    {
        _schoolRepo = new SchoolRepository(context);
    }

    public async Task CreateUserSchoolAsync(string userId, int schoolId)
    {
        try
        {
            await _schoolRepo.CreateUserSchoolAsync(userId, schoolId);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task<School> DeleteSchoolByIdAsync(int schoolId)
    {
        try
        {
            var school = await _schoolRepo.GetSchoolAsync(schoolId);
            return await _schoolRepo.DeleteSchoolAsync(school);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task<School> EditSchoolByIdAsync(School school)
    {
        try
        {
            var schoolToEdit = await _schoolRepo.GetSchoolAsync(school.SchoolId) ?? throw new Exception("School doesn't exist, can't edit");
            return await _schoolRepo.EditSchoolAsync(school);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public Task<School> GetSchoolByIdAsync(int schoolId)
    {
        try
        {
            return _schoolRepo.GetSchoolAsync(schoolId);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task<List<School>> GetSchoolsAsync()
    {
        try
        {
            return await _schoolRepo.GetSchoolsAsync();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task<List<School>> GetUserSchoolsAsync(string userId)
    {
        return await _schoolRepo.GetUserSchoolsAsync(userId);
    }
}
