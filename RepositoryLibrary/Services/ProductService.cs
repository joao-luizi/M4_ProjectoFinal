using Microsoft.EntityFrameworkCore;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using RepositoryLibrary.Services;


namespace RepositoryLibrary.Services
{

    public class ProductService : IProductService
    {
        private readonly EM_DbContext _db;

        public ProductService(EM_DbContext db)
        {
            _db = db;
        }

        public async Task<List<Product>> GetAll()
        {
            return await _db.Products
                .Include(p => p.Entitlements)
                    .ThenInclude(e => e.LessonType)
                .ToListAsync();
        }

        public async Task<Product?> GetById(int id)
            => await _db.Products
                .Include(p => p.Entitlements)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task Create(Product product)
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
        }


    }
}
