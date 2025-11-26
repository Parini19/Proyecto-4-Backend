using Microsoft.AspNetCore.Mvc;
using Cinema.Domain.Entities;
using Cinema.Api.Services;

namespace Cinema.Api.Controllers
{
    [ApiController]
    [Route("api/screenings")]
    public class ScreeningsController : ControllerBase
    {
        private readonly FirestoreScreeningService _firestoreScreeningService;

        public ScreeningsController(FirestoreScreeningService firestoreScreeningService)
        {
            _firestoreScreeningService = firestoreScreeningService;
        }

        [HttpPost("add-screening")]
        public async Task<IActionResult> AddScreening([FromBody] Screening screening)
        {
            await _firestoreScreeningService.AddScreeningAsync(screening);
            return Ok(new { success = true, id = screening.Id });
        }

        [HttpGet("get-screening/{id}")]
        public async Task<IActionResult> GetScreening(string id)
        {
            var screening = await _firestoreScreeningService.GetScreeningAsync(id);
            if (screening == null)
                return NotFound(new { success = false, message = "Screening not found." });

            return Ok(new { success = true, screening });
        }

        [HttpDelete("delete-screening/{id}")]
        public async Task<IActionResult> DeleteScreening(string id)
        {
            await _firestoreScreeningService.DeleteScreeningAsync(id);
            return Ok(new { success = true, message = $"Screening {id} deleted." });
        }

        [HttpPut("edit-screening/{id}")]
        public async Task<IActionResult> EditScreening(string id, [FromBody] Screening screening)
        {
            screening.Id = id;
            await _firestoreScreeningService.UpdateScreeningAsync(screening);
            return Ok(new { success = true, screening });
        }

        [HttpGet("get-all-screenings")]
        public async Task<IActionResult> GetAllScreenings()
        {
            var screenings = await _firestoreScreeningService.GetAllScreeningsAsync();
            return Ok(new { success = true, screenings });
        }
    }
}