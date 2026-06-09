using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Lessons.DTOs
{
    public class UpdateLessonDto
    {
        public int LessonId { get; set; }

        public DateTime BeginOfLesson { get; set; }
        public DateTime EndOfLesson { get; set; }

        public int MaxSpots { get; set; }
    }
}
