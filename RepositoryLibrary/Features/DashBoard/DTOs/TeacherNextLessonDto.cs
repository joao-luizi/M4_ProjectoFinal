using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.DashBoard.DTOs
{
    public class TeacherNextLessonDto
    {
        public DateTime LessonDate { get; set; }

        public List<string> StudentNames { get; set; } = new();
    }
}
