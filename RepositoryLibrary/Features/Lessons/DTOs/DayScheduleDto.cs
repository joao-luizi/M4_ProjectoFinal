using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Lessons.DTOs
{
    public class DayScheduleDto
    {
        public DateOnly Date { get; set; }
        public List<LessonCalendarDto> Lessons { get; set; } = new();

        public int TotalLessons => Lessons.Count;
        public int TotalFreeSpots => Lessons.Sum(l => l.FreeSpots);
    }
}
