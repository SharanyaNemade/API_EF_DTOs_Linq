using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShopApis.Data;
using ShopApis.DTOs;
using ShopApis.Interface;
using ShopApis.Models;

namespace ShopApis.Service
{
    public class ProductService : IProductService
    {
        ApplicationDbContext db;
        IMapper mapper;

        public ProductService(ApplicationDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<List<ProductResponseDto>> GetAllAsync(int? categoryId = null)
        {
            var query = db.Products.Include(p => p.Category).AsNoTracking();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            var data = await query.ToListAsync();
            return mapper.Map<List<ProductResponseDto>>(data);
        }

        public async Task<ProductResponseDto?> GetByIdAsync(int id)
        {
            var product = await db.Products.Include(p => p.Category)
                                  .FirstOrDefaultAsync(p => p.Id == id);
            return product == null ? null : mapper.Map<ProductResponseDto>(product);
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            return await db.Products.AnyAsync(p => p.Name == name &&
                                                   (excludeId == null || p.Id != excludeId));
        }

        public async Task<ProductResponseDto> AddAsync(ProductCreateDto dto)
        {
            if (!await db.Categories.AnyAsync(c => c.Id == dto.CategoryId))
                throw new KeyNotFoundException($"Category {dto.CategoryId} not found.");

            if (await NameExistsAsync(dto.Name))
                throw new InvalidOperationException($"Product '{dto.Name}' already exists.");

            var product = mapper.Map<Product>(dto);
            db.Products.Add(product);
            await db.SaveChangesAsync();

            // load the category so CategoryName is filled in the response
            await db.Entry(product).Reference(p => p.Category).LoadAsync();
            return mapper.Map<ProductResponseDto>(product);
        }

        public async Task<ProductResponseDto?> UpdateAsync(int id, ProductUpdateDto dto)
        {
            var product = await db.Products.Include(p => p.Category)
                                  .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return null;

            if (!await db.Categories.AnyAsync(c => c.Id == dto.CategoryId))
                throw new KeyNotFoundException($"Category {dto.CategoryId} not found.");

            if (await NameExistsAsync(dto.Name, id))
                throw new InvalidOperationException($"Product '{dto.Name}' already exists.");

            mapper.Map(dto, product);        // copies DTO values onto the tracked product
            await db.SaveChangesAsync();     // no Update() needed, EF tracks the change

            // refresh the category in case CategoryId was changed
            await db.Entry(product).Reference(p => p.Category).LoadAsync();
            return mapper.Map<ProductResponseDto>(product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return false;

            db.Products.Remove(product);
            await db.SaveChangesAsync();     // throws DbUpdateException if orders reference it
            return true;
        }
    }
}
