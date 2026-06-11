

using RepositoryLibrary.Features.Entitlements.DTOs;
using RepositoryLibrary.Features.Lessons.DTOs;
using RepositoryLibrary.Features.Lessons.Entities;

namespace RepositoryLibrary.Features.Lessons.Interfaces
{
    public interface ILessonService
    {
        //V2 implemented
        Task<int> CreateAsync(LessonEditDto dto);
        //V2 implemented
        Task UpdateAsync(LessonEditDto dto);
        //V2 implemented
        Task <int> DeleteAsync(int lessonId);
        //V2 implemented
        Task AssignHorseAsync(int lessonId, int horseId);
        //V2 implemented
        Task AssignTeacherAsync(int lessonId, string userId);
        //V2 implemented
        Task ChangeCapacityAsync(int lessonId, int maxSpots);

        //V2 implemented
        Task<LessonEditDto> GetForEditAsync(int lessonId);
        //V2 implemented
        Task<LessonAttendanceDto> GetAttendanceAsync(int lessonId);
        //V2 implemented
        Task UpdateAttendanceAsync(LessonAttendanceDto dto);

        Task CancelBookingAsync(int lessonId, string userId);
        //V2 implemented
        Task<BookingResult> BookLessonAsync(int lessonId, string userId);
    }

}
