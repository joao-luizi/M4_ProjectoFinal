using RepositoryLibrary.IRepository;
using RepositoryLibrary.IServices;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using RepositoryLibrary.Repository;
using SharedLibrary.Models.Static_Class;

namespace RepositoryLibrary.Services
{
    public class LessonService : ILessonService
    {
        private readonly LessonRepository _lessonRepository;
        private readonly IUserService _userService;

        public LessonService(EM_DbContext context, IUserService userService)
        {
            _lessonRepository = new LessonRepository(context);
            _userService = userService;
        }

        public async Task<IEnumerable<Lesson>> GetAllLessonsAsync()
        {
            return await _lessonRepository.GetAllLessonsAsync();
        }

        public async Task<Lesson?> GetLessonByIdAsync(int lessonId)
        {
            return await _lessonRepository.GetLessonByIdAsync(lessonId);
        }

        public async Task<bool> CreateLessonAsync(Lesson lesson)
        {
            if (lesson == null || lesson.MaxSpots <= 0)
                return false;

            await _lessonRepository.CreateLessonAsync(lesson);
            return true;
        }

        public async Task<bool> UpdateLessonAsync(Lesson lesson)
        {
            if (lesson == null || lesson.MaxSpots <= 0)
                return false;

            await _lessonRepository.UpdateLessonAsync(lesson);
            return true;
        }

        public async Task<bool> DeleteLessonAsync(int lessonId)
        {
            var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId);
            if (lesson == null) return false;

            await _lessonRepository.DeleteLessonAsync(lessonId);
            return true;
        }

        public async Task<IEnumerable<Lesson>> GetLessonsByTeacherAsync(string teacherId)
        {
            try
            {
                var roles = await _userService.GetUserRole(teacherId);

                IEnumerable<Lesson> lessons = new List<Lesson>();

                if (!roles.Contains(StaticRole.Teacher))
                {
                    throw new Exception($"The user with Id = {teacherId} is not a teacher.");
                }

                lessons = await _lessonRepository.GetLessonsByTeacherAsync(teacherId);

                return lessons;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<IEnumerable<Lesson>> GetLessonsByStudentAsync(string studentId)
        {
            try
            {
                var roles = await _userService.GetUserRole(studentId);

                IEnumerable<Lesson> lessons = new List<Lesson>();

                if (!roles.Contains(StaticRole.Student))
                {
                    throw new Exception($"The user with Id = {studentId} is not a teacher.");
                }

                lessons = await _lessonRepository.GetLessonsByStudentAsync(studentId);

                return lessons;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<IEnumerable<Lesson>> GetAvailableLessonsAsync()
        {
            return await _lessonRepository.GetAvailableLessonsAsync();
        }

        public async Task<IEnumerable<Lesson>> GetLessonsByHorseIdAsync(int horseId)
        {
            return await _lessonRepository.GetLessonsByHorseIdAsync(horseId);
        }

        public async Task<IEnumerable<Lesson>> GetLessonsByDateAsync(DateTime date)
        {
            return await _lessonRepository.GetLessonsByDateAsync(date);
        }

        public async Task<List<Lesson>> GetLessonByHorseAndDate(int horseId, DateTime date)
        {
            try
            {
                return await _lessonRepository.GetLessonByHorseAndDate(horseId, date);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
    }
}
