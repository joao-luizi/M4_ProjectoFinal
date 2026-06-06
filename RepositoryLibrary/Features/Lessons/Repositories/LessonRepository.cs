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
                .ToListAsync();
            _logger.LogInformation("BD: obtidas {Count} aulas para o professor {TeacherId}.", lessons.Count, teacherId);
            return lessons;
        }

       
    }
}