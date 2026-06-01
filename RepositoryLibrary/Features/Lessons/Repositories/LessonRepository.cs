using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

        public async Task<IEnumerable<Lesson>> GetAllLessonsAsync()
        {
            _logger.LogInformation("BD: a consultar todas as aulas.");
            var lessons = await _emContext.Lessons
                .Include(l => l.LessonType)
                .Include(l => l.Bookings)
                .Include(l => l.LessonProfs)
                .Include(l => l.LessonHorses).ThenInclude(lh => lh.Horse)
                .Include(l => l.School)
                .ToListAsync();
            _logger.LogInformation("BD: obtidas {Count} aulas.", lessons.Count);
            return lessons;
        }

        public async Task<Lesson?> GetLessonByIdAsync(int lessonId)
        {
            _logger.LogInformation("BD: a consultar aula {LessonId}.", lessonId);
            var lesson = await _emContext.Lessons
                .Include(l => l.LessonType)
                .Include(l => l.Bookings)
                .Include(l => l.LessonProfs).FirstOrDefaultAsync(l => l.LessonId == lessonId);

            if (lesson == null)
            {
                _logger.LogWarning("Aula {LessonId} não encontrada.", lessonId);
            }
            else
            {
                _logger.LogInformation("BD: aula {LessonId} obtida.", lessonId);
            }

            return lesson;
        }

        public async Task CreateLessonAsync(Lesson lesson)
        {
            _logger.LogInformation("BD: a criar nova aula.");
            _emContext.Lessons.Add(lesson);
            await _emContext.SaveChangesAsync();
            _logger.LogInformation("BD: aula {LessonId} criada.", lesson.LessonId);
        }

        public async Task UpdateLessonAsync(Lesson lesson)
        {
            _logger.LogInformation("BD: a atualizar aula {LessonId}.", lesson.LessonId);
            _emContext.Lessons.Update(lesson);
            await _emContext.SaveChangesAsync();
            _logger.LogInformation("BD: aula {LessonId} atualizada.", lesson.LessonId);
        }

        public async Task DeleteLessonAsync(int lessonId)
        {
            _logger.LogInformation("BD: a eliminar aula {LessonId}.", lessonId);
            var lesson = await _emContext.Lessons.FindAsync(lessonId);
            if (lesson != null)
            {
                _emContext.Lessons.Remove(lesson);
                await _emContext.SaveChangesAsync();
                _logger.LogInformation("BD: aula {LessonId} eliminada.", lessonId);
            }
            else
            {
                _logger.LogWarning("Aula {LessonId} não encontrada; nada a eliminar.", lessonId);
            }
        }

        public async Task<IEnumerable<Lesson>> GetLessonsByTeacherAsync(string teacherId)
        {
            _logger.LogInformation("BD: a consultar aulas do professor {TeacherId}.", teacherId);
            var lessons = await _emContext.Lessons
                .Where(l => l.LessonProfs.Any(lp => lp.UserId == teacherId))
                .ToListAsync();
            _logger.LogInformation("BD: obtidas {Count} aulas para o professor {TeacherId}.", lessons.Count, teacherId);
            return lessons;
        }

        public async Task<IEnumerable<Lesson>> GetLessonsByStudentAsync(string studentId)
        {
            _logger.LogInformation("BD: a consultar aulas do aluno {StudentId}.", studentId);
            var lessons = await _emContext.Lessons
                .Where(l => l.Bookings.Any(b => b.UserId == studentId))
                .ToListAsync();
            _logger.LogInformation("BD: obtidas {Count} aulas para o aluno {StudentId}.", lessons.Count, studentId);
            return lessons;
        }

        public async Task<IEnumerable<Lesson>> GetAvailableLessonsAsync()
        {
            _logger.LogInformation("BD: a consultar aulas disponíveis (com vagas).");
            var lessons = await _emContext.Lessons
                .Where(l => l.Bookings.Count() < l.MaxSpots)
                .ToListAsync();
            _logger.LogInformation("BD: obtidas {Count} aulas disponíveis.", lessons.Count);
            return lessons;
        }

        public async Task<IEnumerable<Lesson>> GetLessonsByHorseIdAsync(int horseId)
        {
            _logger.LogInformation("BD: a consultar aulas do cavalo {HorseId}.", horseId);
            var lessons = await _emContext.Lessons
                .Where(l => l.LessonHorses.Any(lh => lh.HorseId == horseId))
                .ToListAsync();
            _logger.LogInformation("BD: obtidas {Count} aulas para o cavalo {HorseId}.", lessons.Count, horseId);
            return lessons;
        }

        public async Task<IEnumerable<Lesson>> GetLessonsByDateAsync(DateTime date)
        {
            _logger.LogInformation("BD: a consultar aulas para {Date:yyyy-MM-dd}.", date);
            var lessons = await _emContext.Lessons
                .Where(l => l.BeginOfLesson.Date == date.Date)
                .ToListAsync();
            _logger.LogInformation("BD: obtidas {Count} aulas para {Date:yyyy-MM-dd}.", lessons.Count, date);
            return lessons;
        }

        public async Task<List<Lesson>> GetLessonByHorseAndDate(int horseId, DateTime date)
        {
            _logger.LogInformation("BD: a consultar aulas do cavalo {HorseId} em {Date:yyyy-MM-dd}.", horseId, date);
            try
            {
                //other possibility
                //var nextDay = date.Date.AddDays(1);
                // return await _context.Lessons
                //     .Include(l => l.LessonType)
                //     .Where(l => l.BeginOfLesson >= date.Date &&
                //                 l.BeginOfLesson < nextDay &&
                //                 l.LessonHorses.Any(lh => lh.HorseId == horseId))
                //     .ToListAsync();
                var lessons = await _emContext.Lessons.Include(l => l.LessonType).Where(l => DateTime.Compare(date.Date, l.BeginOfLesson.Date) == 0 && l.LessonHorses.Any(lh => lh.HorseId == horseId)).ToListAsync();
                _logger.LogInformation("BD: obtidas {Count} aulas para o cavalo {HorseId} em {Date:yyyy-MM-dd}.", lessons.Count, horseId, date);
                return lessons;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao consultar aulas do cavalo {HorseId} em {Date:yyyy-MM-dd}.", horseId, date);
                throw new Exception(e.Message, e.InnerException);
            }
        }




    }
}