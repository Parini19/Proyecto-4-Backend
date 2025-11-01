using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cinema.Application.Movies;

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

    [HttpPost("add-movie")]
    public IActionResult AddMovie()
    {
        // TODO: Implement add movie logic
        return Ok();
    }

    [HttpGet("get-movie/{id}")]
    public IActionResult GetMovie(string id)
    {
        // TODO: Implement get movie logic
        return Ok();
    }

    [HttpPut("edit-movie/{id}")]
    public IActionResult EditMovie(string id)
    {
        // TODO: Implement edit movie logic
        return Ok();
    }

    [HttpDelete("delete-movie/{id}")]
    public IActionResult DeleteMovie(string id)
    {
        // TODO: Implement delete movie logic
        return Ok();
    }

    [HttpGet("get-all-movies")]
    public IActionResult GetAllMovies()
    {
        // TODO: Implement get all movies logic
        return Ok();
    }
}
