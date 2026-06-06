using Microsoft.Extensions.Logging;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Bookings.Interfaces;
using RepositoryLibrary.Features.DashBoard.DTOs;
using RepositoryLibrary.Features.DashBoard.Interfaces;
using RepositoryLibrary.Features.Horses.Interfaces;
using RepositoryLibrary.Features.Lessons.Interfaces;
using RepositoryLibrary.Features.Users.Entities;
using RepositoryLibrary.Features.Users.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.DashBoard.Services
{
    public  class AdminDashboardService : IAdminDashboardService
    {
     
        private readonly ILogger<AdminDashboardService> _logger;
        private readonly RideReadyDbContext _context;
        private readonly IUserRepository _userRepo;
        private readonly ILessonRepository _lessonRepo;
        private readonly IBookingRepository _bookingRepo;
        private readonly IHorseRepository _horseRepo; 
        public AdminDashboardService(RideReadyDbContext context, 
            IUserRepository userRepo, 
            IBookingRepository bookingRepo,
            ILessonRepository lessonRepo,
            IHorseRepository horseRepo, 
            ILogger<AdminDashboardService> logger) 
        { 
            _context = context;
            _userRepo = userRepo;
            _bookingRepo = bookingRepo;
            _horseRepo = horseRepo;
            _lessonRepo = lessonRepo;
            _logger = logger;
        }

        //V2 Implemented
        public async Task<AdminStudentDashboardDto> GetAdminDashboardAsync()
        {
            try
            {
                var students = await _userRepo.GetUsersByRoleAsync(StaticRole.Student);

                var studentIds = students.Select(s => s.Id).ToList();

                var bookings = await _bookingRepo.GetBookingsForStudentsAsync(studentIds);

                var totalHorses = await _horseRepo.CountAsync();

                var scheduledClasses = await _lessonRepo.CountFutureLessonsAsync(DateTime.Now);

                var now = DateTime.Now;

                var lastBookingsByUser = bookings
                    .Where(b => b.WasPresent && b.Lesson.BeginOfLesson <= now)
                    .GroupBy(b => b.UserId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.OrderByDescending(x => x.Lesson.BeginOfLesson).FirstOrDefault()
                    );

                var studentDtos = students.Select(s => new StudentDashboardItemDto
                {
                    Name = $"{s.FirstName} {s.LastName}",
                    ClassesRemaining = 0,
                    PaymentStatus = "paid",

                    LastClass = lastBookingsByUser.ContainsKey(s.Id)
                        ? lastBookingsByUser[s.Id]?.Lesson?.BeginOfLesson
                        : null
                }).ToList();

                return new AdminStudentDashboardDto
                {
                    Students = studentDtos,
                    TotalStudents = students.Count,
                    ScheduledClasses = scheduledClasses,
                    TotalHorses = totalHorses,
                    MissingPayments = 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building student dashboard");
                throw;
            }
        }
    }
}
