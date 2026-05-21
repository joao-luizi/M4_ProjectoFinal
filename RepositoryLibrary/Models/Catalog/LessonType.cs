namespace RepositoryLibrary.Models
{
    public class LessonType
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int DurationInMinutes { get; set; }
    }
}
