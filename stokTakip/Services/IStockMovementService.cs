using stokTakip.DTOs;
using stokTakip.Models;

namespace stokTakip.Services
{
    public interface IStockMovementService
    {
        Task<IEnumerable<StockMovementDto>> GetAllStockMovementsAsync();
        Task<IEnumerable<StockMovementDto>> GetStockMovementsByProductAsync(int productId);
        Task<IEnumerable<StockMovementDto>> GetStockMovementsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<StockMovementDto> CreateStockMovementAsync(CreateStockMovementDto createStockMovementDto);
        Task<IEnumerable<StockMovementDto>> GetStockMovementsByTypeAsync(Models.StockMovementType movementType);
    }
}
