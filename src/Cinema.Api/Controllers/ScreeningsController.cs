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
        private readonly FirestoreMovieService _firestoreMovieService;
        private readonly FirestoreTheaterRoomService _firestoreTheaterRoomService;
        private readonly FirestoreCinemaLocationService _firestoreCinemaLocationService;
        private readonly ILogger<ScreeningsController> _logger;

        public ScreeningsController(
            FirestoreScreeningService firestoreScreeningService,
            FirestoreMovieService firestoreMovieService,
            FirestoreTheaterRoomService firestoreTheaterRoomService,
            FirestoreCinemaLocationService firestoreCinemaLocationService,
            ILogger<ScreeningsController> logger)
        {
            _firestoreScreeningService = firestoreScreeningService;
            _firestoreMovieService = firestoreMovieService;
            _firestoreTheaterRoomService = firestoreTheaterRoomService;
            _firestoreCinemaLocationService = firestoreCinemaLocationService;
            _logger = logger;
        }

        /// <summary>
        /// Get metadata needed to create a screening (cinemas, rooms, movies)
        /// GET /api/screenings/creation-data
        /// </summary>
        [HttpGet("creation-data")]
        public async Task<IActionResult> GetCreationData()
        {
            try
            {
                var cinemas = await _firestoreCinemaLocationService.GetAllCinemaLocationsAsync();
                var rooms = await _firestoreTheaterRoomService.GetAllTheaterRoomsAsync();
                var movies = await _firestoreMovieService.GetAllMoviesAsync();

                return Ok(new
                {
                    success = true,
                    cinemas = cinemas.Select(c => new { c.Id, c.Name, c.Address }).ToList(),
                    rooms = rooms.Select(r => new { r.Id, r.Name, r.CinemaId, r.Capacity }).ToList(),
                    movies = movies.Select(m => new { m.Id, m.Title, m.Genre, m.DurationMinutes }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting creation data");
                return StatusCode(500, new { success = false, message = "Error getting creation data", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new screening with validation
        /// POST /api/screenings/add-screening
        /// </summary>
        [HttpPost("add-screening")]
        public async Task<IActionResult> AddScreening([FromBody] Screening screening)
        {
            try
            {
                // Validations
                var errors = new List<string>();

                if (string.IsNullOrEmpty(screening.MovieId))
                    errors.Add("MovieId is required");

                if (string.IsNullOrEmpty(screening.CinemaId))
                    errors.Add("CinemaId is required");

                if (string.IsNullOrEmpty(screening.TheaterRoomId))
                    errors.Add("TheaterRoomId is required");

                if (screening.StartTime == default || screening.StartTime < DateTime.UtcNow)
                    errors.Add("StartTime must be in the future");

                if (screening.EndTime == default || screening.EndTime <= screening.StartTime)
                    errors.Add("EndTime must be after StartTime");

                if (screening.Price <= 0)
                    errors.Add("Price must be greater than 0");

                if (errors.Any())
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Validation failed",
                        errors = errors,
                        receivedData = new
                        {
                            screening.MovieId,
                            screening.CinemaId,
                            screening.TheaterRoomId,
                            screening.StartTime,
                            screening.EndTime,
                            screening.Price
                        }
                    });
                }

                // Verify entities exist
                var movie = await _firestoreMovieService.GetMovieAsync(screening.MovieId);
                if (movie == null)
                    return NotFound(new { success = false, message = $"Movie with ID '{screening.MovieId}' not found" });

                var cinema = await _firestoreCinemaLocationService.GetCinemaLocationAsync(screening.CinemaId);
                if (cinema == null)
                    return NotFound(new { success = false, message = $"Cinema with ID '{screening.CinemaId}' not found" });

                var room = await _firestoreTheaterRoomService.GetTheaterRoomAsync(screening.TheaterRoomId);
                if (room == null)
                    return NotFound(new { success = false, message = $"Theater room with ID '{screening.TheaterRoomId}' not found" });

                // Verify room belongs to cinema
                if (room.CinemaId != screening.CinemaId)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Theater room '{room.Name}' does not belong to cinema '{cinema.Name}'",
                        roomCinemaId = room.CinemaId,
                        providedCinemaId = screening.CinemaId
                    });
                }

                await _firestoreScreeningService.AddScreeningAsync(screening);

                _logger.LogInformation($"✅ Screening created: {screening.Id} for movie {movie.Title} at {cinema.Name} - {room.Name}");

                return Ok(new
                {
                    success = true,
                    id = screening.Id,
                    screening = new
                    {
                        screening.Id,
                        screening.MovieId,
                        MovieTitle = movie.Title,
                        screening.CinemaId,
                        CinemaName = cinema.Name,
                        screening.TheaterRoomId,
                        RoomName = room.Name,
                        screening.StartTime,
                        screening.EndTime,
                        screening.Price
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating screening");
                return StatusCode(500, new { success = false, message = "Error creating screening", error = ex.Message });
            }
        }

        [HttpGet("get-screening/{id}")]
        public async Task<IActionResult> GetScreening(string id)
        {
            var screening = await _firestoreScreeningService.GetScreeningAsync(id);
            if (screening == null)
                return NotFound(new { success = false, message = "Screening not found." });

            return Ok(new { success = true, screening });
        }

        /// <summary>
        /// Delete a screening with validation
        /// DELETE /api/screenings/delete-screening/{id}
        /// </summary>
        [HttpDelete("delete-screening/{id}")]
        public async Task<IActionResult> DeleteScreening(string id)
        {
            try
            {
                var screening = await _firestoreScreeningService.GetScreeningAsync(id);
                if (screening == null)
                    return NotFound(new { success = false, message = $"Screening with ID '{id}' not found" });

                await _firestoreScreeningService.DeleteScreeningAsync(id);

                _logger.LogInformation($"✅ Screening deleted: {id}");

                return Ok(new { success = true, message = $"Screening {id} deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting screening {id}");
                return StatusCode(500, new { success = false, message = "Error deleting screening", error = ex.Message });
            }
        }

        /// <summary>
        /// Update a screening with validation
        /// PUT /api/screenings/edit-screening/{id}
        /// </summary>
        [HttpPut("edit-screening/{id}")]
        public async Task<IActionResult> EditScreening(string id, [FromBody] Screening screening)
        {
            try
            {
                var existing = await _firestoreScreeningService.GetScreeningAsync(id);
                if (existing == null)
                    return NotFound(new { success = false, message = $"Screening with ID '{id}' not found" });

                screening.Id = id;

                // Validations
                var errors = new List<string>();

                if (string.IsNullOrEmpty(screening.MovieId))
                    errors.Add("MovieId is required");

                if (string.IsNullOrEmpty(screening.CinemaId))
                    errors.Add("CinemaId is required");

                if (string.IsNullOrEmpty(screening.TheaterRoomId))
                    errors.Add("TheaterRoomId is required");

                if (screening.StartTime == default)
                    errors.Add("StartTime is required");

                if (screening.EndTime == default || screening.EndTime <= screening.StartTime)
                    errors.Add("EndTime must be after StartTime");

                if (screening.Price <= 0)
                    errors.Add("Price must be greater than 0");

                if (errors.Any())
                {
                    return BadRequest(new { success = false, message = "Validation failed", errors });
                }

                await _firestoreScreeningService.UpdateScreeningAsync(screening);

                _logger.LogInformation($"✅ Screening updated: {id}");

                return Ok(new { success = true, screening });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating screening {id}");
                return StatusCode(500, new { success = false, message = "Error updating screening", error = ex.Message });
            }
        }

        [HttpGet("get-all-screenings")]
        [Obsolete("Use GET /api/screenings/paginated instead. This endpoint loads ALL screenings without limit.")]
        public async Task<IActionResult> GetAllScreenings()
        {
            var screenings = await _firestoreScreeningService.GetAllScreeningsAsync();
            return Ok(new { success = true, screenings, warning = "DEPRECATED: Use /api/screenings/paginated for better performance" });
        }

        /// <summary>
        /// Get screenings with pagination (RECOMMENDED)
        /// GET /api/screenings/paginated?pageNumber=1&pageSize=50
        /// </summary>
        [HttpGet("paginated")]
        public async Task<IActionResult> GetScreeningsPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
        {
            var result = await _firestoreScreeningService.GetScreeningsPaginatedAsync(pageNumber, pageSize);
            return Ok(new
            {
                success = true,
                data = result.Items,
                pagination = new
                {
                    result.TotalCount,
                    result.PageNumber,
                    result.PageSize,
                    result.TotalPages,
                    result.HasPreviousPage,
                    result.HasNextPage
                }
            });
        }

        /// <summary>
        /// Get only future screenings (most common use case)
        /// GET /api/screenings/future?limit=50
        /// </summary>
        [HttpGet("future")]
        public async Task<IActionResult> GetFutureScreenings([FromQuery] int limit = 50)
        {
            var screenings = await _firestoreScreeningService.GetFutureScreeningsAsync(limit);
            return Ok(new
            {
                success = true,
                screenings,
                count = screenings.Count,
                limit
            });
        }

        /// <summary>
        /// Get screenings by movie ID with limit
        /// GET /api/screenings/by-movie/{movieId}?limit=50
        /// </summary>
        [HttpGet("by-movie/{movieId}")]
        public async Task<IActionResult> GetScreeningsByMovie(string movieId, [FromQuery] int limit = 50)
        {
            var screenings = await _firestoreScreeningService.GetScreeningsByMovieIdAsync(movieId, limit);
            return Ok(new
            {
                success = true,
                screenings,
                count = screenings.Count,
                movieId,
                limit
            });
        }

        /// <summary>
        /// Get screenings by cinema ID with limit
        /// GET /api/screenings/by-cinema/{cinemaId}?limit=50
        /// </summary>
        [HttpGet("by-cinema/{cinemaId}")]
        public async Task<IActionResult> GetScreeningsByCinema(string cinemaId, [FromQuery] int limit = 50)
        {
            var screenings = await _firestoreScreeningService.GetScreeningsByCinemaIdAsync(cinemaId, limit);
            return Ok(new
            {
                success = true,
                screenings,
                count = screenings.Count,
                cinemaId,
                limit
            });
        }

        /// <summary>
        /// Clear all existing screenings (for testing).
        /// DELETE /api/screenings/clear-all
        /// </summary>
        [HttpDelete("clear-all")]
        public async Task<IActionResult> ClearAllScreenings()
        {
            try
            {
                int deletedCount = await _firestoreScreeningService.DeleteAllScreeningsAsync();

                return Ok(new
                {
                    success = true,
                    message = $"✅ Deleted {deletedCount} screenings using batch operation",
                    count = deletedCount
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"❌ Error deleting screenings: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Seed screenings for testing purposes.
        /// Creates screenings ONLY for movies that are "En Cartelera" (isNew = true) or "Más Populares" (top 8 by rating).
        /// Movies "Próximamente" (isNew = false and not popular) will NOT get screenings - admin must add them manually.
        /// POST /api/screenings/seed?clearExisting=false
        /// </summary>
        [HttpPost("seed")]
        public async Task<IActionResult> SeedScreenings([FromQuery] bool clearExisting = false)
        {
            // Clear existing screenings if requested
            if (clearExisting)
            {
                await _firestoreScreeningService.DeleteAllScreeningsAsync();
            }

            var screenings = new List<Screening>();
            var baseDate = DateTime.UtcNow.Date;

            // Get all movies from Firestore to filter properly
            var allMovies = await _firestoreMovieService.GetAllMoviesAsync();

            // Filter movies that should have screenings:
            // 1. "En Cartelera" - movies with isNew = true
            // 2. "Más Populares" - top 8 movies by rating
            var nowPlayingMovies = allMovies.Where(m => m.IsNew == true).ToList();
            var popularMovies = allMovies.OrderByDescending(m => m.Rating).Take(8).ToList();

            // Combine and get unique movie IDs
            var moviesWithScreenings = nowPlayingMovies.Union(popularMovies).DistinctBy(m => m.Id).ToList();

            if (!moviesWithScreenings.Any())
            {
                return Ok(new
                {
                    success = true,
                    message = "No movies found to create screenings. Make sure you have movies with isNew=true or high ratings.",
                    count = 0
                });
            }

            var movieIds = moviesWithScreenings.Select(m => m.Id).ToArray();

            // Get all theater rooms and cinemas from Firestore
            var allTheaterRooms = await _firestoreTheaterRoomService.GetAllTheaterRoomsAsync();

            if (!allTheaterRooms.Any())
            {
                return BadRequest(new
                {
                    success = false,
                    message = "No theater rooms found. Please seed theater rooms first using POST /api/theaterrooms/seed"
                });
            }

            // Get all cinemas
            var allCinemas = await _firestoreCinemaLocationService.GetAllCinemaLocationsAsync();
            if (!allCinemas.Any())
            {
                return BadRequest(new
                {
                    success = false,
                    message = "No cinemas found. Please seed cinemas first."
                });
            }

            // Group theater rooms by cinema (filter out rooms without CinemaId)
            var roomsByCinema = allTheaterRooms
                .Where(r => !string.IsNullOrEmpty(r.CinemaId))
                .GroupBy(r => r.CinemaId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Horarios típicos de cine
            var showtimes = new[]
            {
                new { hour = 14, minute = 0 },  // 2:00 PM
                new { hour = 17, minute = 30 },  // 5:30 PM
                new { hour = 21, minute = 0 },   // 9:00 PM
                new { hour = 23, minute = 30 }   // 11:30 PM
            };

            int screeningCounter = 1;

            // REALISMO: Cada cine tiene un subconjunto diferente de películas (60-80%)
            var moviesByCinema = new Dictionary<string, List<string>>();

            foreach (var cinema in allCinemas)
            {
                // Cada cine tendrá entre 60% y 80% de las películas disponibles
                var percentage = Random.Shared.Next(60, 81) / 100.0;
                var movieCount = Math.Max(1, (int)(movieIds.Length * percentage));

                // Seleccionar películas aleatorias para este cine
                var cinemaMovies = movieIds.OrderBy(x => Guid.NewGuid()).Take(movieCount).ToList();
                moviesByCinema[cinema.Id] = cinemaMovies;
            }

            // Crear screenings para los próximos 7 días, por cine
            foreach (var cinema in allCinemas)
            {
                if (!roomsByCinema.ContainsKey(cinema.Id) || !roomsByCinema[cinema.Id].Any())
                    continue;

                var cinemaRooms = roomsByCinema[cinema.Id];
                var cinemaMovies = moviesByCinema[cinema.Id];

                for (int day = 0; day < 7; day++)
                {
                    var screeningDate = baseDate.AddDays(day);

                    foreach (var movieId in cinemaMovies)
                    {
                        // Cada película tiene 2-3 funciones al día en diferentes salas
                        var showtimesToday = showtimes.OrderBy(x => Guid.NewGuid()).Take(Random.Shared.Next(2, 4)).ToArray();

                        foreach (var showtime in showtimesToday)
                        {
                            var startTime = screeningDate.AddHours(showtime.hour).AddMinutes(showtime.minute);
                            var endTime = startTime.AddMinutes(120); // Duración promedio de 2 horas

                            // Seleccionar sala al azar dentro de este cine
                            var room = cinemaRooms[Random.Shared.Next(cinemaRooms.Count)];

                            // Precios típicos de cine en Costa Rica (en colones)
                            var prices = new double[] { 3500, 4000, 4500, 5000, 5500 };
                            var price = prices[Random.Shared.Next(prices.Length)];

                            var screening = new Screening
                            {
                                Id = $"SCR-{screeningCounter:D4}",
                                MovieId = movieId,
                                CinemaId = cinema.Id,
                                TheaterRoomId = room.Id,
                                StartTime = startTime,
                                EndTime = endTime,
                                Price = price
                            };

                            screenings.Add(screening);
                            await _firestoreScreeningService.AddScreeningAsync(screening);
                            screeningCounter++;
                        }
                    }
                }
            }

            return Ok(new
            {
                success = true,
                message = $"Created {screenings.Count} screenings for the next 7 days with realistic distribution",
                count = screenings.Count,
                cinemasCount = allCinemas.Count,
                moviesWithScreenings = moviesWithScreenings.Select(m => new
                {
                    m.Id,
                    m.Title,
                    IsNowPlaying = m.IsNew,
                    m.Rating
                }).ToList(),
                movieDistributionByCinema = allCinemas.Select(c => new
                {
                    CinemaId = c.Id,
                    CinemaName = c.Name,
                    MoviesCount = moviesByCinema.ContainsKey(c.Id) ? moviesByCinema[c.Id].Count : 0,
                    Percentage = moviesByCinema.ContainsKey(c.Id)
                        ? Math.Round((moviesByCinema[c.Id].Count / (double)movieIds.Length) * 100, 0)
                        : 0,
                    Movies = moviesByCinema.ContainsKey(c.Id)
                        ? moviesByCinema[c.Id].Select(mid => moviesWithScreenings.FirstOrDefault(m => m.Id == mid)?.Title ?? mid).Take(5).ToList()
                        : new List<string>()
                }).ToList(),
                nowPlayingCount = nowPlayingMovies.Count,
                popularCount = popularMovies.Count,
                totalMoviesWithScreenings = moviesWithScreenings.Count,
                note = "🎬 REALISMO: Cada cine tiene entre 60-80% de las películas disponibles, distribuidas aleatoriamente. Esto simula la realidad donde no todos los cines tienen todas las películas.",
                sampleScreenings = screenings.Take(5).Select(s => new
                {
                    s.Id,
                    s.CinemaId,
                    CinemaName = allCinemas.FirstOrDefault(c => c.Id == s.CinemaId)?.Name,
                    s.MovieId,
                    MovieTitle = moviesWithScreenings.FirstOrDefault(m => m.Id == s.MovieId)?.Title,
                    s.TheaterRoomId,
                    StartTime = s.StartTime.ToString("yyyy-MM-dd HH:mm"),
                    EndTime = s.EndTime.ToString("yyyy-MM-dd HH:mm")
                })
            });
        }
    }
}
