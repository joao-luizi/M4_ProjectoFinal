using Microsoft.Extensions.Logging;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Bookings.Entities;
using RepositoryLibrary.Features.Bookings.Interfaces;
using RepositoryLibrary.Features.Lessons.DTOs;
using RepositoryLibrary.Features.Lessons.Entities;
using RepositoryLibrary.Features.Lessons.Interfaces;


namespace RepositoryLibrary.Features.Lessons.Services
{
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepo;
        private readonly IBookingRepository _bookRepo;
        private readonly ILogger<LessonService> _logger;

        public LessonService(RideReadyDbContext context, ILessonRepository lessonRepo, IBookingRepository bookRepo, ILogger<LessonService> logger)
        {
            _lessonRepo = lessonRepo;
            _bookRepo = bookRepo;
            _logger = logger;
        }

        private async Task ValidateHorseAvailabilityAsync(
    LessonEditDto dto)
        {
            // TODO:
            // Validate horse workload rules.
            //
            // Rules:
            // - Max 2 lessons per horse/day
            // - If only rides, max 4 rides/day
            // - After 1 ride -> only 1 lesson allowed
            // - After 2 rides -> no lessons allowed
            // - Horses must have 2 rest days/week
        }
        public async Task<int> CreateAsync(LessonEditDto dto)
        {

            //await ValidateHorseAvailabilityAsync(dto);

            var lesson = new Lesson
            {
                SchoolId = dto.SchoolId,
                LessonTypeId = dto.LessonTypeId,
                BeginOfLesson = dto.BeginOfLesson,
                EndOfLesson = dto.EndOfLesson,
                MaxSpots = dto.MaxSpots,

                Bookings = new List<Booking>(),
                LessonProfs = new List<LessonProf>(),
                LessonHorses = new List<LessonHorse>()
            };

            if (dto.TeacherIds != null)
            {
                foreach (var teacherId in dto.TeacherIds)
                {
                    lesson.LessonProfs.Add(new LessonProf
                    {
                        UserId = teacherId,
                        Lesson = lesson
                    });
                }
            }

            if (dto.HorseIds != null)
            {
                foreach (var horseId in dto.HorseIds)
                {
                    lesson.LessonHorses.Add(new LessonHorse
                    {
                        HorseId = horseId,
                        Lesson = lesson
                    });
                }
            }

            await _lessonRepo.AddAsync(lesson);

            return lesson.LessonId;
        }
        public async Task UpdateAsync(LessonEditDto dto)
        {
            var lesson = await _lessonRepo.GetByIdWithDetailsAsync(dto.LessonId);

            if (lesson == null)
                throw new Exception($"Lesson {dto.LessonId} not found");

            lesson.BeginOfLesson = dto.BeginOfLesson;
            lesson.EndOfLesson = dto.EndOfLesson;

            // Capacity rule: não pode ser menor que bookings existentes
            var currentBookings = lesson.Bookings?.Count ?? 0;

            if (dto.MaxSpots < currentBookings)
                throw new Exception("Cannot reduce capacity below current bookings");

            lesson.MaxSpots = dto.MaxSpots;

            await _lessonRepo.UpdateAsync(lesson);
        }
        public async Task DeleteAsync(int lessonId)
        {
            var lesson = await _lessonRepo.GetByIdWithDetailsAsync(lessonId);

            if (lesson == null)
                throw new Exception($"Lesson {lessonId} not found");

            // regra: não apagar se já tem bookings confirmados (opcional mas recomendado)
            if (lesson.Bookings.Any())
                throw new Exception("Cannot delete lesson with bookings");

            await _lessonRepo.DeleteAsync(lesson);
        }
        public async Task AssignHorseAsync(int lessonId, int horseId)
        {
            var lesson = await _lessonRepo.GetByIdWithDetailsAsync(lessonId);

            if (lesson == null)
                throw new Exception("Lesson not found");

            var exists = lesson.LessonHorses.Any(h => h.HorseId == horseId);

            if (exists)
                return;

            lesson.LessonHorses.Add(new LessonHorse
            {
                LessonId = lessonId,
                HorseId = horseId
            });

            await _lessonRepo.UpdateAsync(lesson);
        }
        public async Task AssignTeacherAsync(int lessonId, string userId)
        {
            var lesson = await _lessonRepo.GetByIdWithDetailsAsync(lessonId);

            if (lesson == null)
                throw new Exception("Lesson not found");

            var exists = lesson.LessonProfs.Any(p => p.UserId == userId);

            if (exists)
                return;

            lesson.LessonProfs.Add(new LessonProf
            {
                LessonId = lessonId,
                UserId = userId
            });

            await _lessonRepo.UpdateAsync(lesson);
        }
        public async Task ChangeCapacityAsync(int lessonId, int maxSpots)
        {
            var lesson = await _lessonRepo.GetByIdWithDetailsAsync(lessonId);

            if (lesson == null)
                throw new Exception("Lesson not found");

            var bookings = lesson.Bookings?.Count ?? 0;

            if (maxSpots < bookings)
                throw new Exception("Cannot reduce capacity below current bookings");

            lesson.MaxSpots = maxSpots;

            await _lessonRepo.UpdateAsync(lesson);
        }
    }
}