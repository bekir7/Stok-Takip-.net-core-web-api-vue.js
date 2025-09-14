using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stokTakip.Models
{
    public class Purchase
    {
        [Key]
        public int PurchaseId { get; set; }
        
        [Required]
        public int SupplierId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string PurchaseNumber { get; set; } = string.Empty;
        
        [Required]
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [StringLength(50)]
        public string? PaymentMethod { get; set; } // Nakit, Kredi Kartı, Havale, etc.
        
        public bool IsPaid { get; set; } = false;
        
        public DateTime? PaymentDate { get; set; }
        
        [StringLength(100)]
        public string? CreatedBy { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; } = null!;
        
        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    }
}
