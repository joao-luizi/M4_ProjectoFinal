using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLibrary.Models
{
    public class Photo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string UserId { get; set; }
        public byte[] UserPhoto { get; set; }
    }
}
