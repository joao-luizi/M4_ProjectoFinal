using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Lessons.DTOs
{
    public class LessonAttendanceDto
    {
        public int LessonId { get; set; }

        public List<LessonAttendanceUserDto> Students { get; set; } = new();
    }
}
