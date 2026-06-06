using RepositoryLibrary.Features.DashBoard.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.DashBoard.Interfaces
{
    public interface IStudentDashboardService
    {
        //V2 Implemented
        Task<StudentDashboardDto> GetStudentDashboardAsync(string studentId);
    }
}
