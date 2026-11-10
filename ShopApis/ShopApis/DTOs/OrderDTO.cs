using System.ComponentModel.DataAnnotations;

namespace ShopApis.DTOs
{
    public class OrderCreateDto
    {

        [Required, MaxLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public int ProductId { get; set; }

        [Range(1, 10000)]
        public int Quantity { get; set; }
    }


    public class OrderUpdateDto
    {

        [Required, MaxLength(50)]
        public string Status { get; set; } = string.Empty;
    }


    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
