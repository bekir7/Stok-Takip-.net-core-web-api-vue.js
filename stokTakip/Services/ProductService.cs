using AutoMapper;
using Microsoft.EntityFrameworkCore;
using stokTakip.Data;
using stokTakip.DTOs;
using stokTakip.Models;

namespace stokTakip.Services
{
    public class ProductService : IProductService
    {
        private readonly StokTakipDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(StokTakipDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int productId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsActive);

            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);
            product.CreatedDate = DateTime.UtcNow;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Create initial stock movement
            var stockMovement = new StockMovement
            {
                ProductId = product.ProductId,
                MovementType = StockMovementType.In,
                Quantity = product.StockQuantity,
                UnitPrice = product.UnitPrice,
                Description = "Initial stock",
                MovementDate = DateTime.UtcNow,
                CreatedBy = "System"
            };

            _context.StockMovements.Add(stockMovement);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto?> UpdateProductAsync(int productId, UpdateProductDto updateProductDto)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsActive);

            if (product == null)
                return null;

            var oldStockQuantity = product.StockQuantity;
            var oldUnitPrice = product.UnitPrice;

            _mapper.Map(updateProductDto, product);
            product.UpdatedDate = DateTime.UtcNow;

            // If stock quantity changed, create stock movement
            if (oldStockQuantity != product.StockQuantity)
            {
                var quantityDifference = product.StockQuantity - oldStockQuantity;
                var movementType = quantityDifference > 0 ? StockMovementType.In : StockMovementType.Out;
                
                var stockMovement = new StockMovement
                {
                    ProductId = product.ProductId,
                    MovementType = movementType,
                    Quantity = Math.Abs(quantityDifference),
                    UnitPrice = product.UnitPrice,
                    Description = "Stock adjustment",
                    MovementDate = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                _context.StockMovements.Add(stockMovement);
            }

            await _context.SaveChangesAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsActive);

            if (product == null)
                return false;

            product.IsActive = false;
            product.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync()
        {
            var products = await _context.Products
                .Where(p => p.IsActive && p.StockQuantity <= p.MinStockLevel)
                .OrderBy(p => p.StockQuantity)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
        {
            var products = await _context.Products
                .Where(p => p.IsActive && 
                           (p.ProductName.Contains(searchTerm) ||
                            p.ProductCode != null && p.ProductCode.Contains(searchTerm) ||
                            p.Category != null && p.Category.Contains(searchTerm) ||
                            p.Brand != null && p.Brand.Contains(searchTerm)))
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<bool> UpdateStockQuantityAsync(int productId, int quantity, string? description = null)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsActive);

            if (product == null)
                return false;

            var oldQuantity = product.StockQuantity;
            product.StockQuantity += quantity;
            product.UpdatedDate = DateTime.UtcNow;

            // Create stock movement
            var movementType = quantity > 0 ? StockMovementType.In : StockMovementType.Out;
            var stockMovement = new StockMovement
            {
                ProductId = product.ProductId,
                MovementType = movementType,
                Quantity = Math.Abs(quantity),
                UnitPrice = product.UnitPrice,
                Description = description ?? "Stock update",
                MovementDate = DateTime.UtcNow,
                CreatedBy = "System"
            };

            _context.StockMovements.Add(stockMovement);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
