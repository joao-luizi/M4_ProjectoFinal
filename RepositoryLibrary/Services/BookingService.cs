using RepositoryLibrary.IRepository;
using RepositoryLibrary.IServices;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using RepositoryLibrary.Repository;
using Microsoft.AspNetCore.Identity;

namespace RepositoryLibrary.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IUserService _userService;
  
    private readonly ILessonService _lessonService;
    private readonly ISchoolService _schoolService;
    public BookingService(EM_DbContext _context, IUserService userService, ILessonService lessonService)
    {
        _bookingRepo = new BookingRepository(_context);
      
        _userService = userService;
        _lessonService = lessonService;
    }
    public async Task<Booking> CancelBookingAsync(string userId, int lessonId)
    {
        try
        {
            await CanProceed(userId, lessonId);
            var booking = await _bookingRepo.GetBookingAsync(userId, lessonId) ?? throw new Exception("Booking doesn't exist.");
            var canceled = await _bookingRepo.CancelBookingAsync(booking);
            return canceled;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e.InnerException);
        }

    }

    public async Task<Booking> ChangeBookingPresenceAsync(string userId, int lessonId)
    {
        try
        {
            await CanProceed(userId, lessonId);
            var booking = await _bookingRepo.GetBookingAsync(userId, lessonId);
            booking.WasPresent = !booking.WasPresent;
            booking = await _bookingRepo.ChangeBookingPresenceAsync(booking);
            return booking;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e.InnerException);
        }

    }

    public async Task<Booking> CreateBookingAsync(string userId, int lessonId)
    {
        try
        {
            await CanProceed(userId, lessonId);
            //checks if it is weekly package and if can still book for the week if
            //if not weekly check if it has amount
            // var package = await CanBook(userId, lessonId);
            var booking = new Booking { UserId = userId, LessonId = lessonId };
            booking = await _bookingRepo.CreateBookingAsync(booking);
            //if it's not a weekly package it uses in the lessons amount
            // if (!package.weekly) await _paymentRepo.useClass(userId);
            return booking;

        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task<List<Booking>> GetAllBookingsAsync()
    {
        try
        {
            return await _bookingRepo.GetAllBookedLessonsAsync();
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
            return await _bookingRepo.GetBookingByDateAsync(date);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task<List<Booking>> GetBookingByLesson(int lessonId)
    {
        try
        {
            return await _bookingRepo.GetBookingsByLessonId(lessonId);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e.InnerException);
        }
    }

    public async Task<List<Booking>> GetBookingsByUserIdAsync(string userId)
    {
        try
        {
            //check if user exists
            _ = await _userService.GetUserById(userId) ?? throw new Exception("User not found.");
            return await _bookingRepo.GetBookingsByUserIdAsync(userId);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e.InnerException);
        }
    }

    private async Task CanProceed(string userId, int lessonId)
    {
        //check if user Exists
        _ = await _userService.GetUserById(userId) ?? throw new Exception("User doesn't exist");
        //check if lesson Exists
        _ = await _lessonService.GetLessonByIdAsync(lessonId) ?? throw new Exception("Lesson doesn't exist");
    }

    public async Task<(bool weekly, int? amount)> CanBook(string userId, int lessonId)
    {
        var isWeekly =  ( true, 0);
        
        return isWeekly;
    }
    private bool IsSameWeek(DateTime date1, DateTime date2)
    {
        DateTime startOfWeek1 = date1.Date.AddDays(-(int)(date1.DayOfWeek == DayOfWeek.Sunday ? 6 : (date1.DayOfWeek - DayOfWeek.Monday)));
        DateTime startOfWeek2 = date2.Date.AddDays(-(int)(date2.DayOfWeek == DayOfWeek.Sunday ? 6 : (date2.DayOfWeek - DayOfWeek.Monday)));

        return startOfWeek1 == startOfWeek2;
    }
}
