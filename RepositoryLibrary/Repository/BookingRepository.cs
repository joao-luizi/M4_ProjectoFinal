using RepositoryLibrary.IRepository;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace RepositoryLibrary.Repository
{
    public class BookingRepository : IBookingRepository
    {
        public readonly EM_DbContext _emContext;

        public BookingRepository(EM_DbContext context)
        {
            _emContext = context;
        }

        public async Task<Booking> GetBookingAsync(string userId, int lessonId)
        {
            try
            {
                var booking = await _emContext.Bookings.FirstOrDefaultAsync(b => b.UserId == userId && b.LessonId == lessonId) ?? throw new Exception("Booking not found.");
                return booking;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<List<Booking>> GetBookingsByUserIdAsync(string userId)
        {
            /* TO-DO:
             -> Verify if User exists
             -> Verify if user is a student (if not, can't book)
             -> Verify if User payment is due(???)
             */
            try
            {
                return await _emContext.Bookings.Where(b => b.UserId == userId).ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<List<Booking>> GetAllBookedLessonsAsync()
        {
            /* TO-DO:
             -> Verify if lessons are booked
             -> Verify what's the date to verify(?)
             -> Verify possible filters (date, teacher, horse) - possibly on front-end
             */
            try
            {
                return await _emContext.Bookings.Include(b => b.Lesson).Where(b => DateTime.Compare(DateTime.Now, b.Lesson.EndOfLesson) <= 0).ToListAsync(); //gets All Bookings where the 
            }                                                                                                                                              //Lesson hasn't ended yet
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            /* TO-DO:
             -> Verify if user payment is due 
             -> Verify if user is active, using UserServices
             -> Verify if there are still spots left
             */
            try
            {
                // var user = await _usersContext.Users.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new Exception("User not found, cannot create booking");
                // if (!user.IsActive) throw new Exception("User is not active cannot create booking");
                // int studentsInLesson = _emContext.Bookings.Where(b => b.LessonId == lesson.LessonId).Count();
                // if (!await _emContext.Lessons.AnyAsync(l => l.MaxSpots > studentsInLesson && l.LessonId == lesson.LessonId))
                // {
                //     throw new Exception("Lesson not available. Cannot create booking.");
                // }
                // var newBooking = new Booking { UserId = user.Id, Lesson = lesson };
                //everything above should go to their respective repositories
                await _emContext.Bookings.AddAsync(booking);
                await _emContext.SaveChangesAsync();
                var createdBooking = await _emContext.Bookings.FirstOrDefaultAsync(b => b.LessonId == booking.LessonId && b.UserId == booking.UserId) ?? throw new Exception("Error finding created booking");
                return createdBooking;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
        public async Task<Booking> CancelBookingAsync(Booking booking)
        {
            /* TO-DO:
             -> Verify if combination exists
             */
            try
            {
                if (_emContext.Bookings.Any(b => b.UserId == booking.UserId && b.LessonId == booking.LessonId))
                {
                    _emContext.Bookings.Remove(booking);
                    await _emContext.SaveChangesAsync();
                    return booking;
                }
                throw new Exception("Booking doesn't exist. Couldn't cancel booking.");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<Booking> ChangeBookingPresenceAsync(Booking booking)
        {
            try
            {
                _emContext.Bookings.Update(booking);
                await _emContext.SaveChangesAsync();
                return booking;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<List<Booking>> GetBookingByDateAsync(DateTime date)
        {
            try
            {
                var startOfDay = date.Date;
                var endOfDay = startOfDay.AddDays(1);
                var bookingsForTheDay = await _emContext.Bookings
                    .Where(b => b.Lesson.BeginOfLesson >= startOfDay && b.Lesson.BeginOfLesson < endOfDay)
                    .ToListAsync();

                return bookingsForTheDay;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<List<Booking>> GetBookingsByLessonId(int lessonId)
        {
            try
            {
                return await _emContext.Bookings.Where(b => b.LessonId == lessonId).ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
        
        public async Task<bool> IsLessonBookedAsync(int lessonId, string userId)
        {
            return await _emContext.Bookings.AnyAsync(b => b.LessonId == lessonId && b.UserId == userId);
        }
    }
}
