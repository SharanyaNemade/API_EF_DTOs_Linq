using System.ComponentModel.DataAnnotations;

namespace ShopApis.DTOs
{
    public class ProductCreateDto
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, 100000)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock {  get; set; }

        [Required]
        public int CategoryId { get; set; }
    }


    public class ProductUpdateDto : ProductCreateDto { }

    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
