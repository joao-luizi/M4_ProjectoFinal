using Microsoft.Extensions.Logging;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Schools.DTOs;
using RepositoryLibrary.Features.Schools.Entities;
using RepositoryLibrary.Features.Schools.Interfaces;
using RepositoryLibrary.Features.Schools.Repositories;

namespace RepositoryLibrary.Features.Schools.Services;

public class SchoolService : ISchoolService
{
    private readonly SchoolRepository _schoolRepo;
    private readonly ILogger<SchoolService> _logger;

    public SchoolService(RideReadyDbContext context, ILogger<SchoolService> logger, ILogger<SchoolRepository> repoLogger)
    {
        _schoolRepo = new SchoolRepository(context, repoLogger);
        _logger = logger;
    }

    public async Task<School> CreateSchoolAsync(School school)
    {
        _logger.LogInformation("A criar escola {SchoolName}.", school.SchoolName);

        try
        {
            var created = await _schoolRepo.CreateSchoolAsync(school);

            _logger.LogInformation("Escola {SchoolId} criada com sucesso.", created.SchoolId);

            return created;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao criar escola {SchoolName}.", school.SchoolName);
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task CreateUserSchoolAsync(string userId, int schoolId)
    {
        _logger.LogInformation("A associar o utilizador {UserId} à escola {SchoolId}.", userId, schoolId);
        try
        {
            await _schoolRepo.CreateUserSchoolAsync(userId, schoolId);
            _logger.LogInformation("Utilizador {UserId} associado à escola {SchoolId} com sucesso.", userId, schoolId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao associar o utilizador {UserId} à escola {SchoolId}.", userId, schoolId);
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task<School> DeleteSchoolByIdAsync(int schoolId)
    {
        _logger.LogInformation("A eliminar a escola {SchoolId}.", schoolId);
        try
        {
            var school = await _schoolRepo.GetSchoolAsync(schoolId);
            var deleted = await _schoolRepo.DeleteSchoolAsync(school);
            _logger.LogInformation("Escola {SchoolId} eliminada com sucesso.", schoolId);
            return deleted;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao eliminar a escola {SchoolId}.", schoolId);
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task<School> EditSchoolByIdAsync(School school)
    {
        _logger.LogInformation("A editar a escola {SchoolId}.", school.SchoolId);
        try
        {
            var schoolToEdit = await _schoolRepo.GetSchoolAsync(school.SchoolId) ?? throw new Exception("School doesn't exist, can't edit");
            var edited = await _schoolRepo.EditSchoolAsync(school);
            _logger.LogInformation("Escola {SchoolId} editada com sucesso.", school.SchoolId);
            return edited;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao editar a escola {SchoolId}.", school.SchoolId);
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public Task<School> GetSchoolByIdAsync(int schoolId)
    {
        _logger.LogInformation("A obter a escola {SchoolId}.", schoolId);
        try
        {
            return _schoolRepo.GetSchoolAsync(schoolId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao obter a escola {SchoolId}.", schoolId);
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task<List<SchoolListDto>> GetSchoolsListAsync()
    {
        _logger.LogInformation("A obter a lista de todas as escolas.");
        try
        {
            var schools = await _schoolRepo.GetSchoolsAsync();

            _logger.LogInformation("Obtidas {Count} escolas.", schools.Count);
            return schools.Select (s => new SchoolListDto
            {
                SchoolId = s.SchoolId,
                SchoolName = s.SchoolName,
            }).ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao obter a lista de escolas.");
            throw;
        }
    }

    public async Task<List<School>> GetUserSchoolsAsync(string userId)
    {
        _logger.LogInformation("A obter as escolas do utilizador {UserId}.", userId);
        var schools = await _schoolRepo.GetUserSchoolsAsync(userId);
        _logger.LogInformation("Obtidas {Count} escolas para o utilizador {UserId}.", schools.Count, userId);
        return schools;
    }
}