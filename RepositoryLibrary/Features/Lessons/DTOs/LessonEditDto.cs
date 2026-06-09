using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Lessons.DTOs
{
    public class LessonEditDto
    {
        public int LessonId { get; set; }
        public int SchoolId { get; set; }
        public int LessonTypeId { get; set; }

        public DateTime BeginOfLesson { get; set; }
        public DateTime EndOfLesson { get; set; }

        public int MaxSpots { get; set; }

        public List<string>? TeacherIds { get; set; }
        public List<int>? HorseIds { get; set; }
    }
}
