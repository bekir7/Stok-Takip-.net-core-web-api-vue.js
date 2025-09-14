using System.ComponentModel.DataAnnotations;

namespace stokTakip.DTOs
{
    public class PurchaseDto
    {
        public int PurchaseId { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string PurchaseNumber { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public string? PaymentMethod { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<PurchaseItemDto> PurchaseItems { get; set; } = new List<PurchaseItemDto>();
    }

    public class PurchaseItemDto
    {
        public int PurchaseItemId { get; set; }
        public int PurchaseId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class CreatePurchaseDto
    {
        [Required]
        public int SupplierId { get; set; }
        
        [Required]
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [StringLength(50)]
        public string? PaymentMethod { get; set; }
        
        public bool IsPaid { get; set; } = false;
        
        [StringLength(100)]
        public string? CreatedBy { get; set; }
        
        [Required]
        [MinLength(1, ErrorMessage = "At least one purchase item is required")]
        public List<CreatePurchaseItemDto> PurchaseItems { get; set; } = new List<CreatePurchaseItemDto>();
    }

    public class CreatePurchaseItemDto
    {
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Unit price must be greater than or equal to 0")]
        public decimal UnitPrice { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Discount amount must be greater than or equal to 0")]
        public decimal DiscountAmount { get; set; } = 0;
        
        [Range(0, 100, ErrorMessage = "Tax rate must be between 0 and 100")]
        public decimal TaxRate { get; set; } = 0;
    }

    public class UpdatePurchaseDto
    {
        [Required]
        public int SupplierId { get; set; }
        
        [Required]
        public DateTime PurchaseDate { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [StringLength(50)]
        public string? PaymentMethod { get; set; }
        
        public bool IsPaid { get; set; }
        
        public DateTime? PaymentDate { get; set; }
    }
}
