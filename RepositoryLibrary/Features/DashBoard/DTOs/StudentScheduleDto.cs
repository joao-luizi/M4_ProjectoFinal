using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.DashBoard.DTOs
{
    public class StudentScheduleDto
    {
        public TimeSpan OpenTime { get; set; }

        public TimeSpan CloseTime { get; set; }

        public List<DayOfWeek> WorkingDays { get; set; } = new();
    }
}
