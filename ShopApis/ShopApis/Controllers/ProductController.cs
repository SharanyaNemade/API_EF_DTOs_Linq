using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApis.DTOs;
using ShopApis.Interface;
using ShopApis.Service;

namespace ShopApis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        IProductService service;

        public ProductController(IProductService service)
        {
            this.service = service;
        }


        // GET api/products  or  api/products?categoryId=1
        [HttpGet("GetAllProdcuts")]
        public async Task<IActionResult> GetAll([FromQuery] int? categoryId)
        {
            var data = await service.GetAllAsync(categoryId);
            return Ok(ApiResponse<List<ProductResponseDto>>
                .SuccessResponse(data, "Products fetched successfully", data.Count));
        }



        [HttpGet("GetProductById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await service.GetByIdAsync(id);
            if (data == null)
                return NotFound(ApiResponse<object>.Fail(
                    "Product not found", "NOT_FOUND", $"Product with id {id} does not exist"));

            return Ok(ApiResponse<ProductResponseDto>
                .SuccessResponse(data, "Product fetched successfully", 1));
        }



        // GET api/products/exists?name=Laptop
        // GET api/products/exists?name=Laptop&excludeId=1
        [HttpGet("ProductNameExists/exists")]
        public async Task<IActionResult> NameExists([FromQuery] string name, [FromQuery] int? excludeId)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(ApiResponse<object>.Fail(
                    "Validation failed", "VALIDATION_ERROR", "Name is required"));

            var exists = await service.NameExistsAsync(name.Trim(), excludeId);

            var message = exists ? "Product name already exists" : "Product name is available";
            return Ok(ApiResponse<bool>.SuccessResponse(exists, message));
        }



        [HttpPost("CreateProduct")]
        public async Task<IActionResult> Create(ProductCreateDto dto)
        {
            try
            {
                var created = await service.AddAsync(dto);
                var response = ApiResponse<ProductResponseDto>
                    .SuccessResponse(created, "Product created successfully", 1);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Invalid category", "CATEGORY_NOT_FOUND", ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse<object>.Fail(
                    "Product already exists", "DUPLICATE_RECORD", ex.Message));
            }
        }


        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
        {
            try
            {
                var updated = await service.UpdateAsync(id, dto);
                if (updated == null)
                    return NotFound(ApiResponse<object>.Fail(
                        "Product not found", "NOT_FOUND", $"Product with id {id} does not exist"));

                return Ok(ApiResponse<ProductResponseDto>
                    .SuccessResponse(updated, "Product updated successfully", 1));
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Invalid category", "CATEGORY_NOT_FOUND", ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse<object>.Fail(
                    "Product already exists", "DUPLICATE_RECORD", ex.Message));
            }
        }


        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await service.DeleteAsync(id);
                if (!deleted)
                    return NotFound(ApiResponse<object>.Fail(
                        "Product not found", "NOT_FOUND", $"Product with id {id} does not exist"));

                // 200 with a body (instead of 204) so the response keeps the standard format
                return Ok(ApiResponse<object?>.SuccessResponse(null, "Product deleted successfully"));
            }
            catch (DbUpdateException)
            {
                return Conflict(ApiResponse<object>.Fail(
                    "Cannot delete product", "PRODUCT_HAS_ORDERS", "Product is referenced by existing orders."));
            }
        }
    }
}
