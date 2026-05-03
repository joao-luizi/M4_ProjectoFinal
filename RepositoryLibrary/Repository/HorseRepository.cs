using RepositoryLibrary.IRepository;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace RepositoryLibrary.Repository
{
    public class HorseRepository : IHorseRepository
    {
        private readonly EM_DbContext _emContext;
        public HorseRepository(EM_DbContext context)
        {
            _emContext = context;
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
                var horse = await _emContext.Horses.FirstOrDefaultAsync(horse => horse.HorseId == horseId) ?? throw new Exception("Horse doesn't exist, can't delete.");
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
                    .SingleOrDefaultAsync(h => h.HorseId == horse.HorseId);

                if (existing == null)
                    throw new InvalidOperationException($"Horse {horse.HorseId} not found.");

                existing.Name = horse.Name;
                existing.Breed = horse.Breed;
                existing.Age = horse.Age;

                var school = await _emContext.Schools.FindAsync(horse.School.SchoolId)
                            ?? throw new InvalidOperationException($"School {horse.School.SchoolId} not found.");
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
                var horses = await _emContext.Horses.ToListAsync();
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
    }
}
