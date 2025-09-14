using System.ComponentModel.DataAnnotations;
using stokTakip.Models;

namespace stokTakip.DTOs
{
    public class StockMovementDto
    {
        public int StockMovementId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public StockMovementType MovementType { get; set; }
        public string MovementTypeName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? Description { get; set; }
        public string? ReferenceNumber { get; set; }
        public DateTime MovementDate { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class CreateStockMovementDto
    {
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        public StockMovementType MovementType { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Unit price must be greater than or equal to 0")]
        public decimal? UnitPrice { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [StringLength(100)]
        public string? ReferenceNumber { get; set; }
        
        [StringLength(100)]
        public string? CreatedBy { get; set; }
    }

}
