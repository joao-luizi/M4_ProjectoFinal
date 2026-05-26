using RepositoryLibrary.Models;
using RepositoryLibrary.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.IRepository
{
 
    public interface ISchoolUsersRepository

    {
        public Task<List<SchoolUser>> GetAllUsers(int schoolId);
        public Task<SchoolUser> GetUserById(string id);

        public Task<int> DeleteUserAsync(string id);

    }
}
