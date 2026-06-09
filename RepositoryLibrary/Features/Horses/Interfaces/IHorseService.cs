using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using RepositoryLibrary.Features.Horses.DTOs;
using RepositoryLibrary.Features.Horses.Entities;
using RepositoryLibrary.Features.Users.DTOs;
using System;

namespace RepositoryLibrary.Features.Horses.Interfaces;

public interface IHorseService
{
    //V2 Implelmented
    Task<HorseOverviewDto> GetHorseOverviewAsync();
    //V2 Implelmented
    Task<Horse> GetHorseByIdAsync(int id);

  
    //V2 Implelmented
    Task<HorseEditDto> GetHorseEditDtoAsync(int horseId);

    //V2 Implelmented
    Task SaveHorseAsync(HorseEditDto dto);

    //V2 Implelmented
    Task<List<SelectHorseDto>> GetSelectHorseAsync(int schoolId);

}

