using stokTakip.DTOs;

namespace stokTakip.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int productId);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<ProductDto?> UpdateProductAsync(int productId, UpdateProductDto updateProductDto);
        Task<bool> DeleteProductAsync(int productId);
        Task<IEnumerable<ProductDto>> GetLowStockProductsAsync();
        Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
        Task<bool> UpdateStockQuantityAsync(int productId, int quantity, string? description = null);
    }
}
