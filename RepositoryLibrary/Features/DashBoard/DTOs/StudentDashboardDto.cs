using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.DashBoard.DTOs
{
    public class StudentDashboardDto
    {
        public StudentDashboardSummaryDto Summary { get; set; } = new();

        public StudentPaymentStatusDto Payment { get; set; } = new();

        public StudentScheduleDto Schedule { get; set; } = new();

        public List<StudentUpcomingClassDto> UpcomingClasses { get; set; } = new();
    }
}
