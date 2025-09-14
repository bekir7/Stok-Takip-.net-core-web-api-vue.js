using Microsoft.AspNetCore.Mvc;
using stokTakip.DTOs;
using stokTakip.Services;

namespace stokTakip.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> GetSuppliers()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            return Ok(suppliers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierDto>> GetSupplier(int id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            if (supplier == null)
                return NotFound();

            return Ok(supplier);
        }

        [HttpPost]
        public async Task<ActionResult<SupplierDto>> CreateSupplier(CreateSupplierDto createSupplierDto)
        {
            try
            {
                var supplier = await _supplierService.CreateSupplierAsync(createSupplierDto);
                return CreatedAtAction(nameof(GetSupplier), new { id = supplier.SupplierId }, supplier);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SupplierDto>> UpdateSupplier(int id, UpdateSupplierDto updateSupplierDto)
        {
            try
            {
                var supplier = await _supplierService.UpdateSupplierAsync(id, updateSupplierDto);
                if (supplier == null)
                    return NotFound();

                return Ok(supplier);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSupplier(int id)
        {
            var result = await _supplierService.DeleteSupplierAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> SearchSuppliers([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("Search term is required");

            var suppliers = await _supplierService.SearchSuppliersAsync(q);
            return Ok(suppliers);
        }
    }
}
