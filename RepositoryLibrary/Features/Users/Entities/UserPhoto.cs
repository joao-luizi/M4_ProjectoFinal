using RepositoryLibrary.Features.Horses.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLibrary.Features.Users.Entities
{

    public class UserFoto
    {
        public string UserId { get; set; }

        public string FotoPath { get; set; } = string.Empty;
    }

}
