using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Lessons.Entities;
using RepositoryLibrary.Features.Lessons.Interfaces;

namespace RepositoryLibrary.Features.Lessons.Repositories
{
    public class LessonRepository : ILessonRepository
    {
        private readonly RideReadyDbContext _emContext;
        private readonly ILogger<LessonRepository> _logger;

        public LessonRepository(RideReadyDbContext context, ILogger<LessonRepository> logger)
        {
            _emContext = context;
            _logger = logger;
        }

        //V2 Implemented
        public async Task<int> CountFutureLessonsAsync(DateTime now)
        {
            return await _emContext.Lessons
                .Where(l => l.BeginOfLesson >= now)
                .CountAsync();
        }


        //V2 Implemented
        public async Task<List<Lesson>> GetLessonsByTeacherAsync(string teacherId)
        {
            _logger.LogInformation("BD: a consultar aulas do professor {TeacherId}.", teacherId);
            var lessons = await _emContext.Lessons
                .Where(l => l.LessonProfs.Any(lp => lp.UserId == teacherId))
                .Include(l => l.School)
               .Include(l => l.LessonType)
               .Include(l => l.Bookings)
               .Include(l => l.LessonProfs)
               .Include(l => l.LessonHorses)
                .ToListAsync();
            _logger.LogInformation("BD: obtidas {Count} aulas para o professor {TeacherId}.", lessons.Count, teacherId);
            return lessons;
        }
        //V2 Implemented
        public async Task<List<Lesson>> GetLessonsByDateRangeAsync(DateTime from, DateTime to, int selectedSchoolId)
        {
            return await _emContext.Lessons
               .Where(l => l.BeginOfLesson >= from && l.BeginOfLesson < to && l.School.SchoolId == selectedSchoolId)
               .Include(l => l.School)
               .Include(l => l.LessonType)
               .Include(l => l.Bookings)
               .Include(l => l.LessonProfs)
               .Include(l => l.LessonHorses)
               .ToListAsync();
        }
        //V2 Implemented
        public async Task<Lesson?> GetByIdWithDetailsAsync(int lessonId)
        {
            return await _emContext.Lessons
              .Where(l => l.LessonId == lessonId)
              .Include(l => l.School)
              .Include(l => l.LessonType)
              .Include(l => l.Bookings)
              .Include(l => l.LessonProfs)
              .Include(l => l.LessonHorses)
              .FirstOrDefaultAsync();
        }
        //V2 Implemented
        public async Task AddAsync(Lesson lesson)
        {
            await _emContext.Lessons.AddAsync(lesson);
            await _emContext.SaveChangesAsync();
        }

        //V2 Implemented
        public async Task UpdateAsync(Lesson lesson)
        {
            await _emContext.SaveChangesAsync();
        }
        //V2 Implemented
        public async Task SaveChangesAsync()
        {
            await _emContext.SaveChangesAsync();
        }

        //V2 Implemented
        public async Task DeleteAsync(Lesson lesson)
        {
            var lessonHorses = await _emContext.LessonHorses
                .Where(x => x.LessonId == lesson.LessonId)
                .ToListAsync();

            var lessonProfs = await _emContext.LessonProfs
            .Where(x => x.LessonId == lesson.LessonId)
            .ToListAsync();

            _emContext.LessonHorses.RemoveRange(lessonHorses);
            _emContext.LessonProfs.RemoveRange(lessonProfs);
            _emContext.Lessons.Remove(lesson);

            await _emContext.SaveChangesAsync();
        }

    }
}