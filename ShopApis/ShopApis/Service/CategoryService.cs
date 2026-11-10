using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShopApis.Data;
using ShopApis.Interface;
using ShopApis.Models;

namespace ShopApis.Service
{
    public class CategoryService : ICategoryService
    {

        ApplicationDbContext db;
        IMapper mapper;

        
        public CategoryService(ApplicationDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<CategoryResponseDto> AddAsync(CategoryCreateDto dto)
        {
            if (db.Categories.Any(c => c.Name == dto.Name))
                throw new InvalidOperationException($"Category '{dto.Name}' already exists.");

            var category = mapper.Map<Category>(dto);
            db.Categories.Add(category);
            await db.SaveChangesAsync();
            return mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await db.Categories.Include(c => c.Products)
                             .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return false;

            if (category.Products.Any())
                throw new InvalidOperationException("Cannot delete a category that still has products.");

            db.Categories.Remove(category);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<List<CategoryResponseDto>> GetAllAsync()
        {
            var data = await db.Categories.Include(c => c.Products).AsNoTracking().ToListAsync();
            return mapper.Map<List<CategoryResponseDto>>(data);
        }


        public async Task<CategoryResponseDto?> GetByIdAsync(int id)
        {
            var category = await db.Categories.Include(c => c.Products)
                             .FirstOrDefaultAsync(c => c.Id == id);
            return category == null ? null : mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            return await db.Categories.AnyAsync(c => c.Name == name && (excludeId == null || c.Id != excludeId));
        }

        public async Task<CategoryResponseDto?> UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var category = await db.Categories.Include(c => c.Products)
                             .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return null;

            if (db.Categories.Any(c => c.Name == dto.Name && c.Id != id))
                throw new InvalidOperationException($"Category '{dto.Name}' already exists.");

            mapper.Map(dto, category);   // copies DTO values onto the tracked entity
            await db.SaveChangesAsync();            // no Update() needed, EF tracks the change
            return mapper.Map<CategoryResponseDto>(category);
        }
    }
}
