using System.ComponentModel.DataAnnotations;

namespace stokTakip.DTOs
{
    public class SupplierDto
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? TaxNumber { get; set; }
        public string? TaxOffice { get; set; }
        public string? ContactPerson { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal CurrentBalance { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class CreateSupplierDto
    {
        [Required]
        [StringLength(100)]
        public string SupplierName { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? CompanyName { get; set; }
        
        [StringLength(20)]
        public string? PhoneNumber { get; set; }
        
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }
        
        [StringLength(500)]
        public string? Address { get; set; }
        
        [StringLength(50)]
        public string? TaxNumber { get; set; }
        
        [StringLength(50)]
        public string? TaxOffice { get; set; }
        
        [StringLength(100)]
        public string? ContactPerson { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Credit limit must be greater than or equal to 0")]
        public decimal CreditLimit { get; set; } = 0;
    }

    public class UpdateSupplierDto
    {
        [Required]
        [StringLength(100)]
        public string SupplierName { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? CompanyName { get; set; }
        
        [StringLength(20)]
        public string? PhoneNumber { get; set; }
        
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }
        
        [StringLength(500)]
        public string? Address { get; set; }
        
        [StringLength(50)]
        public string? TaxNumber { get; set; }
        
        [StringLength(50)]
        public string? TaxOffice { get; set; }
        
        [StringLength(100)]
        public string? ContactPerson { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Credit limit must be greater than or equal to 0")]
        public decimal CreditLimit { get; set; }
        
        public bool IsActive { get; set; }
    }
}
