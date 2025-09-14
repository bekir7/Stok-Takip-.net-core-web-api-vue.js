using System.ComponentModel.DataAnnotations;

namespace stokTakip.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductCode { get; set; }
        public string? Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }
        public int MinStockLevel { get; set; }
        public string? Unit { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class CreateProductDto
    {
        [Required]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string? ProductCode { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Unit price must be greater than or equal to 0")]
        public decimal UnitPrice { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be greater than or equal to 0")]
        public int StockQuantity { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Minimum stock level must be greater than or equal to 0")]
        public int MinStockLevel { get; set; }
        
        [StringLength(50)]
        public string? Unit { get; set; }
        
        [StringLength(50)]
        public string? Category { get; set; }
        
        [StringLength(100)]
        public string? Brand { get; set; }
        
        [StringLength(200)]
        public string? ImageUrl { get; set; }
    }

    public class UpdateProductDto
    {
        [Required]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string? ProductCode { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Unit price must be greater than or equal to 0")]
        public decimal UnitPrice { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be greater than or equal to 0")]
        public int StockQuantity { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Minimum stock level must be greater than or equal to 0")]
        public int MinStockLevel { get; set; }
        
        [StringLength(50)]
        public string? Unit { get; set; }
        
        [StringLength(50)]
        public string? Category { get; set; }
        
        [StringLength(100)]
        public string? Brand { get; set; }
        
        [StringLength(200)]
        public string? ImageUrl { get; set; }
        
        public bool IsActive { get; set; }
    }
}
