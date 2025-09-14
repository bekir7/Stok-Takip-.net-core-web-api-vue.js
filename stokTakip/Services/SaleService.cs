using AutoMapper;
using Microsoft.EntityFrameworkCore;
using stokTakip.Data;
using stokTakip.DTOs;
using stokTakip.Models;

namespace stokTakip.Services
{
    public class SaleService : ISaleService
    {
        private readonly StokTakipDbContext _context;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public SaleService(StokTakipDbContext context, IMapper mapper, IProductService productService)
        {
            _context = context;
            _mapper = mapper;
            _productService = productService;
        }

        public async Task<IEnumerable<SaleDto>> GetAllSalesAsync()
        {
            var sales = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<SaleDto?> GetSaleByIdAsync(int saleId)
        {
            var sale = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .FirstOrDefaultAsync(s => s.SaleId == saleId);

            return sale == null ? null : _mapper.Map<SaleDto>(sale);
        }

        public async Task<SaleDto> CreateSaleAsync(CreateSaleDto createSaleDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var sale = new Sale
                {
                    CustomerId = createSaleDto.CustomerId,
                    SaleNumber = await GenerateSaleNumberAsync(),
                    SaleDate = createSaleDto.SaleDate,
                    Notes = createSaleDto.Notes,
                    PaymentMethod = createSaleDto.PaymentMethod,
                    IsPaid = createSaleDto.IsPaid,
                    PaymentDate = createSaleDto.IsPaid ? DateTime.UtcNow : null,
                    CreatedBy = createSaleDto.CreatedBy,
                    CreatedDate = DateTime.UtcNow
                };

                decimal subTotal = 0;
                decimal totalTaxAmount = 0;
                decimal totalDiscountAmount = 0;

                foreach (var itemDto in createSaleDto.SaleItems)
                {
                    var product = await _context.Products.FindAsync(itemDto.ProductId);
                    if (product == null || !product.IsActive)
                        throw new InvalidOperationException($"Product with ID {itemDto.ProductId} not found or inactive");

                    if (product.StockQuantity < itemDto.Quantity)
                        throw new InvalidOperationException($"Insufficient stock for product {product.ProductName}. Available: {product.StockQuantity}, Requested: {itemDto.Quantity}");

                    var itemTotal = (itemDto.UnitPrice * itemDto.Quantity) - itemDto.DiscountAmount;
                    var taxAmount = itemTotal * (itemDto.TaxRate / 100);

                    var saleItem = new SaleItem
                    {
                        SaleId = sale.SaleId,
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity,
                        UnitPrice = itemDto.UnitPrice,
                        DiscountAmount = itemDto.DiscountAmount,
                        TaxRate = itemDto.TaxRate,
                        TotalAmount = itemTotal + taxAmount
                    };

                    sale.SaleItems.Add(saleItem);

                    subTotal += itemTotal;
                    totalTaxAmount += taxAmount;
                    totalDiscountAmount += itemDto.DiscountAmount;

                    // Update product stock
                    await _productService.UpdateStockQuantityAsync(
                        itemDto.ProductId, 
                        -itemDto.Quantity, 
                        $"Sale: {sale.SaleNumber}"
                    );
                }

                sale.SubTotal = subTotal;
                sale.TaxAmount = totalTaxAmount;
                sale.DiscountAmount = totalDiscountAmount;
                sale.TotalAmount = subTotal + totalTaxAmount;

                _context.Sales.Add(sale);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetSaleByIdAsync(sale.SaleId) ?? throw new InvalidOperationException("Failed to retrieve created sale");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<SaleDto?> UpdateSaleAsync(int saleId, UpdateSaleDto updateSaleDto)
        {
            var sale = await _context.Sales
                .Include(s => s.SaleItems)
                .FirstOrDefaultAsync(s => s.SaleId == saleId);

            if (sale == null)
                return null;

            sale.CustomerId = updateSaleDto.CustomerId;
            sale.SaleDate = updateSaleDto.SaleDate;
            sale.Notes = updateSaleDto.Notes;
            sale.PaymentMethod = updateSaleDto.PaymentMethod;
            sale.IsPaid = updateSaleDto.IsPaid;
            sale.PaymentDate = updateSaleDto.PaymentDate;

            await _context.SaveChangesAsync();
            return await GetSaleByIdAsync(saleId);
        }

        public async Task<bool> DeleteSaleAsync(int saleId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var sale = await _context.Sales
                    .Include(s => s.SaleItems)
                    .FirstOrDefaultAsync(s => s.SaleId == saleId);

                if (sale == null)
                    return false;

                // Restore stock quantities
                foreach (var item in sale.SaleItems)
                {
                    await _productService.UpdateStockQuantityAsync(
                        item.ProductId,
                        item.Quantity,
                        $"Sale cancellation: {sale.SaleNumber}"
                    );
                }

                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<IEnumerable<SaleDto>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var sales = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<IEnumerable<SaleDto>> GetSalesByCustomerAsync(int customerId)
        {
            var sales = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .Where(s => s.CustomerId == customerId)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<string> GenerateSaleNumberAsync()
        {
            var today = DateTime.UtcNow.Date;
            var count = await _context.Sales
                .CountAsync(s => s.SaleDate.Date == today);

            return $"SAT-{today:yyyyMMdd}-{count + 1:D4}";
        }
    }
}
