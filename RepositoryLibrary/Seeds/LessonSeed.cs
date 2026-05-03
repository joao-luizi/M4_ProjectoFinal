using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RepositoryLibrary.Seeds
{
    public static class LessonSeed
    {
        public static async Task SeedLessons(IServiceProvider serviceProvider)
        {
            try
            {
                var em_context = serviceProvider.GetRequiredService<EM_DbContext>();

                if (await em_context.Lessons.AnyAsync())
                    return;

                var volteio = await em_context.LessonTypes.FirstOrDefaultAsync(x => x.Name == "Volteio");
                var sela1 = await em_context.LessonTypes.FirstOrDefaultAsync(x => x.Name == "Sela 1");
                var sela2 = await em_context.LessonTypes.FirstOrDefaultAsync(x => x.Name == "Sela 2");
                var individual = await em_context.LessonTypes.FirstOrDefaultAsync(x => x.Name == "Individual");

                var school = await em_context.Schools.FirstOrDefaultAsync();

                if (school == null || volteio == null || sela1 == null || sela2 == null || individual == null)
                    throw new Exception("Missing required School or LessonType records.");

                var lessons = new List<Lesson>
                {
                    new Lesson
                    {
                        LessonType = volteio,
                        School = school,
                        MaxSpots = 6,
                        BeginOfLesson = new DateTime(2025, 4, 20, 9, 0, 0),
                        EndOfLesson = new DateTime(2025, 4, 20, 9, 30, 0)
                    },
                    new Lesson
                    {
                        LessonType = sela1,
                        School = school,
                        MaxSpots = 5,
                        BeginOfLesson = new DateTime(2025, 4, 21, 10, 0, 0),
                        EndOfLesson = new DateTime(2025, 4, 21, 10, 40, 0)
                    },
                    new Lesson
                    {
                        LessonType = sela2,
                        School = school,
                        MaxSpots = 4,
                        BeginOfLesson = new DateTime(2025, 4, 22, 11, 0, 0),
                        EndOfLesson = new DateTime(2025, 4, 22, 11, 50, 0)
                    },
                    new Lesson
                    {
                        LessonType = individual,
                        School = school,
                        MaxSpots = 1,
                        BeginOfLesson = new DateTime(2025, 4, 23, 12, 0, 0),
                        EndOfLesson = new DateTime(2025, 4, 23, 12, 50, 0)
                    }
                };

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
