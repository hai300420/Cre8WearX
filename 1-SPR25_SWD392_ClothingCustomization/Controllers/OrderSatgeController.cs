
using Microsoft.AspNetCore.Mvc;
using Service.Service;
using static BusinessObject.RequestDTO.RequestDTO;

namespace SPR25_SWD392_ClothingCustomization.Controllers
{
    [Route("api/order-stages")]
    [ApiController]
    public class OrderStageController : ControllerBase
    {
        private readonly IOrderStageService _orderStageService;

        public OrderStageController(IOrderStageService orderStageService)
        {
            _orderStageService = orderStageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrderStages() => Ok(await _orderStageService.GetAllOrderStagesAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderStageById(int id) => Ok(await _orderStageService.GetOrderStageByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> CreateOrderStage([FromBody] OrderStageCreateDTO orderStageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data", errors = ModelState });
            }

            try
            {
                var response = await _orderStageService.CreateOrderStageAsync(orderStageDto);
                return StatusCode(response.Status, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred", error = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderStage(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid OrderStage ID. It must be greater than 0." });
            }

            try
            {
                var response = await _orderStageService.DeleteOrderStageAsync(id);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred", error = ex.Message });
            }
        }

    }
}
