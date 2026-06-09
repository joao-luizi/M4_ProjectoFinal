using RepositoryLibrary.Features.Lessons.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Lessons.Interfaces
{
    public interface ILessonScheduleService
    {
        Task<MonthScheduleDto> GetMonthlyScheduleAsync(int year, int month, int selectedSchoolId);

        Task<List<DayScheduleDto>> GetMonthDaysSummaryAsync(int year, int month, int selectedSchoolId);

        Task<DayScheduleDto> GetDayScheduleAsync(DateOnly date, int selectedSchoolId);

        Task<LessonCalendarDto> GetLessonDetailsAsync(int lessonId);
    }
}
