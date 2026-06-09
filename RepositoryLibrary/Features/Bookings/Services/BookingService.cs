
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Users.Interfaces;
using RepositoryLibrary.Features.Bookings.Interfaces;
using RepositoryLibrary.Features.Bookings.Entities;
using RepositoryLibrary.Features.Schools.Interfaces;
using RepositoryLibrary.Features.Lessons.Interfaces;
using RepositoryLibrary.Features.Bookings.Repositories;

namespace RepositoryLibrary.Features.Bookings.Services;

public class BookingService : IBookingService
{
   
    private readonly IUserService _userService;
  
    private readonly ILessonService _lessonService;
    private readonly ISchoolService _schoolService;
    public BookingService(RideReadyDbContext _context, IUserService userService, ILessonService lessonService)
    {
      
      
        _userService = userService;
        _lessonService = lessonService;
    }

     















}
