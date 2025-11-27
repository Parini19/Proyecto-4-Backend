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

        public ScreeningsController(
            FirestoreScreeningService firestoreScreeningService,
            FirestoreMovieService firestoreMovieService,
            FirestoreTheaterRoomService firestoreTheaterRoomService,
            FirestoreCinemaLocationService firestoreCinemaLocationService)
        {
            _firestoreScreeningService = firestoreScreeningService;
            _firestoreMovieService = firestoreMovieService;
            _firestoreTheaterRoomService = firestoreTheaterRoomService;
            _firestoreCinemaLocationService = firestoreCinemaLocationService;
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

        /// <summary>
        /// Clear all existing screenings (for testing).
        /// DELETE /api/screenings/clear-all
        /// </summary>
        [HttpDelete("clear-all")]
        public async Task<IActionResult> ClearAllScreenings()
        {
            try
            {
                var existingScreenings = await _firestoreScreeningService.GetAllScreeningsAsync();
                int deletedCount = 0;

                foreach (var screening in existingScreenings)
                {
                    await _firestoreScreeningService.DeleteScreeningAsync(screening.Id);
                    deletedCount++;
                }

                return Ok(new
                {
                    success = true,
                    message = $"Deleted {deletedCount} screenings",
                    count = deletedCount
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Error deleting screenings: {ex.Message}"
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
                var existingScreenings = await _firestoreScreeningService.GetAllScreeningsAsync();
                foreach (var screening in existingScreenings)
                {
                    await _firestoreScreeningService.DeleteScreeningAsync(screening.Id);
                }
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