using RepositoryLibrary.IRepository;
using RepositoryLibrary.IServices;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using RepositoryLibrary.Models.DTOs;
using RepositoryLibrary.Repository;

namespace RepositoryLibrary.Services;

public class HorseService : IHorseService
{
    private readonly IHorseRepository _horseRepo;
    private readonly ISchoolService _schoolService;
    private readonly ILessonService _lessonService;
    public HorseService(EM_DbContext dbContext, ISchoolService schoolService, ILessonService lessonService)
    {
        _horseRepo = new HorseRepository(dbContext);
        _schoolService = schoolService;
        _lessonService = lessonService;
    }
    public async Task<Horse> AddHorse(Horse horse)
    {

        try
        {
            await _horseRepo.CreateHorse(horse);
            return horse;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e.InnerException);
        }
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
            // foreach (var school in user.SchoolUsers)
            // {
            //     horseList.AddRange(await _horseRepo.GetHorsesBySchool(school.SchoolId));
            // }
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

    public async Task<Horse> UpdateHorse(Horse horse)
    {
        try
        {
            return await _horseRepo.EditHorse(horse);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e.InnerException);
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
