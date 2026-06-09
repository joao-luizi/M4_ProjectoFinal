using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Lessons.DTOs
{
    public class MonthScheduleDto
    {
        public int Year { get; set; }
        public int Month { get; set; }

        public List<DayScheduleDto> Days { get; set; } = new();
    }
}
