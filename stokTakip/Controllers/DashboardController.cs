using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using stokTakip.Data;
using stokTakip.DTOs;

namespace stokTakip.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly StokTakipDbContext _context;

        public DashboardController(StokTakipDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            var totalProducts = await _context.Products.CountAsync(p => p.IsActive);
            var lowStockProducts = await _context.Products.CountAsync(p => p.IsActive && p.StockQuantity <= p.MinStockLevel);
            var totalCustomers = await _context.Customers.CountAsync(c => c.IsActive);
            var totalSuppliers = await _context.Suppliers.CountAsync(s => s.IsActive);

            var today = DateTime.UtcNow.Date;
            var thisMonth = new DateTime(today.Year, today.Month, 1);

            var todaySales = await _context.Sales
                .Where(s => s.SaleDate.Date == today)
                .SumAsync(s => s.TotalAmount);

            var thisMonthSales = await _context.Sales
                .Where(s => s.SaleDate >= thisMonth)
                .SumAsync(s => s.TotalAmount);

            var todayPurchases = await _context.Purchases
                .Where(p => p.PurchaseDate.Date == today)
                .SumAsync(p => p.TotalAmount);

            var thisMonthPurchases = await _context.Purchases
                .Where(p => p.PurchaseDate >= thisMonth)
                .SumAsync(p => p.TotalAmount);

            var stats = new DashboardStatsDto
            {
                TotalProducts = totalProducts,
                LowStockProducts = lowStockProducts,
                TotalCustomers = totalCustomers,
                TotalSuppliers = totalSuppliers,
                TodaySales = todaySales,
                ThisMonthSales = thisMonthSales,
                TodayPurchases = todayPurchases,
                ThisMonthPurchases = thisMonthPurchases,
                NetProfit = thisMonthSales - thisMonthPurchases
            };

            return Ok(stats);
        }

        [HttpGet("recent-sales")]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetRecentSales([FromQuery] int count = 10)
        {
            var sales = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .OrderByDescending(s => s.SaleDate)
                .Take(count)
                .ToListAsync();

            // Map to DTOs manually since we don't have AutoMapper in this controller
            var salesDto = sales.Select(s => new SaleDto
            {
                SaleId = s.SaleId,
                CustomerId = s.CustomerId,
                CustomerName = s.Customer.CustomerName,
                SaleNumber = s.SaleNumber,
                SaleDate = s.SaleDate,
                SubTotal = s.SubTotal,
                TaxAmount = s.TaxAmount,
                DiscountAmount = s.DiscountAmount,
                TotalAmount = s.TotalAmount,
                Notes = s.Notes,
                PaymentMethod = s.PaymentMethod,
                IsPaid = s.IsPaid,
                PaymentDate = s.PaymentDate,
                CreatedBy = s.CreatedBy,
                CreatedDate = s.CreatedDate,
                SaleItems = s.SaleItems.Select(si => new SaleItemDto
                {
                    SaleItemId = si.SaleItemId,
                    SaleId = si.SaleId,
                    ProductId = si.ProductId,
                    ProductName = si.Product.ProductName,
                    Quantity = si.Quantity,
                    UnitPrice = si.UnitPrice,
                    DiscountAmount = si.DiscountAmount,
                    TaxRate = si.TaxRate,
                    TotalAmount = si.TotalAmount
                }).ToList()
            });

            return Ok(salesDto);
        }

        [HttpGet("top-products")]
        public async Task<ActionResult<IEnumerable<TopProductDto>>> GetTopProducts([FromQuery] int count = 10)
        {
            var topProducts = await _context.SaleItems
                .Include(si => si.Product)
                .GroupBy(si => new { si.ProductId, si.Product.ProductName })
                .Select(g => new TopProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    TotalQuantitySold = g.Sum(si => si.Quantity),
                    TotalRevenue = g.Sum(si => si.TotalAmount)
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .Take(count)
                .ToListAsync();

            return Ok(topProducts);
        }
    }

    public class DashboardStatsDto
    {
        public int TotalProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalSuppliers { get; set; }
        public decimal TodaySales { get; set; }
        public decimal ThisMonthSales { get; set; }
        public decimal TodayPurchases { get; set; }
        public decimal ThisMonthPurchases { get; set; }
        public decimal NetProfit { get; set; }
    }

    public class TopProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
