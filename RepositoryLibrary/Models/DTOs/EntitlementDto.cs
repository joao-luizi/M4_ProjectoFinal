using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Models.DTOs
{
    public class EntitlementDto
    {
        public string LessonTypeName { get; set; } = "";
        public int WeeklyFrequency { get; set; }
    }
}
