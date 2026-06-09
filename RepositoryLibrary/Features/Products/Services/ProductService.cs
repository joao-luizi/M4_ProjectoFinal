using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Products.DTOs;
using RepositoryLibrary.Features.Products.Entities;
using RepositoryLibrary.Features.Products.Interfaces;
using RepositoryLibrary.Features.Users.DTOs;


namespace RepositoryLibrary.Features.Products.Services
{

    public class ProductService : IProductService
    {
        private readonly RideReadyDbContext _db;

        public ProductService(RideReadyDbContext db)
        {
            _db = db;
        }

        //V2 Its using
        public async Task<List<SelectLessonTypeDto>> GetAllSelectLessonTypeAsync()
        {
            var lessonTypes = await _db.LessonTypes.ToListAsync();
            return lessonTypes
             .Select(u => new SelectLessonTypeDto
             {
                 Id = u.Id,
                 Name = u.Name,
                 Duration = u.DurationInMinutes
             })
             .ToList();
        }

        //V2 Its using
        public async Task<List<Product>> GetAll()
        {
            return await _db.Products
                .Include(p => p.Entitlements)
                    .ThenInclude(e => e.LessonType)
                .ToListAsync();
        }
        //V2 Its using
        public async Task<Product?> GetById(int id)
            => await _db.Products
                .Include(p => p.Entitlements)
                .FirstOrDefaultAsync(p => p.Id == id);
        //V2 Its using
        public async Task Create(Product product)
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
        }


    }
}
