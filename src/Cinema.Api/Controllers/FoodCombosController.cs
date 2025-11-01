using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodCombosController : ControllerBase
    {
        [HttpPost("add-food-combo")]
        public IActionResult AddFoodCombo()
        {
            // TODO: Implement add food combo logic
            return Ok();
        }

        [HttpGet("get-food-combo/{id}")]
        public IActionResult GetFoodCombo(string id)
        {
            // TODO: Implement get food combo logic
            return Ok();
        }

        [HttpPut("edit-food-combo/{id}")]
        public IActionResult EditFoodCombo(string id)
        {
            // TODO: Implement edit food combo logic
            return Ok();
        }

        [HttpDelete("delete-food-combo/{id}")]
        public IActionResult DeleteFoodCombo(string id)
        {
            // TODO: Implement delete food combo logic
            return Ok();
        }

        [HttpGet("get-all-food-combos")]
        public IActionResult GetAllFoodCombos()
        {
            // TODO: Implement get all food combos logic
            return Ok();
        }
    }
}