using RepositoryLibrary.Models;

namespace RepositoryLibrary.IRepository
{
    public interface ILessonRepository
    {
        Task<IEnumerable<Lesson>> GetAllLessonsAsync();
        Task<Lesson?> GetLessonByIdAsync(int lessonId);
        Task CreateLessonAsync(Lesson lesson);
        Task UpdateLessonAsync(Lesson lesson);
        Task DeleteLessonAsync(int lessonId);

        // Filtering & retrieval methods
        Task<IEnumerable<Lesson>> GetLessonsByTeacherAsync(string teacherId);
        Task<IEnumerable<Lesson>> GetLessonsByStudentAsync(string studentId);
        Task<IEnumerable<Lesson>> GetAvailableLessonsAsync();
        Task<IEnumerable<Lesson>> GetLessonsByHorseIdAsync(int horseId);
        Task<IEnumerable<Lesson>> GetLessonsByDateAsync(DateTime date);
        Task<List<Lesson>> GetLessonByHorseAndDate(int horseId, DateTime date);
    }

}
