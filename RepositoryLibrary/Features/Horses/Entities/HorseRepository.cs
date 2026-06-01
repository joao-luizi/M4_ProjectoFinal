using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Horses.Interfaces;

namespace RepositoryLibrary.Features.Horses.Entities
{
    public class HorseRepository : IHorseRepository
    {
        private readonly RideReadyDbContext _emContext;
        private readonly ILogger<HorseRepository> _logger;

        public HorseRepository(RideReadyDbContext context, ILogger<HorseRepository> logger)
        {
            _emContext = context;
            _logger = logger;
        }

        public async Task<Horse?> GetHorseByIdAsync(int horseId)
        {
            _logger.LogInformation("BD: a consultar cavalo {HorseId}.", horseId);

            var horse = await _emContext.Horses
                .Include(h => h.HorseFoto)
                .Include(h => h.UserHorses)
                .Include(h => h.School)
                .FirstOrDefaultAsync(h => h.HorseId == horseId);

            if (horse == null)
            {
                _logger.LogWarning("Cavalo {HorseId} não encontrado.", horseId);
            }
            else
            {
                _logger.LogInformation("BD: cavalo {HorseId} obtido.", horseId);
            }

            return horse;
        }

        public async Task<Horse> CreateHorse(Horse horse)
        {
            _logger.LogInformation("BD: a criar cavalo '{HorseName}' na escola {SchoolId}.", horse.Name, horse.School.SchoolId);

            var existingSchool = await _emContext.Schools
                .FindAsync(horse.School.SchoolId);

            if (existingSchool == null)
            {
                _logger.LogWarning("Não foi possível criar cavalo: escola {SchoolId} não existe.", horse.School.SchoolId);
                throw new InvalidOperationException(
                    $"Cannot create horse: School with ID {horse.School.SchoolId} not found.");
            }

            _emContext.Entry(existingSchool)
                      .State = EntityState.Unchanged;
            horse.School = existingSchool;

            await _emContext.Horses.AddAsync(horse);
            await _emContext.SaveChangesAsync();

            _logger.LogInformation("BD: cavalo {HorseId} ('{HorseName}') criado na escola {SchoolId}.", horse.HorseId, horse.Name, horse.School.SchoolId);
            return horse;
        }


        public async Task<Horse> DeleteHorseById(int horseId)
        {
            _logger.LogInformation("BD: a eliminar cavalo {HorseId}.", horseId);
            try
            {
                var horse = await _emContext.Horses
                 .Include(h => h.HorseFoto)
                 .Include(h => h.UserHorses)
                 .FirstOrDefaultAsync(h => h.HorseId == horseId)
                 ?? throw new Exception("Horse doesn't exist.");

                if (horse.HorseFoto != null && horse.HorseFoto.FotoPath != null)
                {
                    var path = Path.Combine("wwwroot", horse.HorseFoto.FotoPath.TrimStart('/'));

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                        _logger.LogInformation("BD: foto física do cavalo {HorseId} eliminada de '{Path}'.", horseId, path);
                    }
                }

                _emContext.Horses.Remove(horse);
                await _emContext.SaveChangesAsync();

                _logger.LogInformation("BD: cavalo {HorseId} eliminado.", horseId);
                return horse;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao eliminar cavalo {HorseId}.", horseId);
                throw new Exception(e.Message, e.InnerException);
            }
        }


        public async Task<Horse> EditHorse(Horse horse)
        {
            _logger.LogInformation("BD: a editar cavalo {HorseId}.", horse.HorseId);
            try
            {
                var existing = await _emContext.Horses
                    .Include(h => h.School)
                    .Include(h => h.HorseFoto)
                    .SingleOrDefaultAsync(h => h.HorseId == horse.HorseId);

                if (existing == null)
                {
                    _logger.LogWarning("Cavalo {HorseId} não encontrado para edição.", horse.HorseId);
                    throw new InvalidOperationException(
                        $"Horse {horse.HorseId} not found.");
                }

                existing.Name = horse.Name;
                existing.Breed = horse.Breed;
                existing.DateOfBirth = horse.DateOfBirth;

                // FOTO
                if (horse.HorseFoto?.FotoPath != null)
                {
                    if (existing.HorseFoto == null)
                    {
                        existing.HorseFoto = new HorseFoto
                        {
                            HorseId = existing.HorseId,
                            FotoPath = horse.HorseFoto.FotoPath
                        };
                    }
                    else
                    {
                        existing.HorseFoto.FotoPath = horse.HorseFoto.FotoPath;
                    }
                }

                var school = await _emContext.Schools.FindAsync(horse.School.SchoolId);
                if (school == null)
                {
                    _logger.LogWarning("Não foi possível editar cavalo {HorseId}: escola {SchoolId} não existe.", horse.HorseId, horse.School.SchoolId);
                    throw new InvalidOperationException(
                        $"School {horse.School.SchoolId} not found.");
                }

                existing.School = school;

                await _emContext.SaveChangesAsync();

                _logger.LogInformation("BD: cavalo {HorseId} editado.", horse.HorseId);
                return existing;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao editar cavalo {HorseId}.", horse.HorseId);
                throw new Exception(e.Message, e);
            }
        }



        public async Task<List<Horse>> GetAllHorses()
        {
            _logger.LogInformation("BD: a consultar todos os cavalos.");
            try
            {
                var horses = await _emContext.Horses.Include(h => h.UserHorses).ToListAsync();
                _logger.LogInformation("BD: obtidos {Count} cavalos.", horses.Count);
                return horses;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao consultar a lista de cavalos.");
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<List<Horse>> GetHorsesBySchool(int schoolId)
        {
            _logger.LogInformation("BD: a consultar cavalos sem dono da escola {SchoolId}.", schoolId);
            try
            {
                var horses = await _emContext.Horses.Where(h => h.School.SchoolId == schoolId)
                .Include(h => h.UserHorses)
                .Where(h => !h.UserHorses.Any()).ToListAsync();

                _logger.LogInformation("BD: obtidos {Count} cavalos sem dono na escola {SchoolId}.", horses.Count, schoolId);
                return horses;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao consultar cavalos da escola {SchoolId}.", schoolId);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<List<Horse>> GetHorsesByUser(string userId)
        {
            _logger.LogInformation("BD: a consultar cavalos do utilizador {UserId}.", userId);
            try
            {
                var horses = await _emContext.Horses.Include(h => h.UserHorses).Where(h => h.UserHorses.Any(uh => uh.UserId == userId)).ToListAsync();

                _logger.LogInformation("BD: obtidos {Count} cavalos para o utilizador {UserId}.", horses.Count, userId);
                return horses;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao consultar cavalos do utilizador {UserId}.", userId);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<int> DeleteByUserIdAsync(string userId)
        {
            _logger.LogInformation("BD: a eliminar cavalos do utilizador {UserId}.", userId);

            var userHorses = await _emContext.UserHorses
                .Include(uh => uh.Horse)
                .Where(uh => uh.UserId == userId)
                .ToListAsync();

            if (!userHorses.Any())
            {
                _logger.LogWarning("Utilizador {UserId} não tem cavalos associados; nada a eliminar.", userId);
                return 0;
            }

            var horses = userHorses
                .Select(uh => uh.Horse)
                .Distinct()
                .ToList();

            _emContext.UserHorses.RemoveRange(userHorses);
            _emContext.Horses.RemoveRange(horses);

            var rowsAffected = await _emContext.SaveChangesAsync();
            _logger.LogInformation("BD: eliminados {HorseCount} cavalos do utilizador {UserId} ({Rows} linhas afetadas).", horses.Count, userId, rowsAffected);
            return rowsAffected;
        }
    }
}