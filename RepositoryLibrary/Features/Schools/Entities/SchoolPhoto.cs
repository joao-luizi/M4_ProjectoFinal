using RepositoryLibrary.Features.Horses.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLibrary.Features.Schools.Entities
{
    public class SchoolPhoto
    {
        [Key]
        [ForeignKey(nameof(School))]
        public int SchoolId { get; set; }

        public string? FotoPath { get; set; }

        public School School { get; set; }

    }
}
