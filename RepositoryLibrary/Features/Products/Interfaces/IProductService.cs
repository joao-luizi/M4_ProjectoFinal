using RepositoryLibrary.Features.Products.Entities;

namespace RepositoryLibrary.Features.Products.Interfaces
{
    public interface IProductService
    {
        //V2 Its using
        Task<List<Product>> GetAll();
        //V2 Its using
        Task<Product?> GetById(int id);
        //V2 Its using
        Task Create(Product product);

       

    }
}
