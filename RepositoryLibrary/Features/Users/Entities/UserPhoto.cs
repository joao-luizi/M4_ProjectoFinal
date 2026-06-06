using RepositoryLibrary.Features.Horses.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLibrary.Features.Users.Entities
{
   
    public class UserPhoto
    {
        [Key]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = string.Empty;

        public string? FotoPath { get; set; }

        public EMUser User { get; set; } = null!;
    }

}
