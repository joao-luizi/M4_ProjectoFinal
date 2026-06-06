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
        public Task<List<Booking>> GetBookingsByUserIdAsync(string userId);
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
