using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Lessons.DTOs
{
    public class LessonCalendarDto
    {
        public int LessonId { get; set; }

        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public string LessonType { get; set; }

        public DateTime Begin { get; set; }
        public DateTime End { get; set; }

        public int MaxSpots { get; set; }
        public int BookedSpots { get; set; }

        public int FreeSpots => MaxSpots - BookedSpots;

        public List<string> Teachers { get; set; } = new();
        public List<string> Horses { get; set; } = new();
    }
}
