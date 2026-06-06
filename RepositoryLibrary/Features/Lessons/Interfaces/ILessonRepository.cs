

using RepositoryLibrary.Features.Lessons.Entities;

namespace RepositoryLibrary.Features.Lessons.Interfaces
{
    public interface ILessonRepository
    {
        //V2 Implemented
        Task<int> CountFutureLessonsAsync(DateTime now);
        //Task<IEnumerable<Lesson>> GetAllLessonsAsync();
        //Task<Lesson?> GetLessonByIdAsync(int lessonId);
        //Task CreateLessonAsync(Lesson lesson);
        //Task UpdateLessonAsync(Lesson lesson);
        //Task DeleteLessonAsync(int lessonId);

        //V2 Implemented
        Task<List<Lesson>> GetLessonsByTeacherAsync(string teacherId);
        //Task<List<Lesson>> GetLessonsByStudentAsync(string studentId);
        //Task<List<Lesson>> GetAvailableLessonsAsync();
        //Task<List<Lesson>> GetLessonsByHorseIdAsync(int horseId);
        //Task<List<Lesson>> GetLessonsByDateAsync(DateTime date);
        //Task<List<Lesson>> GetLessonByHorseAndDate(int horseId, DateTime date);


    }

}
