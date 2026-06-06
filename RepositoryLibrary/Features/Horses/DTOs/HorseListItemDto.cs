using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Horses.DTOs
{
    public class HorseListItemDto
    {
        public int HorseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }

        public string Ownership { get; set; } = string.Empty; // "School" | "Private"

        public int Age => (int)((DateTime.Today - DateOfBirth).TotalDays / 365.25);

        public string SchoolName { get; set; }

        public int SchoolId { get; set; }
    }
}
