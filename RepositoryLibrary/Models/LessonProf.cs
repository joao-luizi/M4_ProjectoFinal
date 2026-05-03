using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLibrary.Models
{
    public class LessonProf
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string UserId { get; set; }
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
    }
}
