

using RepositoryLibrary.Features.Lessons.Entities;

namespace RepositoryLibrary.Features.Lessons.Interfaces
{
    public interface ILessonRepository
    {
        //V2 Implemented
        Task<int> CountFutureLessonsAsync(DateTime now);


        //V2 Implemented
        Task<List<Lesson>> GetLessonsByTeacherAsync(string teacherId);

        //V2 Implemented
        Task<List<Lesson>> GetLessonsByDateRangeAsync(DateTime from, DateTime to, int selectedSchoolId);

        //V2 Implemented
        Task<Lesson?> GetByIdWithDetailsAsync(int lessonId);
        //V2 Implemented
        Task AddAsync(Lesson lesson);

        //V2 Implemented
        Task UpdateAsync(Lesson lesson);
        //V2 Implemented
        Task DeleteAsync(Lesson lesson);

    }

}
