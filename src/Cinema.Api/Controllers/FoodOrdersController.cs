using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodOrdersController : ControllerBase
    {
        [HttpPost("add-food-order")]
        public IActionResult AddFoodOrder()
        {
            // TODO: Implement add food order logic
            return Ok();
        }

        [HttpGet("get-food-order/{id}")]
        public IActionResult GetFoodOrder(string id)
        {
            // TODO: Implement get food order logic
            return Ok();
        }

        [HttpPut("edit-food-order/{id}")]
        public IActionResult EditFoodOrder(string id)
        {
            // TODO: Implement edit food order logic
            return Ok();
        }

        [HttpDelete("delete-food-order/{id}")]
        public IActionResult DeleteFoodOrder(string id)
        {
            // TODO: Implement delete food order logic
            return Ok();
        }

        [HttpGet("get-all-food-orders")]
        public IActionResult GetAllFoodOrders()
        {
            // TODO: Implement get all food orders logic
            return Ok();
        }
    }
}