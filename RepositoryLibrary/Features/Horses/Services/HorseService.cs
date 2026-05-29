using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Horses;
using RepositoryLibrary.Features.Horses.Entities;
using RepositoryLibrary.Features.Horses.Interfaces;
using RepositoryLibrary.Features.Lessons.Interfaces;
using RepositoryLibrary.Features.Schools.Interfaces;
using RepositoryLibrary.Features.Users.DTOs;
using RepositoryLibrary.Repository;
using System.ComponentModel.DataAnnotations;

namespace RepositoryLibrary.Features.Horses.Services;

public class HorseService : IHorseService
{
    private readonly IHorseRepository _horseRepo;
    private readonly ISchoolService _schoolService;
    private readonly ILessonService _lessonService;
    private readonly ILogger<HorseService> _logger;

    public HorseService(RideReadyDbContext dbContext, ISchoolService schoolService, ILessonService lessonService, ILogger<HorseService> logger, ILogger<HorseRepository> repoLogger)
    {
        _horseRepo = new HorseRepository(dbContext, repoLogger);
        _schoolService = schoolService;
        _lessonService = lessonService;
        _logger = logger;
    }

    public async Task<Horse> GetHorseByIdAsync(int horseId)
    {
        _logger.LogInformation("A obter o cavalo {HorseId}.", horseId);

        Horse? backendHorse = await _horseRepo.GetHorseByIdAsync(horseId);

        if (backendHorse == null)
        {
            _logger.LogWarning("Cavalo {HorseId} não encontrado.", horseId);
            throw new InvalidOperationException(
                $"Horse with id {horseId} was not found.");
        }

        _logger.LogInformation("Cavalo {HorseId} obtido com sucesso.", horseId);
        return backendHorse;
    }

    public async Task<Horse> AddHorse(Horse horse, IBrowserFile? file)
    {
        _logger.LogInformation("A adicionar o cavalo '{HorseName}' (com foto: {HasPhoto}).", horse.Name, file != null);
        try
        {
            await _horseRepo.CreateHorse(horse);

            if (file != null)
            {
                var photo = await SavePhoto(file, horse.HorseId);

                horse.HorseFoto = photo;
                await _horseRepo.EditHorse(horse);
            }

            _logger.LogInformation("Cavalo {HorseId} ('{HorseName}') adicionado com sucesso.", horse.HorseId, horse.Name);
            return horse;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao adicionar o cavalo '{HorseName}'.", horse.Name);
            throw new Exception(e.Message, e.InnerException);
        }
    }

    private async Task<HorseFoto> SavePhoto(IBrowserFile file, int horseId)
    {
        _logger.LogInformation("A guardar foto '{FileName}' para o cavalo {HorseId}.", file.Name, horseId);

        const long maxFileSize = 5 * 1024 * 1024;

        string uploadsFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "Images",
            "horsephotos");

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        string extension = Path.GetExtension(file.Name).ToLowerInvariant();

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        if (!allowedExtensions.Contains(extension))
        {
            _logger.LogWarning("Formato de imagem inválido '{Extension}' para o cavalo {HorseId}.", extension, horseId);
            throw new InvalidOperationException("Invalid image format.");
        }

        // Nome fixo por cavalo (1 foto por cavalo)
        string fileName = $"{horseId}{extension}";
        string fullPath = Path.Combine(uploadsFolder, fileName);

        await using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
        {
            await file.OpenReadStream(maxFileSize).CopyToAsync(fs);
        }

        _logger.LogInformation("Foto guardada em '{FotoPath}' para o cavalo {HorseId}.", $"/Images/horsephotos/{fileName}", horseId);

        return new HorseFoto
        {
            HorseId = horseId,
            FotoPath = $"/Images/horsephotos/{fileName}"
        };
    }

    public async Task<List<Horse>> GetHorsesAsync()
    {
        _logger.LogInformation("A obter a lista de todos os cavalos.");
        try
        {
            var horses = await _horseRepo.GetAllHorses();
            _logger.LogInformation("Obtidos {Count} cavalos.", horses.Count);
            return horses;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao obter a lista de cavalos.");
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task<List<Horse>> GetHorsesBySchool(int schoolId)
    {
        _logger.LogInformation("A obter cavalos da escola {SchoolId}.", schoolId);
        try
        {
            var horses = await _horseRepo.GetHorsesBySchool(schoolId);
            _logger.LogInformation("Obtidos {Count} cavalos da escola {SchoolId}.", horses.Count, schoolId);
            return horses;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao obter cavalos da escola {SchoolId}.", schoolId);
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task<List<Horse>> GetHorsesByUser(UpdateUserDto user)
    {
        _logger.LogInformation("A obter cavalos do utilizador {UserId}.", user.Id);
        try
        {
            var horseList = new List<Horse>();
            horseList.AddRange(await _horseRepo.GetHorsesByUser(user.Id));

            var schools = await _schoolService.GetUserSchoolsAsync(user.Id);
            foreach (var school in schools)
            {
                horseList.AddRange(await _horseRepo.GetHorsesBySchool(school.SchoolId));
            }

            _logger.LogInformation("Obtidos {Count} cavalos para o utilizador {UserId}.", horseList.Count, user.Id);
            return horseList;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao obter cavalos do utilizador {UserId}.", user.Id);
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task<bool> IsAvailable(Horse horse, DateTime date, bool Aula = false)
    {
        _logger.LogInformation("A verificar disponibilidade do cavalo {HorseId} para {Date:yyyy-MM-dd} (aula: {IsAula}).", horse.HorseId, date, Aula);

        //create in LessonRepo a way to get lessons by date
        //get bookings for date
        //if 2 bookings of type aula return false
        //if 1 booking of type aula and 1 of type passeio return false
        //if 3/4 bookings return false

        var lessons = await _lessonService.GetLessonByHorseAndDate(horse.HorseId, date);
        // Count the bookings by type.
        // Using StringComparison.OrdinalIgnoreCase ensures that our comparisons ignore casing.
        int aulaCount = lessons.Count(l => l.LessonType != null &&
                                           !l.LessonType.Name.Equals("passeio", StringComparison.OrdinalIgnoreCase));
        int passeioCount = lessons.Count(l => l.LessonType != null &&
                                              l.LessonType.Name.Equals("passeio", StringComparison.OrdinalIgnoreCase));
        int totalCount = lessons.Count();

        // Business Rules:
        // If there are exactly two bookings of type "aula" → cannot book.
        if (aulaCount == 2)
        {
            _logger.LogInformation("Cavalo {HorseId} indisponível em {Date:yyyy-MM-dd}: já tem 2 aulas.", horse.HorseId, date);
            return false;
        }

        // If there's exactly one booking of type "aula" and one of type "passeio" → cannot book.
        if (aulaCount == 1 && passeioCount == 1)
        {
            _logger.LogInformation("Cavalo {HorseId} indisponível em {Date:yyyy-MM-dd}: já tem 1 aula e 1 passeio.", horse.HorseId, date);
            return false;
        }

        if (totalCount == 3 && Aula)
        {
            _logger.LogInformation("Cavalo {HorseId} indisponível para aula em {Date:yyyy-MM-dd}: já tem 3 marcações.", horse.HorseId, date);
            return false;
        }

        // If there are 4 bookings (total) → cannot book.
        if (totalCount == 4)
        {
            _logger.LogInformation("Cavalo {HorseId} indisponível em {Date:yyyy-MM-dd}: limite de 4 marcações atingido.", horse.HorseId, date);
            return false;
        }

        // If none of the above conditions are met, it's valid to book.
        _logger.LogInformation("Cavalo {HorseId} disponível em {Date:yyyy-MM-dd}.", horse.HorseId, date);
        return true;
    }

    public async Task<Horse> RemoveHorse(Horse horse)
    {
        _logger.LogInformation("A remover o cavalo {HorseId}.", horse.HorseId);
        try
        {
            var removed = await _horseRepo.DeleteHorseById(horse.HorseId);
            _logger.LogInformation("Cavalo {HorseId} removido com sucesso.", horse.HorseId);
            return removed;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao remover o cavalo {HorseId}.", horse.HorseId);
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task<Horse> UpdateHorse(Horse horse, IBrowserFile? file)
    {
        _logger.LogInformation("A atualizar o cavalo {HorseId} (com foto: {HasPhoto}).", horse.HorseId, file != null);
        try
        {
            var updatedHorse = await _horseRepo.EditHorse(horse);

            if (file == null)
            {
                _logger.LogInformation("Cavalo {HorseId} atualizado com sucesso (sem foto).", horse.HorseId);
                return updatedHorse;
            }

            var photo = await SavePhoto(file, horse.HorseId);

            // só atualizas isto se quiseres suportar mudança de extensão
            if (updatedHorse.HorseFoto == null)
                updatedHorse.HorseFoto = photo;
            else
                updatedHorse.HorseFoto.FotoPath = photo.FotoPath;

            var result = await _horseRepo.EditHorse(updatedHorse);
            _logger.LogInformation("Cavalo {HorseId} atualizado com sucesso (com nova foto).", horse.HorseId);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao atualizar o cavalo {HorseId}.", horse.HorseId);
            throw new Exception(e.Message, e);
        }
    }

    public async Task<List<Horse>> AvailableHorsesByDate(DateTime date, int schoolId)
    {
        _logger.LogInformation("A obter cavalos disponíveis em {Date:yyyy-MM-dd} para a escola {SchoolId}.", date, schoolId);
        try
        {
            List<Horse> availableHorses = [];
            var horses = await _horseRepo.GetHorsesBySchool(schoolId);
            foreach (var horse in horses)
            {
                if (await IsAvailable(horse, date))
                {
                    availableHorses.Add(horse);
                }
            }
            _logger.LogInformation("Encontrados {Count} cavalos disponíveis em {Date:yyyy-MM-dd} na escola {SchoolId}.", availableHorses.Count, date, schoolId);
            return availableHorses;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao obter cavalos disponíveis em {Date:yyyy-MM-dd} na escola {SchoolId}.", date, schoolId);
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task<List<Horse>> AvailableHorsesByDateAula(DateTime date, int schoolId)
    {
        _logger.LogInformation("A obter cavalos disponíveis para aula em {Date:yyyy-MM-dd} para a escola {SchoolId}.", date, schoolId);
        try
        {
            List<Horse> availableHorses = [];
            var horses = await _horseRepo.GetHorsesBySchool(schoolId);
            foreach (var horse in horses)
            {
                if (await IsAvailable(horse, date, true))
                {
                    availableHorses.Add(horse);
                }
            }
            _logger.LogInformation("Encontrados {Count} cavalos disponíveis para aula em {Date:yyyy-MM-dd} na escola {SchoolId}.", availableHorses.Count, date, schoolId);
            return availableHorses;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao obter cavalos disponíveis para aula em {Date:yyyy-MM-dd} na escola {SchoolId}.", date, schoolId);
            throw new Exception(e.Message, e.InnerException);
        }
    }
}