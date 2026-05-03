namespace RepositoryLibrary.Models
{
    public class LessonHorse
    {
        public int HorseId { get; set; }
        public Horse Horse { get; set; }
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
    }
}
