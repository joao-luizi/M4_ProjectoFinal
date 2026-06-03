using RepositoryLibrary.Features.Schools.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLibrary.Features.Users.Entities
{
    public class SchoolUser
    {
        public string UserId { get; set; }
        public int SchoolId { get; set; }

        public EMUser User { get; set; }
        public School School { get; set; }
    }
}
