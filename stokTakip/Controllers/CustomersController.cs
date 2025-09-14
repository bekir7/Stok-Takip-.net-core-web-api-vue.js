using Microsoft.AspNetCore.Mvc;
using stokTakip.DTOs;
using stokTakip.Services;

namespace stokTakip.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDto>> CreateCustomer(CreateCustomerDto createCustomerDto)
        {
            try
            {
                var customer = await _customerService.CreateCustomerAsync(createCustomerDto);
                return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerDto>> UpdateCustomer(int id, UpdateCustomerDto updateCustomerDto)
        {
            try
            {
                var customer = await _customerService.UpdateCustomerAsync(id, updateCustomerDto);
                if (customer == null)
                    return NotFound();

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> SearchCustomers([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("Search term is required");

            var customers = await _customerService.SearchCustomersAsync(q);
            return Ok(customers);
        }
    }
}
