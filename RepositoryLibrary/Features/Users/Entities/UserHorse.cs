using RepositoryLibrary.Features.Horses.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLibrary.Features.Users.Entities
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
