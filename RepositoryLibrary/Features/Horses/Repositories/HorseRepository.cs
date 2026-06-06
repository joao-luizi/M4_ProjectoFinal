using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Horses.Entities;
using RepositoryLibrary.Features.Horses.Interfaces;
using RepositoryLibrary.Features.Users.Entities;

namespace RepositoryLibrary.Features.Horses.Repositories
{
    public class HorseRepository : IHorseRepository
    {
        private readonly RideReadyDbContext _emContext;
        private readonly ILogger<HorseRepository> _logger;

        public HorseRepository(RideReadyDbContext context, ILogger<HorseRepository>? logger = null)
        {
            _emContext = context;
            _logger = logger ?? NullLogger<HorseRepository>.Instance;
        }



        //V2 Implelmented
        public async Task SaveChangesAsync()
        {
            await _emContext.SaveChangesAsync();
        }
        //V2 Implelmented
        public async Task<Horse?> GetByIdAsync(int horseId)
        {
            return await _emContext.Horses
                .Include(x => x.HorseFoto)
                .Include(x => x.UserHorses)
                .Include(x => x.School)
                .FirstOrDefaultAsync(x => x.HorseId == horseId);
        }
        //V2 Implelmented
        public async Task<List<Horse>> GetAllAsync()
        {
            return await _emContext.Horses
                .Include(x => x.HorseFoto)
                .Include(x => x.UserHorses)
                .Include(x => x.School)
                .ToListAsync();
        }

        //V2 Implemented
        public async Task<List<Horse>> GetByIdsAsync(List<int> ids)
        {
            _logger.LogInformation("BD: a obter cavalos com o id em lista.");
            if (ids == null || ids.Count == 0)
            {
                _logger.LogWarning("Nenhum cavalo encontrado contido na lista de ids.");
                return new List<Horse>();
            }

            return await _emContext.Horses
                .Where(u => ids.Contains(u.HorseId))
                .ToListAsync();
        }
        //V2 Implemented
        public async Task<int> CountAsync()
        {
            return await _emContext.Horses.CountAsync();
        }
        //V2 Implemented
        public async Task AddAsync(Horse horse)
        {
            await _emContext.Horses.AddAsync(horse);
        }
    }
}