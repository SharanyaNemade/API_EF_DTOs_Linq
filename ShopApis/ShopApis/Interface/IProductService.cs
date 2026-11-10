using ShopApis.DTOs;
using ShopApis.Models;

namespace ShopApis.Interface
{
    public interface IProductService
    {
        Task<List<ProductResponseDto>> GetAllAsync(int? categoryId = null);

        Task<ProductResponseDto?> GetByIdAsync(int id);
        Task<bool> NameExistsAsync(string name, int? excludeId = null);
        Task<ProductResponseDto> AddAsync(ProductCreateDto dto);
        Task<ProductResponseDto?> UpdateAsync(int id, ProductUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
