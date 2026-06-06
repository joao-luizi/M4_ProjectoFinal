using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Users.DTOs
{
    public class AdminUsersDto
    {
        public List<AdminUserListItemDto> Users { get; set; } = [];

        public int TotalUsers { get; set; }

        public int ActiveUsers { get; set; }

        public int InactiveUsers { get; set; }
    }
}
