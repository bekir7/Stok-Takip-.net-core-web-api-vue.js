using AutoMapper;
using Microsoft.EntityFrameworkCore;
using stokTakip.Data;
using stokTakip.DTOs;
using stokTakip.Models;

namespace stokTakip.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly StokTakipDbContext _context;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public PurchaseService(StokTakipDbContext context, IMapper mapper, IProductService productService)
        {
            _context = context;
            _mapper = mapper;
            _productService = productService;
        }

        public async Task<IEnumerable<PurchaseDto>> GetAllPurchasesAsync()
        {
            var purchases = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Product)
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PurchaseDto>>(purchases);
        }

        public async Task<PurchaseDto?> GetPurchaseByIdAsync(int purchaseId)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Product)
                .FirstOrDefaultAsync(p => p.PurchaseId == purchaseId);

            return purchase == null ? null : _mapper.Map<PurchaseDto>(purchase);
        }

        public async Task<PurchaseDto> CreatePurchaseAsync(CreatePurchaseDto createPurchaseDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var purchase = new Purchase
                {
                    SupplierId = createPurchaseDto.SupplierId,
                    PurchaseNumber = await GeneratePurchaseNumberAsync(),
                    PurchaseDate = createPurchaseDto.PurchaseDate,
                    Notes = createPurchaseDto.Notes,
                    PaymentMethod = createPurchaseDto.PaymentMethod,
                    IsPaid = createPurchaseDto.IsPaid,
                    PaymentDate = createPurchaseDto.IsPaid ? DateTime.UtcNow : null,
                    CreatedBy = createPurchaseDto.CreatedBy,
                    CreatedDate = DateTime.UtcNow
                };

                decimal subTotal = 0;
                decimal totalTaxAmount = 0;
                decimal totalDiscountAmount = 0;

                foreach (var itemDto in createPurchaseDto.PurchaseItems)
                {
                    var product = await _context.Products.FindAsync(itemDto.ProductId);
                    if (product == null || !product.IsActive)
                        throw new InvalidOperationException($"Product with ID {itemDto.ProductId} not found or inactive");

                    var itemTotal = (itemDto.UnitPrice * itemDto.Quantity) - itemDto.DiscountAmount;
                    var taxAmount = itemTotal * (itemDto.TaxRate / 100);

                    var purchaseItem = new PurchaseItem
                    {
                        PurchaseId = purchase.PurchaseId,
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity,
                        UnitPrice = itemDto.UnitPrice,
                        DiscountAmount = itemDto.DiscountAmount,
                        TaxRate = itemDto.TaxRate,
                        TotalAmount = itemTotal + taxAmount
                    };

                    purchase.PurchaseItems.Add(purchaseItem);

                    subTotal += itemTotal;
                    totalTaxAmount += taxAmount;
                    totalDiscountAmount += itemDto.DiscountAmount;

                    // Update product stock
                    await _productService.UpdateStockQuantityAsync(
                        itemDto.ProductId,
                        itemDto.Quantity,
                        $"Purchase: {purchase.PurchaseNumber}"
                    );
                }

                purchase.SubTotal = subTotal;
                purchase.TaxAmount = totalTaxAmount;
                purchase.DiscountAmount = totalDiscountAmount;
                purchase.TotalAmount = subTotal + totalTaxAmount;

                _context.Purchases.Add(purchase);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetPurchaseByIdAsync(purchase.PurchaseId) ?? throw new InvalidOperationException("Failed to retrieve created purchase");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PurchaseDto?> UpdatePurchaseAsync(int purchaseId, UpdatePurchaseDto updatePurchaseDto)
        {
            var purchase = await _context.Purchases
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync(p => p.PurchaseId == purchaseId);

            if (purchase == null)
                return null;

            purchase.SupplierId = updatePurchaseDto.SupplierId;
            purchase.PurchaseDate = updatePurchaseDto.PurchaseDate;
            purchase.Notes = updatePurchaseDto.Notes;
            purchase.PaymentMethod = updatePurchaseDto.PaymentMethod;
            purchase.IsPaid = updatePurchaseDto.IsPaid;
            purchase.PaymentDate = updatePurchaseDto.PaymentDate;

            await _context.SaveChangesAsync();
            return await GetPurchaseByIdAsync(purchaseId);
        }

        public async Task<bool> DeletePurchaseAsync(int purchaseId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var purchase = await _context.Purchases
                    .Include(p => p.PurchaseItems)
                    .FirstOrDefaultAsync(p => p.PurchaseId == purchaseId);

                if (purchase == null)
                    return false;

                // Reduce stock quantities
                foreach (var item in purchase.PurchaseItems)
                {
                    await _productService.UpdateStockQuantityAsync(
                        item.ProductId,
                        -item.Quantity,
                        $"Purchase cancellation: {purchase.PurchaseNumber}"
                    );
                }

                _context.Purchases.Remove(purchase);
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

        public async Task<IEnumerable<PurchaseDto>> GetPurchasesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var purchases = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Product)
                .Where(p => p.PurchaseDate >= startDate && p.PurchaseDate <= endDate)
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PurchaseDto>>(purchases);
        }

        public async Task<IEnumerable<PurchaseDto>> GetPurchasesBySupplierAsync(int supplierId)
        {
            var purchases = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Product)
                .Where(p => p.SupplierId == supplierId)
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PurchaseDto>>(purchases);
        }

        public async Task<string> GeneratePurchaseNumberAsync()
        {
            var today = DateTime.UtcNow.Date;
            var count = await _context.Purchases
                .CountAsync(p => p.PurchaseDate.Date == today);

            return $"ALI-{today:yyyyMMdd}-{count + 1:D4}";
        }
    }
}
