using Microsoft.AspNetCore.Mvc;
using stokTakip.DTOs;
using stokTakip.Services;

namespace stokTakip.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetSales()
        {
            var sales = await _saleService.GetAllSalesAsync();
            return Ok(sales);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SaleDto>> GetSale(int id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);
            if (sale == null)
                return NotFound();

            return Ok(sale);
        }

        [HttpPost]
        public async Task<ActionResult<SaleDto>> CreateSale(CreateSaleDto createSaleDto)
        {
            try
            {
                var sale = await _saleService.CreateSaleAsync(createSaleDto);
                return CreatedAtAction(nameof(GetSale), new { id = sale.SaleId }, sale);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SaleDto>> UpdateSale(int id, UpdateSaleDto updateSaleDto)
        {
            try
            {
                var sale = await _saleService.UpdateSaleAsync(id, updateSaleDto);
                if (sale == null)
                    return NotFound();

                return Ok(sale);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSale(int id)
        {
            var result = await _saleService.DeleteSaleAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetSalesByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var sales = await _saleService.GetSalesByDateRangeAsync(startDate, endDate);
            return Ok(sales);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetSalesByCustomer(int customerId)
        {
            var sales = await _saleService.GetSalesByCustomerAsync(customerId);
            return Ok(sales);
        }

        [HttpGet("sale-number")]
        public async Task<ActionResult<string>> GenerateSaleNumber()
        {
            var saleNumber = await _saleService.GenerateSaleNumberAsync();
            return Ok(new { saleNumber });
        }
    }
}
