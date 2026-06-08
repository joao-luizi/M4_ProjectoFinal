using RepositoryLibrary.Features.Lessons.Entities;
using RepositoryLibrary.Features.Schools.Entities;
using System.ComponentModel.DataAnnotations;

namespace RepositoryLibrary.Features.Horses.Entities
{
    public class Horse
    {
        public int HorseId { get; set; }

        public int SchoolId { get; set; }

        public School School { get; set; }

        public string Name { get; set; }

        public string Breed { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public HorseFoto? HorseFoto { get; set; }

        public ICollection<LessonHorse> LessonHorses { get; set; }

        public ICollection<UserHorse> UserHorses { get; set; }
    }
}
