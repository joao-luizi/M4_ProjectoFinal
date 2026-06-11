using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Bookings.Entities;
using RepositoryLibrary.Features.Bookings.Interfaces;
using RepositoryLibrary.Features.Lessons.Entities;

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

        //V2 Implemented
        public async Task Delete(Booking booking)
        {
            _emContext.Bookings.Remove(booking);
            await _emContext.SaveChangesAsync();
        }
        //V2 Implemented
        public async Task SaveChanges()
        {
            await _emContext.SaveChangesAsync();
        }

        public async  Task<Booking?> GetByLessonandUserIdsAsync(int lessonId, string userId)
        {
            return await _emContext.Bookings.Where(x => x.UserId == userId && x.LessonId == lessonId).FirstOrDefaultAsync();
        }

        public async Task addAsync(Booking booking)
        {
            await _emContext.AddAsync(booking);

        }

        public async Task<int> GetWeeklyBooking(string userId, int lessonTypeId)
        {
            var now = DateTime.UtcNow;

            // calcular início da semana (segunda-feira)
            var diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
            var startOfWeek = now.Date.AddDays(-diff);

            // fim da semana (domingo 23:59:59)
            var endOfWeek = startOfWeek.AddDays(7);

            return await _emContext.Bookings
                .Where(x =>
                    x.UserId == userId &&
                    x.Lesson.LessonTypeId == lessonTypeId &&
                    x.Lesson.BeginOfLesson >= startOfWeek &&
                    x.Lesson.BeginOfLesson < endOfWeek)
                .CountAsync();
        }







    }
}
