using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.DashBoard.DTOs
{
    public class StudentDashboardItemDto
    {
        public string Name { get; set; }
        public string PaymentStatus { get; set; }
        public int ClassesRemaining { get; set; }
        public DateTime? LastClass { get; set; }
    }
}
