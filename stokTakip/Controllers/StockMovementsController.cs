using Microsoft.AspNetCore.Mvc;
using stokTakip.DTOs;
using stokTakip.Services;

namespace stokTakip.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockMovementsController : ControllerBase
    {
        private readonly IStockMovementService _stockMovementService;

        public StockMovementsController(IStockMovementService stockMovementService)
        {
            _stockMovementService = stockMovementService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetStockMovements()
        {
            var movements = await _stockMovementService.GetAllStockMovementsAsync();
            return Ok(movements);
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetStockMovementsByProduct(int productId)
        {
            var movements = await _stockMovementService.GetStockMovementsByProductAsync(productId);
            return Ok(movements);
        }

        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetStockMovementsByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var movements = await _stockMovementService.GetStockMovementsByDateRangeAsync(startDate, endDate);
            return Ok(movements);
        }

        [HttpGet("type/{movementType}")]
        public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetStockMovementsByType(Models.StockMovementType movementType)
        {
            var movements = await _stockMovementService.GetStockMovementsByTypeAsync(movementType);
            return Ok(movements);
        }

        [HttpPost]
        public async Task<ActionResult<StockMovementDto>> CreateStockMovement(CreateStockMovementDto createStockMovementDto)
        {
            try
            {
                var movement = await _stockMovementService.CreateStockMovementAsync(createStockMovementDto);
                return CreatedAtAction(nameof(GetStockMovements), new { id = movement.StockMovementId }, movement);
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
    }
}
