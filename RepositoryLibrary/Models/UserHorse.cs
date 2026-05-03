using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLibrary.Models
{
    public class UserHorse
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string UserId { get; set; }
        public int HorseId { get; set; }
        public Horse Horse { get; set; }
        public string Relationship { get; set; }
    }
}
