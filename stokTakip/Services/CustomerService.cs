using AutoMapper;
using Microsoft.EntityFrameworkCore;
using stokTakip.Data;
using stokTakip.DTOs;
using stokTakip.Models;

namespace stokTakip.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly StokTakipDbContext _context;
        private readonly IMapper _mapper;

        public CustomerService(StokTakipDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            var customers = await _context.Customers
                .Where(c => c.IsActive)
                .OrderBy(c => c.CustomerName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(int customerId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.IsActive);

            return customer == null ? null : _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto)
        {
            var customer = _mapper.Map<Customer>(createCustomerDto);
            customer.CreatedDate = DateTime.UtcNow;

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto?> UpdateCustomerAsync(int customerId, UpdateCustomerDto updateCustomerDto)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.IsActive);

            if (customer == null)
                return null;

            _mapper.Map(updateCustomerDto, customer);
            customer.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.IsActive);

            if (customer == null)
                return false;

            customer.IsActive = false;
            customer.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CustomerDto>> SearchCustomersAsync(string searchTerm)
        {
            var customers = await _context.Customers
                .Where(c => c.IsActive && 
                           (c.CustomerName.Contains(searchTerm) ||
                            c.CompanyName != null && c.CompanyName.Contains(searchTerm) ||
                            c.PhoneNumber != null && c.PhoneNumber.Contains(searchTerm) ||
                            c.Email != null && c.Email.Contains(searchTerm)))
                .OrderBy(c => c.CustomerName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }
    }
}
