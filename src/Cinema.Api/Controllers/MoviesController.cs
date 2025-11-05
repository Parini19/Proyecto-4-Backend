using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cinema.Application.Movies;
using Cinema.Domain.Entities;

namespace Cinema.Api.Controllers;

[ApiController]
[Route("api/movies")]
public class MoviesController : ControllerBase
{
    private readonly IMovieRepository _repo;
    public MoviesController(IMovieRepository repo) => _repo = repo;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get() => Ok(await _repo.ListAsync());

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(string id)
    {
        var movie = await _repo.GetByIdAsync(id);
        if (movie == null)
            return NotFound(new { message = $"Movie with id '{id}' not found" });

        return Ok(movie);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] Movie movie)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _repo.AddAsync(movie);
        return CreatedAtAction(nameof(GetById), new { id = movie.Id }, movie);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(string id, [FromBody] Movie movie)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _repo.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = $"Movie with id '{id}' not found" });

        movie.Id = id;
        await _repo.UpdateAsync(movie);
        return Ok(movie);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(string id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = $"Movie with id '{id}' not found" });

        await _repo.DeleteAsync(id);
        return NoContent();
    }

    // Legacy endpoints for backwards compatibility
    [HttpPost("add-movie")]
    [Authorize]
    public async Task<IActionResult> AddMovie([FromBody] Movie movie) => await Create(movie);

    [HttpGet("get-movie/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMovie(string id) => await GetById(id);

    [HttpPut("edit-movie/{id}")]
    [Authorize]
    public async Task<IActionResult> EditMovie(string id, [FromBody] Movie movie) => await Update(id, movie);

    [HttpDelete("delete-movie/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteMovie(string id) => await Delete(id);

    [HttpGet("get-all-movies")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllMovies() => await Get();
}
