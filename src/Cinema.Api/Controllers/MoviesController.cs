using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cinema.Application.Movies;
using Cinema.Api.Services;
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
}
