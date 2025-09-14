using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stokTakip.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string? ProductCode { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        
        [Required]
        public int StockQuantity { get; set; }
        
        [Required]
        public int MinStockLevel { get; set; }
        
        [StringLength(50)]
        public string? Unit { get; set; } // Adet, Kg, Litre, etc.
        
        [StringLength(50)]
        public string? Category { get; set; }
        
        [StringLength(100)]
        public string? Brand { get; set; }
        
        [StringLength(200)]
        public string? ImageUrl { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedDate { get; set; }
        
        // Navigation properties
        public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
        public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    }
}
