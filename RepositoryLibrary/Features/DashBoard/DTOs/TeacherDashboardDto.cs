using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.DashBoard.DTOs
{
    public class TeacherDashboardDto
    {
        public int TotalStudents { get; set; }

        public int ScheduledClasses { get; set; }

        public int TotalHorses { get; set; }

        public TeacherNextLessonDto? NextLesson { get; set; }

        public List<TeacherStudentDashboardItemDto> Students { get; set; } = new();
    }
}
