using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Horses.Entities;
using RepositoryLibrary.Features.Horses.Interfaces;
using RepositoryLibrary.Features.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Horses.Repositories
{
    public class HorsePhotoRepository : IHorsePhotoRepository
    {
        private readonly RideReadyDbContext _context;
        private readonly ILogger<HorsePhotoRepository> _logger;

        public HorsePhotoRepository(RideReadyDbContext context, ILogger<HorsePhotoRepository> logger)
        {
            _context = context;
            _logger = logger ;
        }

        //V2 Implemented
        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
       
        //V2 Implemented
        public async Task AddAsync(HorseFoto entity)
        {
            await _context.Set<HorseFoto>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        //V2 Implemented
        public async Task DeleteAsync(HorseFoto entity)
        {
            _context.Set<HorseFoto>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        //V2 Implemented
        public async Task<HorseFoto?> GetByHorseIdAsync(int horseId)
        {
            return await _context.HorseFotos
               .Where(x => x.HorseId == horseId)
               .FirstOrDefaultAsync();
        }
    }
}
