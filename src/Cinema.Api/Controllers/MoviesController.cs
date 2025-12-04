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
    private readonly CloudinaryImageService _cloudinaryService;
    private readonly ILogger<MoviesController> _logger;

    public MoviesController(
        IMovieRepository repo,
        FirestoreMovieService firestoreMovieService,
        CloudinaryImageService cloudinaryService,
        ILogger<MoviesController> logger)
    {
        _repo = repo;
        _firestoreMovieService = firestoreMovieService;
        _cloudinaryService = cloudinaryService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get() => Ok(await _repo.ListAsync());

    [HttpPost("add-movie")]
    public async Task<IActionResult> AddMovie([FromBody] AddMovieRequest request)
    {
        try
        {
            // If base64 image is provided, upload to Cloudinary
            if (!string.IsNullOrEmpty(request.PosterBase64))
            {
                var fileName = $"{request.Movie.Title.Replace(" ", "_")}_{Guid.NewGuid()}.jpg";
                request.Movie.PosterUrl = await _cloudinaryService.UploadImageFromBase64Async(
                    request.PosterBase64,
                    fileName,
                    "movies"
                );
                _logger.LogInformation($"Uploaded poster for movie: {request.Movie.Title}");
            }

            await _firestoreMovieService.AddMovieAsync(request.Movie);
            return Ok(new { success = true, id = request.Movie.Id, posterUrl = request.Movie.PosterUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add movie");
            return StatusCode(500, new { success = false, message = "Failed to add movie.", error = ex.Message });
        }
    }

    [HttpPost("upload-poster")]
    public async Task<IActionResult> UploadPoster([FromBody] UploadPosterRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Base64Image))
                return BadRequest(new { success = false, message = "No image provided" });

            var fileName = $"{request.FileName ?? Guid.NewGuid().ToString()}.jpg";
            var imageUrl = await _cloudinaryService.UploadImageFromBase64Async(
                request.Base64Image,
                fileName,
                "movies"
            );

            return Ok(new { success = true, imageUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload poster");
            return StatusCode(500, new { success = false, message = "Failed to upload poster.", error = ex.Message });
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
            // Get movie to delete its poster from Cloudinary
            var movie = await _firestoreMovieService.GetMovieAsync(id);
            if (movie != null && !string.IsNullOrEmpty(movie.PosterUrl))
            {
                var publicId = _cloudinaryService.GetPublicIdFromUrl(movie.PosterUrl);
                if (publicId != null)
                {
                    await _cloudinaryService.DeleteImageAsync(publicId);
                    _logger.LogInformation($"Deleted poster for movie: {movie.Title}");
                }
            }

            await _firestoreMovieService.DeleteMovieAsync(id);
            return Ok(new { success = true, message = $"Movie {id} deleted." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete movie");
            return StatusCode(500, new { success = false, message = "Failed to delete movie.", error = ex.Message });
        }
    }

    [HttpPut("edit-movie/{id}")]
    public async Task<IActionResult> EditMovie(string id, [FromBody] EditMovieRequest request)
    {
        try
        {
            request.Movie.Id = id;

            // If new image is provided, upload it and delete old one
            if (!string.IsNullOrEmpty(request.PosterBase64))
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(request.Movie.PosterUrl))
                {
                    var publicId = _cloudinaryService.GetPublicIdFromUrl(request.Movie.PosterUrl);
                    if (publicId != null)
                    {
                        await _cloudinaryService.DeleteImageAsync(publicId);
                    }
                }

                // Upload new image
                var fileName = $"{request.Movie.Title.Replace(" ", "_")}_{Guid.NewGuid()}.jpg";
                request.Movie.PosterUrl = await _cloudinaryService.UploadImageFromBase64Async(
                    request.PosterBase64,
                    fileName,
                    "movies"
                );
                _logger.LogInformation($"Updated poster for movie: {request.Movie.Title}");
            }

            await _firestoreMovieService.UpdateMovieAsync(request.Movie);
            return Ok(new { success = true, movie = request.Movie });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to edit movie");
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

// DTOs for movie operations with image upload
public class AddMovieRequest
{
    public Movie Movie { get; set; } = new();
    public string? PosterBase64 { get; set; }
}

public class EditMovieRequest
{
    public Movie Movie { get; set; } = new();
    public string? PosterBase64 { get; set; }
}

public class UploadPosterRequest
{
    public string Base64Image { get; set; } = string.Empty;
    public string? FileName { get; set; }
}
