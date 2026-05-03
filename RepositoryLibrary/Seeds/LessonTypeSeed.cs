using RepositoryLibrary.Models.Context;
using RepositoryLibrary.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;

namespace RepositoryLibrary.Seeds
{
    public static class LessonTypeSeed
    {
        public static async Task SeedLessonTypeAsync(IServiceProvider serviceProvider)
        {
            try
            {
                var em_Context = serviceProvider.GetRequiredService<EM_DbContext>();

                var lessonTypes = em_Context.LessonTypes;

                if (lessonTypes.IsNullOrEmpty())
                {
                    List<LessonType> types = new List<LessonType>
                    {
                        new LessonType() { Name = "Volteio", DurationInMinutes = 30 },
                        new LessonType() { Name = "Sela 1", DurationInMinutes = 40 },
                        new LessonType() { Name = "Sela 2", DurationInMinutes = 50 },
                        new LessonType() { Name = "Individual", DurationInMinutes = 50 },
                        new LessonType() { Name = "Passeio", DurationInMinutes = 45}
                    };

                    await em_Context.LessonTypes.AddRangeAsync(types);
                    em_Context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
    }
}
