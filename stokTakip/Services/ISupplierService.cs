using stokTakip.DTOs;

namespace stokTakip.Services
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync();
        Task<SupplierDto?> GetSupplierByIdAsync(int supplierId);
        Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto createSupplierDto);
        Task<SupplierDto?> UpdateSupplierAsync(int supplierId, UpdateSupplierDto updateSupplierDto);
        Task<bool> DeleteSupplierAsync(int supplierId);
        Task<IEnumerable<SupplierDto>> SearchSuppliersAsync(string searchTerm);
    }
}
