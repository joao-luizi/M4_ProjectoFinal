using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.DashBoard.DTOs
{
    public class StudentUpcomingClassDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }

 
    }
}
