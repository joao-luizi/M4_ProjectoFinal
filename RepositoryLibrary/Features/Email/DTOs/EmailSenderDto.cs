using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Email.DTOs
{
    public class EmailSenderDto
    {
        public List<string> UserIds { get; set; } = new();

        public string Subject { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;
    }
}
