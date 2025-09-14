using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stokTakip.Models
{
    public class StockMovement
    {
        [Key]
        public int StockMovementId { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        public StockMovementType MovementType { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? UnitPrice { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [StringLength(100)]
        public string? ReferenceNumber { get; set; }
        
        public DateTime MovementDate { get; set; } = DateTime.UtcNow;
        
        [StringLength(100)]
        public string? CreatedBy { get; set; }
        
        // Navigation properties
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
    
    public enum StockMovementType
    {
        In = 1,     // Giriş
        Out = 2,    // Çıkış
        Transfer = 3, // Transfer
        Adjustment = 4 // Düzeltme
    }
}
