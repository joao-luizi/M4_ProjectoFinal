using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLibrary.Features.Users
{
    public class Photo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string UserId { get; set; }
        public byte[] UserPhoto { get; set; }
    }
}
