using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLibrary.Features.Horses.Entities
{
    public class HorseFoto
    {
        [Key]
        [ForeignKey(nameof(Horse))]
        public int HorseId { get; set; }

        public string? FotoPath { get; set; }

        public Horse Horse { get; set; } = null!;

    }
}
