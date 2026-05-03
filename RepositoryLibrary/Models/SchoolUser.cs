using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLibrary.Models
{
    public class SchoolUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string UserId { get; set; }
        public int SchoolId { get; set; }
        public School School { get; set; }
    }
}
