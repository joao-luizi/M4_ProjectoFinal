
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Logging;
using RepositoryLibrary.Features.Horses.DTOs;
using RepositoryLibrary.Features.Horses.Entities;
using RepositoryLibrary.Features.Horses.Interfaces;
using RepositoryLibrary.Features.Images.Interfaces;
using RepositoryLibrary.Features.Users.Entities;



namespace RepositoryLibrary.Features.Horses.Services;

public class HorseService : IHorseService
{
    private readonly IHorseRepository _horseRepo;
    private readonly IImageService _imageService;
    private readonly IHorsePhotoRepository _horsePhotoRepo;
    private readonly ILogger<HorseService> _logger;

    public HorseService(
        IHorseRepository horseRepo, 
        IImageService imageService,
        IHorsePhotoRepository horsePhotoRepo,
        ILogger<HorseService> logger)
    {
        _horseRepo = horseRepo;
        _imageService = imageService; 
        _horsePhotoRepo = horsePhotoRepo;
        _logger = logger;
    }



    //V2 Implelmented
    public async Task<HorseOverviewDto> GetHorseOverviewAsync()
    {
        try
        {
            var horses = await _horseRepo.GetAllAsync();

            var horseList = horses.ToList();

            var total = horseList.Count;

            var mapped = horseList.Select(h =>
            {
                var ownership = ResolveOwnership(h);

                return new HorseListItemDto
                {
                    HorseId = h.HorseId,
                    Name = h.Name,
                    Breed = h.Breed,
                    DateOfBirth = h.DateOfBirth,
                    SchoolId = h.School.SchoolId,
                    SchoolName = h.School.SchoolName,
                    Ownership = ownership
                };
            }).ToList();

            var schoolCount = mapped.Count(h => h.Ownership != "Owner");
            var privateCount = total - schoolCount;

            return new HorseOverviewDto
            {
                TotalHorses = total,
                SchoolHorses = schoolCount,
                PrivateHorses = privateCount,
                Horses = mapped
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching horse overview");
            throw;
        }
    }
    //V2 Implelmented
    private string ResolveOwnership(Horse horse)
    {
        if (horse.UserHorses == null || horse.UserHorses.Count == 0)
            return "School";

        var primary = horse.UserHorses.FirstOrDefault();

        if (primary == null)
            return "School";

        return primary.Relationship ?? "Private";
    }

    //V2 Implelmented
    public async Task<Horse> GetHorseByIdAsync(int id)
    {
        try
        {
            var horse = await _horseRepo.GetByIdAsync(id);

            if (horse == null)
                throw new KeyNotFoundException($"Horse {id} not found");

            return horse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching horse {HorseId}", id);
            throw;
        }
    }

    //V2 Implelmented
    private async Task UpdateHorseAsync(HorseEditDto horseDto)
    {
        try
        {
           
            var existing = await _horseRepo.GetByIdAsync(horseDto.HorseId);

            if (existing == null)
                throw new KeyNotFoundException($"Horse {horseDto.HorseId} not found");

            existing.Name = horseDto.Name;
            existing.Breed = horseDto.Breed;
            existing.DateOfBirth = horseDto.DateOfBirth;

            await _horseRepo.SaveChangesAsync();

            var existingOwner = await _horseRepo.GetUserHorseByHorseId(horseDto.HorseId);

            if (string.IsNullOrWhiteSpace(horseDto.OwnerUserId))
            {
                if (existingOwner != null)
                {
                    await _horseRepo.Delete(existingOwner);
                }
            }
            else
            {
                if (existingOwner == null)
                {
                    await _horseRepo.AddAsync(new UserHorse
                    {
                        HorseId = horseDto.HorseId,
                        UserId = horseDto.OwnerUserId,
                        Relationship = "Owner"
                    });
                }
                else if (existingOwner.UserId != horseDto.OwnerUserId)
                {
                    existingOwner.UserId = horseDto.OwnerUserId;

                    await _horseRepo.SaveChangesAsync();
                }
            }

            // 2. Se não há imagem nova, termina aqui
            if (horseDto.NewPhoto is null)
                return;

            // 3. Obter foto atual (se existir)
            var existingPhoto = await _horsePhotoRepo.GetByHorseIdAsync(horseDto.HorseId);

            string newPath = await _imageService.ReplaceImageAsync(
                horseDto.NewPhoto,
                folder: "horses",
                fileName: horseDto.HorseId.ToString(),
                existingImagePath: existingPhoto?.FotoPath);

            // 4. Persistir relação UserPhoto
            if (existingPhoto is null)
            {
                await _horsePhotoRepo.AddAsync(new HorseFoto
                {
                    HorseId = horseDto.HorseId,
                    FotoPath = newPath
                });
            }
            else
            {
                existingPhoto.FotoPath = newPath;
                await _horsePhotoRepo.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating horse {HorseId}", horseDto.HorseId);
            throw;
        }
    }
    //V2 Implelmented
    private async Task CreateHorseAsync(HorseEditDto horseDto)
    {
        try
        {
            var horse = new Horse
            {
                Name = horseDto.Name,
                Breed = horseDto.Breed,
                DateOfBirth = horseDto.DateOfBirth,
                SchoolId = horseDto.SchoolId
            };

            await _horseRepo.AddAsync(horse);

            if (!string.IsNullOrWhiteSpace(horseDto.OwnerUserId))
            {
                var userHorse = new UserHorse
                {
                    UserId = horseDto.OwnerUserId,
                    HorseId = horse.HorseId,
                    Relationship = "Owner"
                };

                await _horseRepo.AddAsync(userHorse);
            }

            if (horseDto.NewPhoto is null)
                return;

            string newPath = await _imageService.ReplaceImageAsync(
                horseDto.NewPhoto,
                folder: "horses",
                fileName: horse.HorseId.ToString(),
                existingImagePath: null);

            await _horsePhotoRepo.AddAsync(new HorseFoto
            {
                HorseId = horse.HorseId,
                FotoPath = newPath
            });

            await _horsePhotoRepo.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error creating horse");

            throw;
        }
    }
    //V2 Implemented
    public async Task<HorseEditDto> GetHorseEditDtoAsync(int horseId)
    {
        try
        {
            var horse = await _horseRepo.GetByIdAsync(horseId);

            if (horse == null)
                throw new KeyNotFoundException($"Horse {horseId} not found.");

            var userHorse = await _horseRepo.GetUserHorseByHorseId(horseId);

            return new HorseEditDto
        {
            HorseId = horse.HorseId,
            Name = horse.Name,
            Breed = horse.Breed,
            DateOfBirth = horse.DateOfBirth,
            PhotoPath = horse.HorseFoto?.FotoPath,
            SchoolId = horse.SchoolId,
            OwnerUserId = userHorse?.UserId, 
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading horse edit dto for horse {HorseId}", horseId);
            throw;
        }
    }

    //V2 Implelmented
    public async Task SaveHorseAsync(HorseEditDto dto)
    {
        if (dto.HorseId == 0)
        {
            // create
            await CreateHorseAsync(dto);
        }
        else
        {
            // update
            await UpdateHorseAsync(dto);
        }
    }


}