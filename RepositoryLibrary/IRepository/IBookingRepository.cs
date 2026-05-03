using RepositoryLibrary.Models;

namespace RepositoryLibrary.IRepository
{
    public interface IBookingRepository
    {
        public Task<List<Booking>> GetBookingsByUserIdAsync(string userId);
        public Task<Booking> GetBookingAsync(string userId, int lessonId);
        public Task<List<Booking>> GetAllBookedLessonsAsync();
        public Task<List<Booking>> GetBookingByDateAsync(DateTime date);
        public Task<Booking> CreateBookingAsync(Booking booking);
        public Task<Booking> CancelBookingAsync(Booking booking); //delete
        public Task<Booking> ChangeBookingPresenceAsync(Booking booking);
        public Task<List<Booking>> GetBookingsByLessonId(int lessonId);

    	public Task<bool> IsLessonBookedAsync(int lessonId, string studentId);
    }
}
