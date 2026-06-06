using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Users.DTOs
{
    //Esta class é uma projecção para evitar querys N + 1 no service
    public class UserAdminProjectionDto
    {
        public string Id { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        public string? Role { get; set; }

        public string? PhotoPath { get; set; }
    }
}
