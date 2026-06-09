using Microsoft.Extensions.Logging;
using RepositoryLibrary.Features.Lessons.DTOs;
using RepositoryLibrary.Features.Lessons.Entities;
using RepositoryLibrary.Features.Lessons.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Lessons.Services
{
    public class LessonScheduleService : ILessonScheduleService
    {
        private readonly ILessonRepository _lessonRepo;
        private readonly ILogger<LessonScheduleService> _logger;
        public LessonScheduleService(ILessonRepository lessonRepo, ILogger<LessonScheduleService> logger)
        {
            _lessonRepo = lessonRepo;
            _logger = logger;
        }
        public async Task<MonthScheduleDto> GetMonthlyScheduleAsync(int year, int month, int selectedSchoolId)
        {
            var from = new DateTime(year, month, 1);
            var to = from.AddMonths(1);

            var lessons = await _lessonRepo.GetLessonsByDateRangeAsync(from, to, selectedSchoolId);

            var grouped = lessons
                .GroupBy(l => DateOnly.FromDateTime(l.BeginOfLesson))
                .ToList();

            var monthDto = new MonthScheduleDto
            {
                Year = year,
                Month = month,
                Days = new List<DayScheduleDto>()
            };

            foreach (var dayGroup in grouped)
            {
                var dayDto = new DayScheduleDto
                {
                    Date = dayGroup.Key,
                    Lessons = dayGroup.Select(MapToDto).ToList()
                };

                monthDto.Days.Add(dayDto);
            }

            return monthDto;
        }

        public async Task<List<DayScheduleDto>> GetMonthDaysSummaryAsync(int year, int month, int selectedSchoolId)
        {
            var from = new DateTime(year, month, 1);
            var to = from.AddMonths(1);

            var lessons = await _lessonRepo.GetLessonsByDateRangeAsync(from, to, selectedSchoolId);

            var result = lessons
                .GroupBy(l => DateOnly.FromDateTime(l.BeginOfLesson))
                .Select(g => new DayScheduleDto
                {
                    Date = g.Key,
                    Lessons = new List<LessonCalendarDto>(), // vazio intencional
                })
                .ToList();

            return result;
        }

        public async Task<DayScheduleDto> GetDayScheduleAsync(DateOnly date, int selectedSchoolId)
        {
            var from = date.ToDateTime(TimeOnly.MinValue);
            var to = from.AddDays(1);

            var lessons = await _lessonRepo.GetLessonsByDateRangeAsync(from, to, selectedSchoolId);

            return new DayScheduleDto
            {
                Date = date,
                Lessons = lessons.Select(MapToDto).ToList()
            };
        }

        public async Task<LessonCalendarDto> GetLessonDetailsAsync(int lessonId)
        {
            var lesson = await _lessonRepo.GetByIdWithDetailsAsync(lessonId);

            if (lesson == null)
                throw new Exception($"Lesson {lessonId} not found");

            return MapToDto(lesson);
        }

        private LessonCalendarDto MapToDto(Lesson lesson)
        {
            return new LessonCalendarDto
            {
                LessonId = lesson.LessonId,
                SchoolName = lesson.School?.SchoolName,
                LessonType = lesson.LessonType?.Name,

                Begin = lesson.BeginOfLesson,
                End = lesson.EndOfLesson,

                MaxSpots = lesson.MaxSpots,
                BookedSpots = lesson.Bookings?.Count ?? 0,

                Teachers = lesson.LessonProfs?
                    .Select(p => p.UserId)
                    .ToList() ?? new(),

                Horses = lesson.LessonHorses?
                    .Select(h => h.HorseId.ToString())
                    .ToList() ?? new()
            };
        }
    }
}
