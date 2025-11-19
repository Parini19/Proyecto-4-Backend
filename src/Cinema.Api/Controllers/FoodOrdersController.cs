using System;
using System.Linq;
using System.Threading.Tasks;
using Cinema.Api.Services;
using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace Cinema.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodOrdersController : ControllerBase
    {
        private readonly FirestoreFoodOrderService _foodOrderService;

        public FoodOrdersController(FirestoreFoodOrderService foodOrderService)
        {
            _foodOrderService = foodOrderService;
        }

        [HttpPost("add-food-order")]
        public async Task<IActionResult> AddFoodOrder([FromBody] FoodOrder order)
        {
            try
            {
                await _foodOrderService.AddFoodOrderAsync(order);
                return Ok(new { success = true, id = order.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to add food order.", error = ex.Message });
            }
        }

        [HttpGet("get-food-order/{id}")]
        public async Task<IActionResult> GetFoodOrder(string id)
        {
            try
            {
                var order = await _foodOrderService.GetFoodOrderAsync(id);
                if (order == null)
                    return NotFound(new { success = false, message = "Food order not found." });

                return Ok(new { success = true, order });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to get food order.", error = ex.Message });
            }
        }

        [HttpPut("edit-food-order/{id}")]
        public async Task<IActionResult> EditFoodOrder(string id, [FromBody] FoodOrder order)
        {
            try
            {
                order.Id = id;
                await _foodOrderService.UpdateFoodOrderAsync(order);
                return Ok(new { success = true, order });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to edit food order.", error = ex.Message });
            }
        }

        [HttpDelete("delete-food-order/{id}")]
        public async Task<IActionResult> DeleteFoodOrder(string id)
        {
            try
            {
                await _foodOrderService.DeleteFoodOrderAsync(id);
                return Ok(new { success = true, message = $"Food order {id} deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to delete food order.", error = ex.Message });
            }
        }

        [HttpGet("get-all-food-orders")]
        [FeatureGate("DatabaseReadAll")]
        public async Task<IActionResult> GetAllFoodOrders()
        {
            try
            {
                var orders = await _foodOrderService.GetAllFoodOrdersAsync();
                return Ok(new
                {
                    success = true,
                    orders
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to get food orders.", error = ex.Message });
            }
        }
    }
}