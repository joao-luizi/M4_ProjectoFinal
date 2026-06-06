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
    public class TeacherDashboardService : ITeacherDashboardService
    {

        private readonly ILogger<AdminDashboardService> _logger;
        private readonly RideReadyDbContext _context;
        private readonly IUserRepository _userRepo;
        private readonly ILessonRepository _lessonRepo;
        private readonly IBookingRepository _bookingRepo;
        private readonly IHorseRepository _horseRepo;
        public TeacherDashboardService(RideReadyDbContext context,
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
        public async Task<TeacherDashboardDto> GetTeacherDashboardAsync(string teacherId)
        {
            try
            {

                // 1. Get all lessons for this teacher
                var teacherLessons = await _lessonRepo.GetLessonsByTeacherAsync(teacherId);

                //2. Future lessons
                var now = DateTime.UtcNow;

                var futureLessons = teacherLessons
                    .Where(l => l.BeginOfLesson >= now)
                    .ToList();

                
                //3. Students
                var studentIds = teacherLessons
                .SelectMany(l => l.Bookings)
                .Select(b => b.UserId)
                .Distinct()
                .ToList();

                var students = await _userRepo.GetUsersByIdsAsync(studentIds);

                var lessonsByStudent = teacherLessons
                .SelectMany(l => l.Bookings.Select(b => new { b.UserId, Lesson = l }))
                .GroupBy(x => x.UserId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.Lesson).ToList()
                );

                var studentItems = students.Select(s =>
                {
                    var studentLessons = teacherLessons
                        .Where(l => l.Bookings.Any(b => b.UserId == s.Id))
                        .ToList();

                    var lastLesson = studentLessons
                        .OrderByDescending(l => l.BeginOfLesson)
                        .FirstOrDefault();

                    var classesRemaining = futureLessons
                        .Count(l => l.Bookings.Any(b => b.UserId == s.Id));

                    return new TeacherStudentDashboardItemDto
                    {
                        Name = s.FirstName + " " + s.LastName,
                        ClassesRemaining = classesRemaining,
                        LastClass = lastLesson?.BeginOfLesson
                    };
                }).ToList();

                var nextLesson = futureLessons
                    .OrderBy(l => l.BeginOfLesson)
                    .FirstOrDefault();

                TeacherNextLessonDto? nextLessonDto = null;

                if (nextLesson != null)
                {
                    var studentNames = nextLesson.Bookings
                        .Select(b => students.FirstOrDefault(s => s.Id == b.UserId))
                        .Where(s => s != null)
                        .Select(s => s!.FirstName + " " + s.LastName)
                        .ToList();

                    nextLessonDto = new TeacherNextLessonDto
                    {
                        LessonDate = nextLesson.BeginOfLesson,
                        StudentNames = studentNames
                    };
                }

                //4. Horses
                var horseIds = teacherLessons
                .SelectMany(l => l.LessonHorses)
                .Select(h => h.HorseId)
                .Distinct()
                .ToList();

                var horses = await _horseRepo.GetByIdsAsync(horseIds);

                var scheduledClasses = futureLessons.Count;

                var totalHorses = horses.Count;

                return new TeacherDashboardDto
                {
                    TotalStudents = studentIds.Count,
                    ScheduledClasses = scheduledClasses,
                    TotalHorses = totalHorses,
                    NextLesson = nextLessonDto,
                    Students = studentItems
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building teacher dashboard");
                throw;
            }
        }
    }
}
