using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.DashBoard.DTOs
{
    public class TeacherStudentDashboardItemDto
    {
        public string Name { get; set; } = string.Empty;

        public int ClassesRemaining { get; set; }

        public DateTime? LastClass { get; set; }
    }
}
