using System;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.DTOs;

namespace RepositoryLibrary.IServices;

public interface IHorseService
{
    Task<List<Horse>> GetHorsesAsync();
    Task<List<Horse>> GetHorsesBySchool(int schoolId);
    Task<bool> IsAvailable(Horse horse, DateTime date, bool Aula);
    Task<List<Horse>> GetHorsesByUser(UpdateUserDto user);
    Task<Horse> AddHorse(Horse horse);
    Task<Horse> RemoveHorse(Horse horse);
    Task<Horse> UpdateHorse(Horse horse);
    Task<List<Horse>> AvailableHorsesByDate(DateTime date, int schoolId);
}

