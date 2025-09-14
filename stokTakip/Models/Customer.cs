using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stokTakip.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? CompanyName { get; set; }
        
        [StringLength(20)]
        public string? PhoneNumber { get; set; }
        
        [StringLength(100)]
        public string? Email { get; set; }
        
        [StringLength(500)]
        public string? Address { get; set; }
        
        [StringLength(50)]
        public string? TaxNumber { get; set; }
        
        [StringLength(50)]
        public string? TaxOffice { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal CreditLimit { get; set; } = 0;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentBalance { get; set; } = 0;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedDate { get; set; }
        
        // Navigation properties
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
