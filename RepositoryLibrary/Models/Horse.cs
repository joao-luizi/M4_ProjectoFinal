namespace RepositoryLibrary.Models
{
    public class Horse
    {
        public int HorseId { get; set; }
        public School School { get; set; }
        public string Name { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }

        public ICollection<LessonHorse> LessonHorses { get; set; }
        public ICollection<UserHorse> UserHorses { get; set; }
    }
}
