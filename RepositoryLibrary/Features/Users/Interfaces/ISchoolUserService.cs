using RepositoryLibrary.Features.Users.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Users.Interfaces
{
    public  interface ISchoolUserService
    {
        Task AddUserToSchoolAsync(string userId, int schoolId);

        Task RemoveUserFromSchoolAsync(string userId, int schoolId);

        Task<List<UserListDto>> GetSchoolUsersAsync(int schoolId);

        Task<List<UserSchoolDto>> GetUserSchoolsAsync(string userId);
    }
}
