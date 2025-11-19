using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cinema.Application.Movies;
<<<<<<< Updated upstream
=======
using Cinema.Api.Services;
>>>>>>> Stashed changes
using Cinema.Domain.Entities;

namespace Cinema.Api.Controllers;

[ApiController]
[Route("api/movies")]
public class MoviesController : ControllerBase
{
    private readonly IMovieRepository _repo;
    private readonly FirestoreMovieService _firestoreMovieService;

    public MoviesController(IMovieRepository repo, FirestoreMovieService firestoreMovieService)
    {
        _repo = repo;
        _firestoreMovieService = firestoreMovieService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get() => Ok(await _repo.ListAsync());

<<<<<<< Updated upstream
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
=======
    [HttpPost("add-movie")]
    public async Task<IActionResult> AddMovie([FromBody] Movie movie)
    {
        try
        {
            await _firestoreMovieService.AddMovieAsync(movie);
            return Ok(new { success = true, id = movie.Id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Failed to add movie.", error = ex.Message });
        }
    }

    [HttpGet("get-movie/{id}")]
    public async Task<IActionResult> GetMovie(string id)
    {
        try
        {
            var movie = await _firestoreMovieService.GetMovieAsync(id);
            if (movie == null)
                return NotFound(new { success = false, message = "Movie not found." });

            return Ok(new { success = true, movie });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Failed to get movie.", error = ex.Message });
        }
    }

    [HttpDelete("delete-movie/{id}")]
    public async Task<IActionResult> DeleteMovie(string id)
    {
        try
        {
            await _firestoreMovieService.DeleteMovieAsync(id);
            return Ok(new { success = true, message = $"Movie {id} deleted." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Failed to delete movie.", error = ex.Message });
        }
    }

    [HttpPut("edit-movie/{id}")]
    public async Task<IActionResult> EditMovie(string id, [FromBody] Movie movie)
    {
        try
        {
            movie.Id = id;
            await _firestoreMovieService.UpdateMovieAsync(movie);
            return Ok(new { success = true, movie });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Failed to edit movie.", error = ex.Message });
        }
    }

    [HttpGet("get-all-movies")]
    public async Task<IActionResult> GetAllMovies()
    {
        try
        {
            var movies = await _firestoreMovieService.GetAllMoviesAsync();
            return Ok(new { success = true, movies });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Failed to get movies.", error = ex.Message });
        }
    }
>>>>>>> Stashed changes
}
