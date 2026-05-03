using System;
using RepositoryLibrary.Models;

namespace RepositoryLibrary.IServices;

public interface IBookingService
{

    public Task<List<Booking>> GetAllBookingsAsync();
    public Task<List<Booking>> GetBookingsByUserIdAsync(string userId);
    public Task<List<Booking>> GetBookingByDateAsync(DateTime date);
    public Task<List<Booking>> GetBookingByLesson(int lessonId);

    public Task<Booking> CreateBookingAsync(string userId, int lessonId);
    public Task<Booking> CancelBookingAsync(string userId, int lessonId);
    public Task<Booking> ChangeBookingPresenceAsync(string userId, int lessonId);
    Task<(bool weekly, int? amount)> CanBook(string userId, int lessonId);

}
