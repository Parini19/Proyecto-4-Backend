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

        [HttpPost("seed")]
        public async Task<IActionResult> SeedFoodCombos([FromQuery] bool clearExisting = false)
        {
            try
            {
                // Clear existing if requested
                if (clearExisting)
                {
                    var existingCombos = await _foodComboService.GetAllFoodCombosAsync();
                    foreach (var combo in existingCombos)
                    {
                        await _foodComboService.DeleteFoodComboAsync(combo.Id);
                    }
                }

                var combos = new System.Collections.Generic.List<FoodCombo>();

                // COMBOS
                combos.Add(new FoodCombo
                {
                    Id = "COMBO-001",
                    Name = "Combo Clásico",
                    Description = "Palomitas medianas + Refresco mediano",
                    Price = 3500, // ₡3,500 colones
                    Items = new System.Collections.Generic.List<string> { "Palomitas medianas", "Refresco mediano" },
                    ImageUrl = "https://via.placeholder.com/400x300/FF6B35/FFFFFF?text=Combo+Clasico",
                    Category = "combo",
                    IsAvailable = true
                });

                combos.Add(new FoodCombo
                {
                    Id = "COMBO-002",
                    Name = "Combo Pareja",
                    Description = "Palomitas grandes + 2 Refrescos medianos",
                    Price = 5500, // ₡5,500 colones
                    Items = new System.Collections.Generic.List<string> { "Palomitas grandes", "2 Refrescos medianos" },
                    ImageUrl = "https://via.placeholder.com/400x300/FF6B35/FFFFFF?text=Combo+Pareja",
                    Category = "combo",
                    IsAvailable = true
                });

                combos.Add(new FoodCombo
                {
                    Id = "COMBO-003",
                    Name = "Combo Familiar",
                    Description = "Palomitas jumbo + 4 Refrescos medianos + 2 Nachos",
                    Price = 9500, // ₡9,500 colones
                    Items = new System.Collections.Generic.List<string> { "Palomitas jumbo", "4 Refrescos medianos", "2 Nachos" },
                    ImageUrl = "https://via.placeholder.com/400x300/FF6B35/FFFFFF?text=Combo+Familiar",
                    Category = "combo",
                    IsAvailable = true
                });

                combos.Add(new FoodCombo
                {
                    Id = "COMBO-004",
                    Name = "Combo Dulce",
                    Description = "Palomitas medianas + Refresco + M&M's",
                    Price = 4200, // ₡4,200 colones
                    Items = new System.Collections.Generic.List<string> { "Palomitas medianas", "Refresco mediano", "M&M's" },
                    ImageUrl = "https://via.placeholder.com/400x300/FF6B35/FFFFFF?text=Combo+Dulce",
                    Category = "combo",
                    IsAvailable = true
                });

                // PALOMITAS INDIVIDUALES
                combos.Add(new FoodCombo
                {
                    Id = "POPCORN-001",
                    Name = "Palomitas Pequeñas",
                    Description = "Palomitas con mantequilla",
                    Price = 1500, // ₡1,500 colones
                    Items = new System.Collections.Generic.List<string> { "Palomitas pequeñas" },
                    ImageUrl = "https://via.placeholder.com/400x300/FFD93D/000000?text=Palomitas+Pequenas",
                    Category = "popcorn",
                    IsAvailable = true
                });

                combos.Add(new FoodCombo
                {
                    Id = "POPCORN-002",
                    Name = "Palomitas Medianas",
                    Description = "Palomitas con mantequilla",
                    Price = 2200, // ₡2,200 colones
                    Items = new System.Collections.Generic.List<string> { "Palomitas medianas" },
                    ImageUrl = "https://via.placeholder.com/400x300/FFD93D/000000?text=Palomitas+Medianas",
                    Category = "popcorn",
                    IsAvailable = true
                });

                combos.Add(new FoodCombo
                {
                    Id = "POPCORN-003",
                    Name = "Palomitas Grandes",
                    Description = "Palomitas con mantequilla",
                    Price = 3000, // ₡3,000 colones
                    Items = new System.Collections.Generic.List<string> { "Palomitas grandes" },
                    ImageUrl = "https://via.placeholder.com/400x300/FFD93D/000000?text=Palomitas+Grandes",
                    Category = "popcorn",
                    IsAvailable = true
                });

                // BEBIDAS
                combos.Add(new FoodCombo
                {
                    Id = "DRINK-001",
                    Name = "Refresco Pequeño",
                    Description = "Coca-Cola, Sprite o Fanta",
                    Price = 1200, // ₡1,200 colones
                    Items = new System.Collections.Generic.List<string> { "Refresco pequeño" },
                    ImageUrl = "https://via.placeholder.com/400x300/E63946/FFFFFF?text=Refresco+Pequeno",
                    Category = "drink",
                    IsAvailable = true
                });

                combos.Add(new FoodCombo
                {
                    Id = "DRINK-002",
                    Name = "Refresco Mediano",
                    Description = "Coca-Cola, Sprite o Fanta",
                    Price = 1800, // ₡1,800 colones
                    Items = new System.Collections.Generic.List<string> { "Refresco mediano" },
                    ImageUrl = "https://via.placeholder.com/400x300/E63946/FFFFFF?text=Refresco+Mediano",
                    Category = "drink",
                    IsAvailable = true
                });

                combos.Add(new FoodCombo
                {
                    Id = "DRINK-003",
                    Name = "Refresco Grande",
                    Description = "Coca-Cola, Sprite o Fanta",
                    Price = 2400, // ₡2,400 colones
                    Items = new System.Collections.Generic.List<string> { "Refresco grande" },
                    ImageUrl = "https://via.placeholder.com/400x300/E63946/FFFFFF?text=Refresco+Grande",
                    Category = "drink",
                    IsAvailable = true
                });

                combos.Add(new FoodCombo
                {
                    Id = "DRINK-004",
                    Name = "Agua Embotellada",
                    Description = "Agua natural 600ml",
                    Price = 1000, // ₡1,000 colones
                    Items = new System.Collections.Generic.List<string> { "Agua 600ml" },
                    ImageUrl = "https://via.placeholder.com/400x300/457B9D/FFFFFF?text=Agua",
                    Category = "drink",
                    IsAvailable = true
                });

                // DULCES
                combos.Add(new FoodCombo
                {
                    Id = "CANDY-001",
                    Name = "M&M's",
                    Description = "Chocolate con cacahuate",
                    Price = 1500, // ₡1,500 colones
                    Items = new System.Collections.Generic.List<string> { "M&M's 45g" },
                    ImageUrl = "https://via.placeholder.com/400x300/A8DADC/000000?text=M%26Ms",
                    Category = "candy",
                    IsAvailable = true
                });

                combos.Add(new FoodCombo
                {
                    Id = "CANDY-002",
                    Name = "Skittles",
                    Description = "Caramelos de frutas",
                    Price = 1500, // ₡1,500 colones
                    Items = new System.Collections.Generic.List<string> { "Skittles 61g" },
                    ImageUrl = "https://via.placeholder.com/400x300/A8DADC/000000?text=Skittles",
                    Category = "candy",
                    IsAvailable = true
                });

                combos.Add(new FoodCombo
                {
                    Id = "CANDY-003",
                    Name = "Kit Kat",
                    Description = "Chocolate con wafer",
                    Price = 1800, // ₡1,800 colones
                    Items = new System.Collections.Generic.List<string> { "Kit Kat" },
                    ImageUrl = "https://via.placeholder.com/400x300/A8DADC/000000?text=Kit+Kat",
                    Category = "candy",
                    IsAvailable = true
                });

                // SNACKS
                combos.Add(new FoodCombo
                {
                    Id = "SNACK-001",
                    Name = "Nachos con Queso",
                    Description = "Nachos con queso cheddar",
                    Price = 2500, // ₡2,500 colones
                    Items = new System.Collections.Generic.List<string> { "Nachos", "Queso cheddar" },
                    ImageUrl = "https://via.placeholder.com/400x300/F4A261/000000?text=Nachos",
                    Category = "snack",
                    IsAvailable = true
                });

                combos.Add(new FoodCombo
                {
                    Id = "SNACK-002",
                    Name = "Hot Dog",
                    Description = "Hot dog con condimentos",
                    Price = 2800, // ₡2,800 colones
                    Items = new System.Collections.Generic.List<string> { "Hot dog", "Pan", "Salsas" },
                    ImageUrl = "https://via.placeholder.com/400x300/F4A261/000000?text=Hot+Dog",
                    Category = "snack",
                    IsAvailable = true
                });

                combos.Add(new FoodCombo
                {
                    Id = "SNACK-003",
                    Name = "Papas Fritas",
                    Description = "Papas fritas crujientes",
                    Price = 2000, // ₡2,000 colones
                    Items = new System.Collections.Generic.List<string> { "Papas fritas" },
                    ImageUrl = "https://via.placeholder.com/400x300/F4A261/000000?text=Papas+Fritas",
                    Category = "snack",
                    IsAvailable = true
                });

                // Save all combos
                foreach (var combo in combos)
                {
                    await _foodComboService.AddFoodComboAsync(combo);
                }

                return Ok(new
                {
                    success = true,
                    message = $"Created {combos.Count} food items",
                    count = combos.Count,
                    combos = combos.GroupBy(c => c.Category).Select(g => new
                    {
                        category = g.Key,
                        count = g.Count(),
                        items = g.Select(c => new { c.Id, c.Name, c.Price }).ToList()
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to seed food combos.", error = ex.Message });
            }
        }

        [HttpDelete("clear-all")]
        public async Task<IActionResult> ClearAllFoodCombos()
        {
            try
            {
                var combos = await _foodComboService.GetAllFoodCombosAsync();
                foreach (var combo in combos)
                {
                    await _foodComboService.DeleteFoodComboAsync(combo.Id);
                }

                return Ok(new { success = true, message = $"Deleted {combos.Count} food combos", count = combos.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to clear food combos.", error = ex.Message });
            }
        }
    }
}