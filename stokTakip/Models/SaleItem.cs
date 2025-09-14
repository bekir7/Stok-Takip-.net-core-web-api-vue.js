using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stokTakip.Models
{
    public class SaleItem
    {
        [Key]
        public int SaleItemId { get; set; }
        
        [Required]
        public int SaleId { get; set; }
        
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
        [ForeignKey("SaleId")]
        public virtual Sale Sale { get; set; } = null!;
        
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}
