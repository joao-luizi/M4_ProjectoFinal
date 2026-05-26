using RepositoryLibrary.Features.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Schools.Interfaces
{
 
    public interface ISchoolUsersRepository

    {
        public Task<List<SchoolUser>> GetAllUsers(int schoolId);
        public Task<SchoolUser> GetUserById(string id);

        public Task<int> DeleteUserAsync(string id);

    }
}
