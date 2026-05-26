using RepositoryLibrary.Features.Horses.Entities;

namespace RepositoryLibrary.Features.Lessons.Entities
{
    public class LessonHorse
    {
        public int HorseId { get; set; }
        public Horse Horse { get; set; }
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
    }
}
