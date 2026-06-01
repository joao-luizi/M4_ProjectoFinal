using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
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
    public HorseService(RideReadyDbContext dbContext, ISchoolService schoolService, ILessonService lessonService)
    {
        _horseRepo = new HorseRepository(dbContext);
        _schoolService = schoolService;
        _lessonService = lessonService;
    }

    public async Task<Horse> GetHorseByIdAsync(int horseId)
    {
        Horse? backendHorse = await _horseRepo.GetHorseByIdAsync(horseId);

        if (backendHorse == null)
        {
            throw new InvalidOperationException(
                $"Horse with id {horseId} was not found.");
        }

        return backendHorse;

    }

    public async Task<Horse> AddHorse(Horse horse, IBrowserFile? file)
    {

        try
        {
            await _horseRepo.CreateHorse(horse);

            if (file != null)
            {
                var photo = await SavePhoto(file, horse.HorseId);

                horse.HorseFoto = photo;
                await _horseRepo.EditHorse(horse);

            }
            return horse;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e.InnerException);
        }
    }

    private async Task<HorseFoto> SavePhoto(IBrowserFile file, int horseId)
    {
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
            throw new InvalidOperationException("Invalid image format.");

        // Nome fixo por cavalo (1 foto por cavalo)
        string fileName = $"{horseId}{extension}";
        string fullPath = Path.Combine(uploadsFolder, fileName);

        await using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
        {
            await file.OpenReadStream(maxFileSize).CopyToAsync(fs);
        }

        return new HorseFoto
        {
            HorseId = horseId,
            FotoPath = $"/Images/horsephotos/{fileName}"
        };
    }

    public async Task<List<Horse>> GetHorsesAsync()
    {
        try
        {
            return await _horseRepo.GetAllHorses();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task<List<Horse>> GetHorsesBySchool(int schoolId)
    {
        try
        {
            return await _horseRepo.GetHorsesBySchool(schoolId);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task<List<Horse>> GetHorsesByUser(UpdateUserDto user)
    {
        try
        {
            var horseList = new List<Horse>();
            horseList.AddRange(await _horseRepo.GetHorsesByUser(user.Id));

            var schools = await _schoolService.GetUserSchoolsAsync(user.Id);
            foreach (var school in schools)
            {
                horseList.AddRange(await _horseRepo.GetHorsesBySchool(school.SchoolId));
            }
            return horseList;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e.InnerException);
        }
    }



    public async Task<bool> IsAvailable(Horse horse, DateTime date, bool Aula = false)
    {
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
            return false;

        // If there's exactly one booking of type "aula" and one of type "passeio" → cannot book.
        if (aulaCount == 1 && passeioCount == 1)
            return false;

        if (totalCount == 3 && Aula)
            return false;

        // If there are 4 bookings (total) → cannot book.
        if (totalCount == 4)
            return false;

        // If none of the above conditions are met, it's valid to book.
        return true;
    }

    public async Task<Horse> RemoveHorse(Horse horse)
    {
        try
        {
            return await _horseRepo.DeleteHorseById(horse.HorseId);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task<Horse> UpdateHorse(Horse horse, IBrowserFile? file)
    {
        try
        {
            var updatedHorse = await _horseRepo.EditHorse(horse);

            if (file == null)
                return updatedHorse;

            var photo = await SavePhoto(file, horse.HorseId);

            // só atualizas isto se quiseres suportar mudança de extensão
            if (updatedHorse.HorseFoto == null)
                    updatedHorse.HorseFoto = photo;
            else
                updatedHorse.HorseFoto.FotoPath = photo.FotoPath;

            return await _horseRepo.EditHorse(updatedHorse);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<List<Horse>> AvailableHorsesByDate(DateTime date, int schoolId)
    {
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
            return availableHorses;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task<List<Horse>> AvailableHorsesByDateAula(DateTime date, int schoolId)
    {
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
            return availableHorses;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e.InnerException);
        }
    }
}
