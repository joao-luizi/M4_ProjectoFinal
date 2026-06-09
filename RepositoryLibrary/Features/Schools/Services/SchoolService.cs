using Microsoft.Extensions.Logging;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Images.Interfaces;
using RepositoryLibrary.Features.Schools.DTOs;
using RepositoryLibrary.Features.Schools.Entities;
using RepositoryLibrary.Features.Schools.Interfaces;
using RepositoryLibrary.Features.Schools.Repositories;
using RepositoryLibrary.Features.Users.DTOs;

namespace RepositoryLibrary.Features.Schools.Services;

public class SchoolService : ISchoolService
{
    private readonly ISchoolRepository _schoolRepo;
    private readonly IImageService _imageService;
    private readonly ILogger<SchoolService> _logger;

    public SchoolService(IImageService imageService, ISchoolRepository schoolRepo, ILogger<SchoolService> logger)
    {
        _imageService = imageService;
        _schoolRepo = schoolRepo;
        _logger = logger;
    }

    //TODO
    private async Task CreateSchoolAsync(AdminSchoolDetailsDto dto)
    {
        var school = new School
        {
            SchoolName = dto.SchoolName,
            Address = dto.Address,
            Contact = dto.Contact,
            Email = dto.Email,
            CAE = dto.CAE,
            SchoolCapacity = dto.SchoolCapacity
        };

        // 1. Persistir escola
        await _schoolRepo.AddAsync(school);


        // 2. Foto (se existir)
        if (dto.NewPhoto is not null)
        {
            var photoPath = await _imageService.SaveImageAsync(
                dto.NewPhoto,
                folder: "schools",
                fileName: school.SchoolId.ToString());

            var photo = new SchoolPhoto
            {
                SchoolId = school.SchoolId,
                FotoPath = photoPath
            };

            await _schoolRepo.AddAsync(photo);
        }
    }

    private async Task UpdateSchoolAsync(AdminSchoolDetailsDto dto)
    {
        if (dto is null)
            return;

        var existingSchool = await _schoolRepo.GetSchoolAsync(dto.SchoolId);

        if (existingSchool is null)
            return;

        // 1. Atualizar dados base
        existingSchool.SchoolName = dto.SchoolName;
        existingSchool.Address = dto.Address;
        existingSchool.Contact = dto.Contact;
        existingSchool.Email = dto.Email;
        existingSchool.CAE = dto.CAE;
        existingSchool.SchoolCapacity = dto.SchoolCapacity;

        await _schoolRepo.UpdateAsync(existingSchool);

        // 2. Se não há imagem nova, termina
        if (dto.NewPhoto is null)
            return;

        // 3. Foto existente (se houver)
        var existingPhoto = await _schoolRepo.GetSchoolPhotoByIsAsync(dto.SchoolId);

        string newPath = await _imageService.ReplaceImageAsync(
            dto.NewPhoto,
            folder: "schools",
            fileName: dto.SchoolId.ToString(),
            existingImagePath: existingPhoto?.FotoPath);

        // 4. Persistência 1–1 (PK = FK)
        if (existingPhoto is null)
        {
            await _schoolRepo.AddAsync(new SchoolPhoto
            {
                SchoolId = dto.SchoolId,
                FotoPath = newPath
            });
        }
        else
        {
            existingPhoto.FotoPath = newPath;

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

    public async Task<AdminSchoolDetailsDto> GetAdminSchoolByIdAsync(int schoolId)
    {
        _logger.LogInformation("A obter a escola {SchoolId}.", schoolId);

        try
        {
            var school = await _schoolRepo.GetSchoolAsync(schoolId);

            if (school is null)
            {
                _logger.LogWarning("Escola {SchoolId} não encontrada.", schoolId);
                throw new KeyNotFoundException($"School with id {schoolId} was not found.");
            }

            return new AdminSchoolDetailsDto
            {
                SchoolId = school.SchoolId,
                SchoolName = school.SchoolName,
                Address = school.Address,
                Contact = school.Contact,
                Email = school.Email,
                CAE = school.CAE,
                SchoolCapacity = school.SchoolCapacity,
                ExistingPhotoPath = school.SchoolPhoto?.FotoPath
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao obter a escola {SchoolId}.", schoolId);
            throw;
        }
    }

    public async Task<List<School>> GetSchoolsAsync()
    {
        _logger.LogInformation("A obter a lista de todas as escolas.");
        try
        {
            var schools = await _schoolRepo.GetSchoolsAsync();

            _logger.LogInformation("Obtidas {Count} escolas.", schools.Count);
            return schools;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao obter a lista de escolas.");
            throw;
        }
    }

    public async Task<List<SelectSchoolDto>> GetSchoolsListAsync()
    {
        _logger.LogInformation("A obter a lista de todas as escolas.");
        try
        {
            var schools = await _schoolRepo.GetSchoolsAsync();

            _logger.LogInformation("Obtidas {Count} escolas.", schools.Count);
            return schools.Select (s => new SelectSchoolDto
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


    //V2 Implelmented
    public async Task SaveSchoolAsync(AdminSchoolDetailsDto dto)
    {
        if (dto.SchoolId == 0)
        {
            // create
            await CreateSchoolAsync(dto);
        }
        else
        {
            // update
            await UpdateSchoolAsync(dto);
        }
    }

    public async Task<List<AdminSchoolListItemDto>> GetAdminSchoolsAsync()
    {
        _logger.LogInformation("A obter a lista de todas as escolas.");

        try
        {
            var schools = await _schoolRepo.GetSchoolsAsync();

            var result = schools.Select(s => new AdminSchoolListItemDto
            {
                SchoolId = s.SchoolId,
                SchoolName = s.SchoolName,
                Contact = s.Contact,
                Email = s.Email,
                SchoolCapacity = s.SchoolCapacity,
              
            }).ToList();

            _logger.LogInformation("Obtidas {Count} escolas.", result.Count);

            return result;
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