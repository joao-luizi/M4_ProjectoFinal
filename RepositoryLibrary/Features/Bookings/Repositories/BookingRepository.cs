using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Bookings.Entities;
using RepositoryLibrary.Features.Bookings.Interfaces;

namespace RepositoryLibrary.Features.Bookings.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly RideReadyDbContext _emContext;
        private readonly ILogger<BookingRepository> _logger;

        public BookingRepository(RideReadyDbContext context, ILogger<BookingRepository> logger)
        {
            _emContext = context;
            _logger = logger;
        }

        //V2 Implemented
        public async Task<List<Booking>> GetBookingsForStudentsAsync(List<string> studentIds)
        {
            _logger.LogInformation(
                "BD: a obter bookings para {Count} alunos.",
                studentIds.Count);

            return await _emContext.Bookings
                .Where(b => studentIds.Contains(b.UserId))
                .Include(b => b.Lesson)
                .ToListAsync();
        }
       
       
        //V2 Implemented
        public async Task<List<Booking>> GetBookingsByUserIdAsync(string userId)
        {
            return await _emContext.Bookings
                .Where(b => b.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

       

       
  


    }
}
