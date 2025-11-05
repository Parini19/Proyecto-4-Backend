using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cinema.Application.Screenings;
using Cinema.Domain.Entities;

namespace Cinema.Api.Controllers
{
    [ApiController]
    [Route("api/screenings")]
    public class ScreeningsController : ControllerBase
    {
        private readonly IScreeningRepository _repo;
        public ScreeningsController(IScreeningRepository repo) => _repo = repo;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get() => Ok(await _repo.ListAsync());

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string id)
        {
            var screening = await _repo.GetByIdAsync(id);
            if (screening == null)
                return NotFound(new { message = $"Screening with id '{id}' not found" });

            return Ok(screening);
        }

        [HttpGet("by-movie/{movieId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByMovieId(string movieId)
        {
            var screenings = await _repo.GetByMovieIdAsync(movieId);
            return Ok(screenings);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] Screening screening)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _repo.AddAsync(screening);
            return CreatedAtAction(nameof(GetById), new { id = screening.Id }, screening);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, [FromBody] Screening screening)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Screening with id '{id}' not found" });

            screening.Id = id;
            await _repo.UpdateAsync(screening);
            return Ok(screening);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Screening with id '{id}' not found" });

            await _repo.DeleteAsync(id);
            return NoContent();
        }

        // Legacy endpoints for backwards compatibility
        [HttpPost("add-screening")]
        [Authorize]
        public async Task<IActionResult> AddScreening([FromBody] Screening screening) => await Create(screening);

        [HttpGet("get-screening/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetScreening(string id) => await GetById(id);

        [HttpPut("edit-screening/{id}")]
        [Authorize]
        public async Task<IActionResult> EditScreening(string id, [FromBody] Screening screening) => await Update(id, screening);

        [HttpDelete("delete-screening/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteScreening(string id) => await Delete(id);

        [HttpGet("get-all-screenings")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllScreenings() => await Get();
    }
}