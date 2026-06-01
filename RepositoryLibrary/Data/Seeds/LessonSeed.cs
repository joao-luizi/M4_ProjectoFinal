using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Lessons.Entities;


namespace RepositoryLibrary.Data.Seeds
{
    public static class LessonSeed
    {
        public static async Task SeedLessons(IServiceProvider serviceProvider)
        {
            try
            {
                var em_context = serviceProvider.GetRequiredService<RideReadyDbContext>();

                if (await em_context.Lessons.AnyAsync())
                    return;

                var volteio = await em_context.LessonTypes
                    .FirstOrDefaultAsync(x => x.Name == "Volteio");

                var sela1 = await em_context.LessonTypes
                    .FirstOrDefaultAsync(x => x.Name == "Sela 1");

                var sela2 = await em_context.LessonTypes
                    .FirstOrDefaultAsync(x => x.Name == "Sela 2");

                var individual = await em_context.LessonTypes
                    .FirstOrDefaultAsync(x => x.Name == "Individual");

                var school = await em_context.Schools.FirstOrDefaultAsync();

                if (school == null
                    || volteio == null
                    || sela1 == null
                    || sela2 == null
                    || individual == null)
                {
                    throw new Exception("Missing required School or LessonType records.");
                }

                var lessons = new List<Lesson>();

                // Semana atual como referência
                DateTime today = DateTime.Today;

                // Encontrar segunda-feira da semana atual
                int diff = today.DayOfWeek == DayOfWeek.Sunday
                    ? 6
                    : (int)today.DayOfWeek - 1;

                DateTime currentWeekMonday = today.AddDays(-diff);

                // Semana anterior + semana atual + próximas 2 semanas
                DateTime startWeek = currentWeekMonday.AddDays(-7);

                // 4 semanas no total
                for (int week = 0; week < 4; week++)
                {
                    DateTime weekStart = startWeek.AddDays(week * 7);

                    lessons.AddRange(new List<Lesson>
            {
                // Segunda - Volteio
                new Lesson
                {
                    LessonType = volteio,
                    School = school,
                    MaxSpots = 6,
                    BeginOfLesson = weekStart
                        .AddDays(0)
                        .AddHours(9),

                    EndOfLesson = weekStart
                        .AddDays(0)
                        .AddHours(9)
                        .AddMinutes(30)
                },

                // Terça - Sela 1
                new Lesson
                {
                    LessonType = sela1,
                    School = school,
                    MaxSpots = 5,
                    BeginOfLesson = weekStart
                        .AddDays(1)
                        .AddHours(10),

                    EndOfLesson = weekStart
                        .AddDays(1)
                        .AddHours(10)
                        .AddMinutes(40)
                },

                // Quarta - Sela 2
                new Lesson
                {
                    LessonType = sela2,
                    School = school,
                    MaxSpots = 4,
                    BeginOfLesson = weekStart
                        .AddDays(2)
                        .AddHours(11),

                    EndOfLesson = weekStart
                        .AddDays(2)
                        .AddHours(11)
                        .AddMinutes(50)
                },

                // Quinta - Individual
                new Lesson
                {
                    LessonType = individual,
                    School = school,
                    MaxSpots = 1,
                    BeginOfLesson = weekStart
                        .AddDays(3)
                        .AddHours(12),

                    EndOfLesson = weekStart
                        .AddDays(3)
                        .AddHours(12)
                        .AddMinutes(50)
                }
            });
                }

                await em_context.Lessons.AddRangeAsync(lessons);
                await em_context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding lessons: {ex.Message}");
            }
        }
    }
}
