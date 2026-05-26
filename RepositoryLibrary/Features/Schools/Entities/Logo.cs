using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLibrary.Features.Schools.Entities
{
    public class Logo
    {
        public int SchoolId { get; set; }
        public string LogoName { get; set; }
        public byte[] Image { get; set; }

        public School School { get; set; }
    }
}
