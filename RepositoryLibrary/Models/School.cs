using Microsoft.EntityFrameworkCore;

namespace RepositoryLibrary.Models
{
    public class School
    {
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public int CAE { get; set; }
        public int SchoolCapacity { get; set; }
        public Logo Logo { get; set; }

        public ICollection<SchoolUser> SchoolUsers { get; set; }
    }
}
