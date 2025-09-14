using stokTakip.DTOs;

namespace stokTakip.Services
{
    public interface ISaleService
    {
        Task<IEnumerable<SaleDto>> GetAllSalesAsync();
        Task<SaleDto?> GetSaleByIdAsync(int saleId);
        Task<SaleDto> CreateSaleAsync(CreateSaleDto createSaleDto);
        Task<SaleDto?> UpdateSaleAsync(int saleId, UpdateSaleDto updateSaleDto);
        Task<bool> DeleteSaleAsync(int saleId);
        Task<IEnumerable<SaleDto>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<SaleDto>> GetSalesByCustomerAsync(int customerId);
        Task<string> GenerateSaleNumberAsync();
    }
}
