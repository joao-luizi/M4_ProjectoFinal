using RepositoryLibrary.Features.Bookings.Entities;
using RepositoryLibrary.Features.Products.Entities;
using RepositoryLibrary.Features.Schools.Entities;

namespace RepositoryLibrary.Features.Lessons.Entities
{
    public class Lesson
    {
        public int LessonId { get; set; }

        public int LessonTypeId { get; set; }
        public LessonType LessonType { get; set; }

        public int SchoolId { get; set; }
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
