using stokTakip.DTOs;

namespace stokTakip.Services
{
    public interface IPurchaseService
    {
        Task<IEnumerable<PurchaseDto>> GetAllPurchasesAsync();
        Task<PurchaseDto?> GetPurchaseByIdAsync(int purchaseId);
        Task<PurchaseDto> CreatePurchaseAsync(CreatePurchaseDto createPurchaseDto);
        Task<PurchaseDto?> UpdatePurchaseAsync(int purchaseId, UpdatePurchaseDto updatePurchaseDto);
        Task<bool> DeletePurchaseAsync(int purchaseId);
        Task<IEnumerable<PurchaseDto>> GetPurchasesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PurchaseDto>> GetPurchasesBySupplierAsync(int supplierId);
        Task<string> GeneratePurchaseNumberAsync();
    }
}
