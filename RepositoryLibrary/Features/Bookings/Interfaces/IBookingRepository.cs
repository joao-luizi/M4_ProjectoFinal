using RepositoryLibrary.Features.Bookings.Entities;

namespace RepositoryLibrary.Features.Bookings.Interfaces
{
    public interface IBookingRepository
    {
        //GetBookingsForStudentsAsync
        //GetBookingsByUserIdAsync
        //V2 Implemented
        Task<List<Booking>> GetBookingsForStudentsAsync(List<string> studentIds);
        //public Task<Booking?> GetLastAttendedBookingAsync(string userId, DateTime now);
        //V2 Implemented
        Task<List<Booking>> GetBookingsByUserIdAsync(string userId);
        //V2 Implemented
        Task Delete(Booking booking);
        //V2 Implemented
        Task SaveChanges();
        //V2 Implemented
        Task<Booking?> GetByLessonandUserIdsAsync(int lessonId, string userId);

        //V2 Implemented
        Task addAsync(Booking booking);

        //V2 Implemented
        Task<int> GetWeeklyBooking(string userId, int lessonTypeId);

        //public Task<Booking> GetBookingAsync(string userId, int lessonId);
        //public Task<List<Booking>> GetAllBookedLessonsAsync();
        //public Task<List<Booking>> GetBookingByDateAsync(DateTime date);
        //public Task<Booking> CreateBookingAsync(Booking booking);
        //public Task<Booking> CancelBookingAsync(Booking booking); //delete
        //public Task<Booking> ChangeBookingPresenceAsync(Booking booking);
        //public Task<List<Booking>> GetBookingsByLessonId(int lessonId);

        //public Task<bool> IsLessonBookedAsync(int lessonId, string studentId);


    }
}
