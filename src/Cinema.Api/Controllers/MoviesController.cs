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
}
