using stokTakip.DTOs;

namespace stokTakip.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
        Task<CustomerDto?> GetCustomerByIdAsync(int customerId);
        Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto);
        Task<CustomerDto?> UpdateCustomerAsync(int customerId, UpdateCustomerDto updateCustomerDto);
        Task<bool> DeleteCustomerAsync(int customerId);
        Task<IEnumerable<CustomerDto>> SearchCustomersAsync(string searchTerm);
    }
}
