using RepositoryLibrary.IRepository;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace RepositoryLibrary.Repository
{
    public class PackageRepository : IPackageRepository
    {
        private readonly EM_DbContext _emContext;

        public PackageRepository(EM_DbContext em_Context)
        {
            _emContext = em_Context;
        }

        public async Task<Package> GetPackageByIdAsync(int id)
        {
            try
            {
                Package? package = await _emContext.Packages.FirstOrDefaultAsync(pkg => pkg.Id == id);

                if(package is null)
                {
                    throw new Exception("The package does not exist.");
                }

                return package;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<List<Package>> GetPackageByUserIdAsync(string userId)
        {
            try
            {
                // it returns a list, because a user can change packages from one month to the other.
                List<UserPayment> userPayments = await _emContext.UserPayments.Where(usr => usr.UserId == userId).ToListAsync();
                List<Package> packageList = [];

                if (userPayments.IsNullOrEmpty()) throw new Exception($"There's no user with the Id = {userId}.");

                foreach(UserPayment userPayment in userPayments)
                {
                    var package = await _emContext.Packages.FirstAsync();

                    if(!packageList.Contains(package))
                    {
                        packageList.Add(package);
                    }
                }

                return packageList;
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<Package> AddNewPackageAsync(Package package)
        {
            try
            {
                await _emContext.Packages.AddAsync(package);
                await _emContext.SaveChangesAsync();

                return package;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<Package> EditPackageAsync(Package package)
        {
            try
            {
                Package? packageToUpdate = await _emContext.Packages.FirstOrDefaultAsync(pckg => pckg.Id == package.Id);

                if(packageToUpdate is null)
                {
                    throw new Exception($"Could not find a package with the Id = {package.Id}");
                }

                packageToUpdate.LessonTypeId = package.LessonTypeId;
                packageToUpdate.Name = package.Name;
                packageToUpdate.Weekly = package.Weekly;
                packageToUpdate.ClassesIncluded = package.ClassesIncluded;
                packageToUpdate.Valor = package.Valor;

                await _emContext.SaveChangesAsync();

                return packageToUpdate;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<Package> DeletePackageByIdAsync(int packageId)
        {
            try
            {
                Package? package = await _emContext.Packages.FirstOrDefaultAsync(pckg => pckg.Id == packageId);

                if (package is null)
                {
                    throw new Exception($"Could not find a package with the Id = {packageId}");
                }

                _emContext.Remove(package);
                await _emContext.SaveChangesAsync();

                return package;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
    }
}
