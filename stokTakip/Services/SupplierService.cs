using AutoMapper;
using Microsoft.EntityFrameworkCore;
using stokTakip.Data;
using stokTakip.DTOs;
using stokTakip.Models;

namespace stokTakip.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly StokTakipDbContext _context;
        private readonly IMapper _mapper;

        public SupplierService(StokTakipDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
        {
            var suppliers = await _context.Suppliers
                .Where(s => s.IsActive)
                .OrderBy(s => s.SupplierName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
        }

        public async Task<SupplierDto?> GetSupplierByIdAsync(int supplierId)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierId == supplierId && s.IsActive);

            return supplier == null ? null : _mapper.Map<SupplierDto>(supplier);
        }

        public async Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto createSupplierDto)
        {
            var supplier = _mapper.Map<Supplier>(createSupplierDto);
            supplier.CreatedDate = DateTime.UtcNow;

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return _mapper.Map<SupplierDto>(supplier);
        }

        public async Task<SupplierDto?> UpdateSupplierAsync(int supplierId, UpdateSupplierDto updateSupplierDto)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierId == supplierId && s.IsActive);

            if (supplier == null)
                return null;

            _mapper.Map(updateSupplierDto, supplier);
            supplier.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<SupplierDto>(supplier);
        }

        public async Task<bool> DeleteSupplierAsync(int supplierId)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierId == supplierId && s.IsActive);

            if (supplier == null)
                return false;

            supplier.IsActive = false;
            supplier.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SupplierDto>> SearchSuppliersAsync(string searchTerm)
        {
            var suppliers = await _context.Suppliers
                .Where(s => s.IsActive && 
                           (s.SupplierName.Contains(searchTerm) ||
                            s.CompanyName != null && s.CompanyName.Contains(searchTerm) ||
                            s.PhoneNumber != null && s.PhoneNumber.Contains(searchTerm) ||
                            s.Email != null && s.Email.Contains(searchTerm)))
                .OrderBy(s => s.SupplierName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
        }
    }
}
