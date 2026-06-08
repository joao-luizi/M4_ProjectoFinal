using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Horses.DTOs
{
    public class HorseEditDto
    {
        public int HorseId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Breed { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string? PhotoPath { get; set; }

        public IBrowserFile? NewPhoto { get; set; }

        public int SchoolId { get; set; }

        public string? OwnerUserId { get; set; }
    }
}
