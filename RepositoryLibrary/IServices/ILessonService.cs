using RepositoryLibrary.Models;

namespace RepositoryLibrary.IServices
{
    public interface ILessonService
    {
        Task<IEnumerable<Lesson>> GetAllLessonsAsync();
        Task<Lesson?> GetLessonByIdAsync(int lessonId);
        Task<bool> CreateLessonAsync(Lesson lesson);
        Task<bool> UpdateLessonAsync(Lesson lesson);
        Task<bool> DeleteLessonAsync(int lessonId);

        Task<IEnumerable<Lesson>> GetLessonsByTeacherAsync(string teacherId);
        Task<IEnumerable<Lesson>> GetLessonsByStudentAsync(string studentId);
        Task<IEnumerable<Lesson>> GetAvailableLessonsAsync();
        Task<IEnumerable<Lesson>> GetLessonsByHorseIdAsync(int horseId);
        Task<IEnumerable<Lesson>> GetLessonsByDateAsync(DateTime date);
        Task<List<Lesson>> GetLessonByHorseAndDate(int horseId, DateTime date);
    }

}
