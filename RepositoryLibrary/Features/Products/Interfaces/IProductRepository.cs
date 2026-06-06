using RepositoryLibrary.Features.Products.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Products.Interfaces
{
    public interface  IProductRepository
    {
        //V2 implemented
        Task<List<Product>> GetByIdsAsync(List<int> productIds);
    }
}
