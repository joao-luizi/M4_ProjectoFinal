using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLibrary.Models
{
    public class Logo
    {
        public int SchoolId { get; set; }
        public string LogoName { get; set; }
        public byte[] Image { get; set; }

        public School School { get; set; }
    }
}
