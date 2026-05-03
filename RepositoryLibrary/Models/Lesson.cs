namespace RepositoryLibrary.Models
{
    public class Lesson
    {
        public int LessonId { get; set; }
        public LessonType LessonType { get; set; }
        public School School { get; set; }
        // changed to maxSpots for readability
        public int MaxSpots { get; set; }
        public DateTime BeginOfLesson { get; set; }
        public DateTime EndOfLesson { get; set; }

        public ICollection<Booking> Bookings { get; set; }
        public ICollection<LessonProf> LessonProfs { get; set; }
        public ICollection<LessonHorse> LessonHorses { get; set; }
    }
}
