using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Users.DTOs
{
    public class AdminUserDetailsDto
    {
        public string Id { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }

        public DateTime Birthdate { get; set; }
        public string Address { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public string? Role { get; set; }

        public string? PhotoPath { get; set; }

        public IBrowserFile? NewPhoto { get; set; }

        public string? Password { get; set; }
    }
}
