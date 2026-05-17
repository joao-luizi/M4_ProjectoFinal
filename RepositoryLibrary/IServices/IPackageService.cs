using RepositoryLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.IServices
{
    public interface IPackageService
    {
        public Task<List<Package>> GetAllPackages();
    }
}
