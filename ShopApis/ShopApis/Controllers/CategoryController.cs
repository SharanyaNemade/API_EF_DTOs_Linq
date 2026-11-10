using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopApis.Interface;
using ShopApis.Service;

namespace ShopApis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        ICategoryService service;

        public CategoryController(ICategoryService service)
        {
            this.service = service;
        }


        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.GetAllAsync();
            return Ok(ApiResponse<List<CategoryResponseDto>>
                .SuccessResponse(data, "Categories fetched successfully", data.Count));
        }

        [HttpGet("GetCategoryById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await service.GetByIdAsync(id);
            if (data == null)
                return NotFound(ApiResponse<object>.Fail(
                    "Category not found", "NOT_FOUND", $"Category with id {id} does not exist"));

            return Ok(ApiResponse<CategoryResponseDto>
                .SuccessResponse(data, "Category fetched successfully", 1));
        }


        // GET api/categories/exists?name=Electronics
        // GET api/categories/exists?name=Electronics&excludeId=1
        [HttpGet("NameExistsAsync")]
        public async Task<IActionResult> NameExists([FromQuery] string name, [FromQuery] int? excludeId)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(ApiResponse<object>.Fail(
                    "Validation failed", "VALIDATION_ERROR", "Name is required"));

            var exists = await service.NameExistsAsync(name.Trim(), excludeId);

            var message = exists ? "Category is available" : "Category is unavailable";
            return Ok(ApiResponse<bool>.SuccessResponse(exists, message));
        }


        [HttpPost("AddCategory")]
        public async Task<IActionResult> Add(CategoryCreateDto dto)
        {
            try
            {
                var created = await service.AddAsync(dto);
                var response = ApiResponse<CategoryResponseDto>
                    .SuccessResponse(created, "Category created successfully", 1);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse<object>.Fail("Category already exists", "DUPLICATE_RECORD", ex.Message));
            }
        }

        [HttpPut("UpdateCategory/{id}")]
        public async Task<IActionResult> Update(int id, CategoryUpdateDto dto)
        {
            try
            {
                var updated = await service.UpdateAsync(id, dto);
                if (updated == null)
                    return NotFound(ApiResponse<object>.Fail(
                        "Category not found", "NOT_FOUND", $"Category with id {id} does not exist"));

                return Ok(ApiResponse<CategoryResponseDto>
                    .SuccessResponse(updated, "Category updated successfully", 1));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse<object>.Fail("Category already exists", "DUPLICATE_RECORD", ex.Message));
            }
        }

        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await service.DeleteAsync(id);
                if (!deleted)
                    return NotFound(ApiResponse<object>.Fail(
                        "Category not found", "NOT_FOUND", $"Category with id {id} does not exist"));

                // 200 with a body (instead of 204) so the response keeps the standard format
                return Ok(ApiResponse<object?>.SuccessResponse(null, "Category deleted successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse<object>.Fail("Cannot delete category", "CATEGORY_HAS_PRODUCTS", ex.Message));
            }
        }
    }
}
