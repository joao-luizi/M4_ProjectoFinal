using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.DTOs;
using System;

namespace RepositoryLibrary.IServices;

public interface IHorseService
{
    Task<List<Horse>> GetHorsesAsync();
    Task<List<Horse>> GetHorsesBySchool(int schoolId);
    Task<bool> IsAvailable(Horse horse, DateTime date, bool Aula);
    Task<List<Horse>> GetHorsesByUser(UpdateUserDto user);
    public Task<Horse> AddHorse(Horse horse, IBrowserFile? file);
    Task<Horse> RemoveHorse(Horse horse);
    Task<Horse> UpdateHorse(Horse horse, IBrowserFile? file);
    Task<List<Horse>> AvailableHorsesByDate(DateTime date, int schoolId);

    public Task<Horse> GetHorseByIdAsync(int horseId);

}

