using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLibrary.Features.Lessons.Entities
{
    public class LessonProf
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string UserId { get; set; }
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
    }
}
