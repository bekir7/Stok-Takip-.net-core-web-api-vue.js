using AutoMapper;
using Microsoft.EntityFrameworkCore;
using stokTakip.Data;
using stokTakip.DTOs;
using stokTakip.Models;

namespace stokTakip.Services
{
    public class StockMovementService : IStockMovementService
    {
        private readonly StokTakipDbContext _context;
        private readonly IMapper _mapper;

        public StockMovementService(StokTakipDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StockMovementDto>> GetAllStockMovementsAsync()
        {
            var movements = await _context.StockMovements
                .Include(sm => sm.Product)
                .OrderByDescending(sm => sm.MovementDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<StockMovementDto>>(movements);
        }

        public async Task<IEnumerable<StockMovementDto>> GetStockMovementsByProductAsync(int productId)
        {
            var movements = await _context.StockMovements
                .Include(sm => sm.Product)
                .Where(sm => sm.ProductId == productId)
                .OrderByDescending(sm => sm.MovementDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<StockMovementDto>>(movements);
        }

        public async Task<IEnumerable<StockMovementDto>> GetStockMovementsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var movements = await _context.StockMovements
                .Include(sm => sm.Product)
                .Where(sm => sm.MovementDate >= startDate && sm.MovementDate <= endDate)
                .OrderByDescending(sm => sm.MovementDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<StockMovementDto>>(movements);
        }

        public async Task<StockMovementDto> CreateStockMovementAsync(CreateStockMovementDto createStockMovementDto)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == createStockMovementDto.ProductId && p.IsActive);

            if (product == null)
                throw new InvalidOperationException($"Product with ID {createStockMovementDto.ProductId} not found or inactive");

            var stockMovement = _mapper.Map<StockMovement>(createStockMovementDto);
            stockMovement.MovementDate = DateTime.UtcNow;

            _context.StockMovements.Add(stockMovement);

            // Update product stock quantity
            if (createStockMovementDto.MovementType == StockMovementType.In)
            {
                product.StockQuantity += createStockMovementDto.Quantity;
            }
            else if (createStockMovementDto.MovementType == StockMovementType.Out)
            {
                if (product.StockQuantity < createStockMovementDto.Quantity)
                    throw new InvalidOperationException($"Insufficient stock. Available: {product.StockQuantity}, Requested: {createStockMovementDto.Quantity}");

                product.StockQuantity -= createStockMovementDto.Quantity;
            }
            else if (createStockMovementDto.MovementType == StockMovementType.Adjustment)
            {
                product.StockQuantity = createStockMovementDto.Quantity;
            }

            product.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetStockMovementByIdAsync(stockMovement.StockMovementId) ?? 
                   throw new InvalidOperationException("Failed to retrieve created stock movement");
        }

        public async Task<IEnumerable<StockMovementDto>> GetStockMovementsByTypeAsync(Models.StockMovementType movementType)
        {
            var movements = await _context.StockMovements
                .Include(sm => sm.Product)
                .Where(sm => sm.MovementType == movementType)
                .OrderByDescending(sm => sm.MovementDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<StockMovementDto>>(movements);
        }

        private async Task<StockMovementDto?> GetStockMovementByIdAsync(int stockMovementId)
        {
            var movement = await _context.StockMovements
                .Include(sm => sm.Product)
                .FirstOrDefaultAsync(sm => sm.StockMovementId == stockMovementId);

            return movement == null ? null : _mapper.Map<StockMovementDto>(movement);
        }
    }
}
