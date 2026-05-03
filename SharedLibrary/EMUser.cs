using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SharedLibrary
{
    public class EMUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; } = true;
        public bool InformationAuthorized { get; set; }
        public bool ImageAuthorized { get; set; }
        public DateTime RegisterDate { get; set; }
        public int TaxIdentificationNumber { get; set; }
        public long SocialHealthNumber { get; set; }
        public int CitizenNumber { get; set; }
        public DateTime Birthdate { get; set; }
        public string Address { get; set; }
    }
}
