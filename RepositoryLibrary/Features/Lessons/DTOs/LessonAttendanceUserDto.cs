using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Lessons.DTOs
{
    public class LessonAttendanceUserDto
    {
        public string UserId { get; set; }

        public string Name { get; set; }

        public bool IsBooked { get; set; }

        public bool WasPresent { get; set; }
    }
}
