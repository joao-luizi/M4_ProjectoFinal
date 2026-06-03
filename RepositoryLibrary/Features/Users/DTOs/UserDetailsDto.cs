using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Users.DTOs
{
    public class UserDetailsDto
    {
        public string Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public DateTime Birthdate { get; set; }

        public int TaxIdentificationNumber { get; set; }
        public long SocialHealthNumber { get; set; }
        public int CitizenNumber { get; set; }

        public bool IsActive { get; set; }

        public string? PhotoPath { get; set; }

        public IList<string> Roles { get; set; } = [];
    }
}
