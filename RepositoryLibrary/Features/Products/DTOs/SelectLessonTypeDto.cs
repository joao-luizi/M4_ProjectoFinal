using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Products.DTOs
{
    public class SelectLessonTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int Duration { get; set; }
    }
}
