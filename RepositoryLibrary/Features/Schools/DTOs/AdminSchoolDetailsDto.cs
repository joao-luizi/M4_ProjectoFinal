using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Schools.DTOs
{
    public class AdminSchoolDetailsDto
    {
        public int SchoolId { get; set; } 

        public string SchoolName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public int CAE { get; set; }
        public int SchoolCapacity { get; set; }

        public IBrowserFile? NewPhoto { get; set; }

        public string? ExistingPhotoPath { get; set; }
    }
}
