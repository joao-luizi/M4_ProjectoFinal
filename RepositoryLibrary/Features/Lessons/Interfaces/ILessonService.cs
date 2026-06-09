

using RepositoryLibrary.Features.Lessons.DTOs;
using RepositoryLibrary.Features.Lessons.Entities;

namespace RepositoryLibrary.Features.Lessons.Interfaces
{
    public interface ILessonService
    {
        Task<int> CreateAsync(LessonEditDto dto);
        Task UpdateAsync(LessonEditDto dto);
        Task DeleteAsync(int lessonId);

        Task AssignHorseAsync(int lessonId, int horseId);
        Task AssignTeacherAsync(int lessonId, string userId);

        Task ChangeCapacityAsync(int lessonId, int maxSpots);
    }

}
