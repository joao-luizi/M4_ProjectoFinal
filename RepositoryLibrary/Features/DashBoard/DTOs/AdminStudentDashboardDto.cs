using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.DashBoard.DTOs
{
    public class AdminStudentDashboardDto
    {
        public List<StudentDashboardItemDto> Students { get; set; } = new();

        public int TotalStudents { get; set; }
        public int ScheduledClasses { get; set; }
        public int TotalHorses { get; set; }
        public int MissingPayments { get; set; }
    }
}
