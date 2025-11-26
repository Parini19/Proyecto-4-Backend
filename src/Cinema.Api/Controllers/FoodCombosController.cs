using System;
using System.Linq;
using System.Threading.Tasks;
using Cinema.Api.Services;
using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement.Mvc;

namespace Cinema.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodCombosController : ControllerBase
    {
        private readonly FirestoreFoodComboService _foodComboService;

        public FoodCombosController(FirestoreFoodComboService foodComboService)
        {
            _foodComboService = foodComboService;
        }

        [HttpPost("add-food-combo")]
        public async Task<IActionResult> AddFoodCombo([FromBody] FoodCombo combo)
        {
            try
            {
                await _foodComboService.AddFoodComboAsync(combo);
                return Ok(new { success = true, id = combo.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to add food combo.", error = ex.Message });
            }
        }

        [HttpGet("get-food-combo/{id}")]
        public async Task<IActionResult> GetFoodCombo(string id)
        {
            try
            {
                var combo = await _foodComboService.GetFoodComboAsync(id);
                if (combo == null)
                    return NotFound(new { success = false, message = "Food combo not found." });

                return Ok(new { success = true, combo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to get food combo.", error = ex.Message });
            }
        }

        [HttpPut("edit-food-combo/{id}")]
        public async Task<IActionResult> EditFoodCombo(string id, [FromBody] FoodCombo combo)
        {
            try
            {
                combo.Id = id;
                await _foodComboService.UpdateFoodComboAsync(combo);
                return Ok(new { success = true, combo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to edit food combo.", error = ex.Message });
            }
        }

        [HttpDelete("delete-food-combo/{id}")]
        public async Task<IActionResult> DeleteFoodCombo(string id)
        {
            try
            {
                await _foodComboService.DeleteFoodComboAsync(id);
                return Ok(new { success = true, message = $"Food combo {id} deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to delete food combo.", error = ex.Message });
            }
        }

        [HttpGet("get-all-food-combos")]
        public async Task<IActionResult> GetAllFoodCombos()
        {
            try
            {
                var combos = await _foodComboService.GetAllFoodCombosAsync();
                return Ok(new { success = true, combos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to get food combos.", error = ex.Message });
            }
        }
    }
}