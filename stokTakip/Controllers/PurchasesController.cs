using Microsoft.AspNetCore.Mvc;
using stokTakip.DTOs;
using stokTakip.Services;

namespace stokTakip.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchasesController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseDto>>> GetPurchases()
        {
            var purchases = await _purchaseService.GetAllPurchasesAsync();
            return Ok(purchases);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseDto>> GetPurchase(int id)
        {
            var purchase = await _purchaseService.GetPurchaseByIdAsync(id);
            if (purchase == null)
                return NotFound();

            return Ok(purchase);
        }

        [HttpPost]
        public async Task<ActionResult<PurchaseDto>> CreatePurchase(CreatePurchaseDto createPurchaseDto)
        {
            try
            {
                var purchase = await _purchaseService.CreatePurchaseAsync(createPurchaseDto);
                return CreatedAtAction(nameof(GetPurchase), new { id = purchase.PurchaseId }, purchase);
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
        public async Task<ActionResult<PurchaseDto>> UpdatePurchase(int id, UpdatePurchaseDto updatePurchaseDto)
        {
            try
            {
                var purchase = await _purchaseService.UpdatePurchaseAsync(id, updatePurchaseDto);
                if (purchase == null)
                    return NotFound();

                return Ok(purchase);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePurchase(int id)
        {
            var result = await _purchaseService.DeletePurchaseAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<PurchaseDto>>> GetPurchasesByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var purchases = await _purchaseService.GetPurchasesByDateRangeAsync(startDate, endDate);
            return Ok(purchases);
        }

        [HttpGet("supplier/{supplierId}")]
        public async Task<ActionResult<IEnumerable<PurchaseDto>>> GetPurchasesBySupplier(int supplierId)
        {
            var purchases = await _purchaseService.GetPurchasesBySupplierAsync(supplierId);
            return Ok(purchases);
        }

        [HttpGet("purchase-number")]
        public async Task<ActionResult<string>> GeneratePurchaseNumber()
        {
            var purchaseNumber = await _purchaseService.GeneratePurchaseNumberAsync();
            return Ok(new { purchaseNumber });
        }
    }
}
