using Microsoft.Extensions.Logging;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Lessons.Entities;
using RepositoryLibrary.Features.Lessons.Interfaces;
using RepositoryLibrary.Features.Lessons.Repositories;
using RepositoryLibrary.Features.Users.Entities;
using RepositoryLibrary.Features.Users.Interfaces;

namespace RepositoryLibrary.Features.Lessons.Services
{
    public class LessonService : ILessonService
    {
        private readonly LessonRepository _lessonRepository;
        private readonly IUserService _userService;
        private readonly ILogger<LessonService> _logger;

        public LessonService(RideReadyDbContext context, IUserService userService, ILogger<LessonService> logger, ILogger<LessonRepository> repoLogger)
        {
            _lessonRepository = new LessonRepository(context, repoLogger);
            _userService = userService;
            _logger = logger;
        }

        public async Task<IEnumerable<Lesson>> GetAllLessonsAsync()
        {
            _logger.LogInformation("A obter a lista de todas as aulas.");
            var lessons = await _lessonRepository.GetAllLessonsAsync();
            _logger.LogInformation("Obtidas {Count} aulas.", lessons.Count());
            return lessons;
        }

        public async Task<Lesson?> GetLessonByIdAsync(int lessonId)
        {
            _logger.LogInformation("A obter a aula {LessonId}.", lessonId);
            var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId);

            if (lesson == null)
            {
                _logger.LogWarning("Aula {LessonId} não encontrada.", lessonId);
            }
            else
            {
                _logger.LogInformation("Aula {LessonId} obtida com sucesso.", lessonId);
            }

            return lesson;
        }

        public async Task<bool> CreateLessonAsync(Lesson lesson)
        {
            _logger.LogInformation("A criar uma nova aula.");

            if (lesson == null || lesson.MaxSpots <= 0)
            {
                _logger.LogWarning("Tentativa de criar aula inválida (lesson nula: {IsNull}, MaxSpots: {MaxSpots}).", lesson == null, lesson?.MaxSpots);
                return false;
            }

            await _lessonRepository.CreateLessonAsync(lesson);
            _logger.LogInformation("Aula {LessonId} criada com sucesso.", lesson.LessonId);
            return true;
        }

        public async Task<bool> UpdateLessonAsync(Lesson lesson)
        {
            _logger.LogInformation("A atualizar a aula {LessonId}.", lesson?.LessonId);

            if (lesson == null || lesson.MaxSpots <= 0)
            {
                _logger.LogWarning("Tentativa de atualizar aula inválida (lesson nula: {IsNull}, MaxSpots: {MaxSpots}).", lesson == null, lesson?.MaxSpots);
                return false;
            }

            await _lessonRepository.UpdateLessonAsync(lesson);
            _logger.LogInformation("Aula {LessonId} atualizada com sucesso.", lesson.LessonId);
            return true;
        }

        public async Task<bool> DeleteLessonAsync(int lessonId)
        {
            _logger.LogInformation("A eliminar a aula {LessonId}.", lessonId);

            var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId);
            if (lesson == null)
            {
                _logger.LogWarning("Não foi possível eliminar: aula {LessonId} não encontrada.", lessonId);
                return false;
            }

            await _lessonRepository.DeleteLessonAsync(lessonId);
            _logger.LogInformation("Aula {LessonId} eliminada com sucesso.", lessonId);
            return true;
        }

        public async Task<IEnumerable<Lesson>> GetLessonsByTeacherAsync(string teacherId)
        {
            _logger.LogInformation("A obter aulas do professor {TeacherId}.", teacherId);
            try
            {
                var roles = await _userService.GetUserRole(teacherId);

                IEnumerable<Lesson> lessons = new List<Lesson>();

                if (!roles.Contains(StaticRole.Teacher))
                {
                    _logger.LogWarning("Utilizador {TeacherId} não tem papel de professor.", teacherId);
                    throw new Exception($"The user with Id = {teacherId} is not a teacher.");
                }

                lessons = await _lessonRepository.GetLessonsByTeacherAsync(teacherId);

                _logger.LogInformation("Obtidas {Count} aulas para o professor {TeacherId}.", lessons.Count(), teacherId);
                return lessons;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao obter aulas do professor {TeacherId}.", teacherId);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<IEnumerable<Lesson>> GetLessonsByStudentAsync(string studentId)
        {
            _logger.LogInformation("A obter aulas do aluno {StudentId}.", studentId);
            try
            {
                var roles = await _userService.GetUserRole(studentId);

                IEnumerable<Lesson> lessons = new List<Lesson>();

                if (!roles.Contains(StaticRole.Student))
                {
                    _logger.LogWarning("Utilizador {StudentId} não tem papel de aluno.", studentId);
                    throw new Exception($"The user with Id = {studentId} is not a teacher.");
                }

                lessons = await _lessonRepository.GetLessonsByStudentAsync(studentId);

                _logger.LogInformation("Obtidas {Count} aulas para o aluno {StudentId}.", lessons.Count(), studentId);
                return lessons;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao obter aulas do aluno {StudentId}.", studentId);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<IEnumerable<Lesson>> GetAvailableLessonsAsync()
        {
            _logger.LogInformation("A obter aulas disponíveis.");
            var lessons = await _lessonRepository.GetAvailableLessonsAsync();
            _logger.LogInformation("Obtidas {Count} aulas disponíveis.", lessons.Count());
            return lessons;
        }

        public async Task<IEnumerable<Lesson>> GetLessonsByHorseIdAsync(int horseId)
        {
            _logger.LogInformation("A obter aulas do cavalo {HorseId}.", horseId);
            var lessons = await _lessonRepository.GetLessonsByHorseIdAsync(horseId);
            _logger.LogInformation("Obtidas {Count} aulas para o cavalo {HorseId}.", lessons.Count(), horseId);
            return lessons;
        }

        public async Task<IEnumerable<Lesson>> GetLessonsByDateAsync(DateTime date)
        {
            _logger.LogInformation("A obter aulas para {Date:yyyy-MM-dd}.", date);
            var lessons = await _lessonRepository.GetLessonsByDateAsync(date);
            _logger.LogInformation("Obtidas {Count} aulas para {Date:yyyy-MM-dd}.", lessons.Count(), date);
            return lessons;
        }

        public async Task<List<Lesson>> GetLessonByHorseAndDate(int horseId, DateTime date)
        {
            _logger.LogInformation("A obter aulas do cavalo {HorseId} em {Date:yyyy-MM-dd}.", horseId, date);
            try
            {
                var lessons = await _lessonRepository.GetLessonByHorseAndDate(horseId, date);
                _logger.LogInformation("Obtidas {Count} aulas para o cavalo {HorseId} em {Date:yyyy-MM-dd}.", lessons.Count, horseId, date);
                return lessons;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao obter aulas do cavalo {HorseId} em {Date:yyyy-MM-dd}.", horseId, date);
                throw new Exception(e.Message, e.InnerException);
            }
        }
    }
}