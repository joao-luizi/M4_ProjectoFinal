
using Microsoft.EntityFrameworkCore;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Horses.Interfaces;

namespace RepositoryLibrary.Features.Horses.Entities
{
    public class HorseRepository : IHorseRepository
    {
        private readonly EM_DbContext _emContext;
        public HorseRepository(EM_DbContext context)
        {
            _emContext = context;
        }

        public async Task<Horse?> GetHorseByIdAsync(int horseId)
        {
           
            return await _emContext.Horses
                .Include(h => h.HorseFoto)
                .Include(h => h.UserHorses)
                .Include(h => h.School)
                .FirstOrDefaultAsync(h => h.HorseId == horseId);
        }

        public async Task<Horse> CreateHorse(Horse horse)
        {
            var existingSchool = await _emContext.Schools
                .FindAsync(horse.School.SchoolId);

            if (existingSchool == null)
            {
                throw new InvalidOperationException(
                    $"Cannot create horse: School with ID {horse.School.SchoolId} not found.");
            }

            _emContext.Entry(existingSchool)
                      .State = EntityState.Unchanged;
            horse.School = existingSchool;
            
            await _emContext.Horses.AddAsync(horse);
            await _emContext.SaveChangesAsync();
            return horse;
        }


        public async Task<Horse> DeleteHorseById(int horseId)
        {
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
                        File.Delete(path);
                }

                _emContext.Horses.Remove(horse);
                await _emContext.SaveChangesAsync();
                return horse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }


        public async Task<Horse> EditHorse(Horse horse)
        {
            try
            {
                var existing = await _emContext.Horses
                    .Include(h => h.School)
                    .Include(h => h.HorseFoto)
                    .SingleOrDefaultAsync(h => h.HorseId == horse.HorseId);

                if (existing == null)
                    throw new InvalidOperationException(
                        $"Horse {horse.HorseId} not found.");

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

                var school = await _emContext.Schools.FindAsync(horse.School.SchoolId)
                             ?? throw new InvalidOperationException(
                                 $"School {horse.School.SchoolId} not found.");

                existing.School = school;

                await _emContext.SaveChangesAsync();

                return existing;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }



        public async Task<List<Horse>> GetAllHorses()
        {
            try
            {
                var horses = await _emContext.Horses.Include(h => h.UserHorses).ToListAsync();
                return horses;
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
                var horses = await _emContext.Horses.Where(h => h.School.SchoolId == schoolId)
                .Include(h => h.UserHorses)
                .Where(h => !h.UserHorses.Any()).ToListAsync();
                return horses;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<List<Horse>> GetHorsesByUser(string userId)
        {
            try
            {
                var horses = await _emContext.Horses.Include(h => h.UserHorses).Where(h => h.UserHorses.Any(uh => uh.UserId == userId)).ToListAsync();
                return horses;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<int> DeleteByUserIdAsync(string userId)
        {
            var userHorses = await _emContext.UserHorses
                .Include(uh => uh.Horse)
                .Where(uh => uh.UserId == userId)
                .ToListAsync();

            if (!userHorses.Any())
                return 0;

            var horses = userHorses
                .Select(uh => uh.Horse)
                .Distinct()
                .ToList();

            _emContext.UserHorses.RemoveRange(userHorses);
            _emContext.Horses.RemoveRange(horses);

            return await _emContext.SaveChangesAsync();
        }
    }
}
