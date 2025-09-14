using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stokTakip.Models
{
    public class PurchaseItem
    {
        [Key]
        public int PurchaseItemId { get; set; }
        
        [Required]
        public int PurchaseId { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxRate { get; set; } = 0;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        
        // Navigation properties
        [ForeignKey("PurchaseId")]
        public virtual Purchase Purchase { get; set; } = null!;
        
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}
