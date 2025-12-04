using Microsoft.AspNetCore.Mvc;
using Cinema.Api.Services;
using Cinema.Domain.Entities;
using Serilog;

namespace Cinema.Api.Controllers
{
    [ApiController]
    [Route("api/minimal-seed")]
    public class MinimalSeedController : ControllerBase
    {
        private readonly FirestoreScreeningService _screeningService;
        private readonly FirestoreMovieService _movieService;
        private readonly FirestoreTheaterRoomService _theaterRoomService;
        private readonly FirestoreCinemaLocationService _cinemaLocationService;

        public MinimalSeedController(
            FirestoreScreeningService screeningService,
            FirestoreMovieService movieService,
            FirestoreTheaterRoomService theaterRoomService,
            FirestoreCinemaLocationService cinemaLocationService)
        {
            _screeningService = screeningService;
            _movieService = movieService;
            _theaterRoomService = theaterRoomService;
            _cinemaLocationService = cinemaLocationService;
        }

        /// <summary>
        /// Creates MINIMAL screenings for testing: 2 screenings per cinema for TODAY only.
        /// Uses all available movies distributed evenly across cinemas.
        /// POST /api/minimal-seed/create-today-screenings
        /// </summary>
        [HttpPost("create-today-screenings")]
        public async Task<IActionResult> CreateTodayScreenings()
        {
            try
            {
                Log.Information("🎬 Starting minimal seed for today's screenings...");

                var screenings = new List<Screening>();
                var today = DateTime.UtcNow.Date;

                // Get all required data
                var allMovies = await _movieService.GetAllMoviesAsync();
                var allCinemas = await _cinemaLocationService.GetAllCinemaLocationsAsync();
                var allTheaterRooms = await _theaterRoomService.GetAllTheaterRoomsAsync();

                if (!allMovies.Any())
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No movies found. Please seed movies first."
                    });
                }

                if (!allCinemas.Any())
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No cinemas found. Please create cinemas first."
                    });
                }

                if (!allTheaterRooms.Any())
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No theater rooms found. Please create theater rooms first."
                    });
                }

                // Filter only "En Cartelera" movies (isNew = true) or top rated
                var activeMovies = allMovies
                    .Where(m => m.IsNew == true || m.Rating >= 7.5)
                    .OrderByDescending(m => m.Rating)
                    .ToList();

                if (!activeMovies.Any())
                {
                    activeMovies = allMovies.OrderByDescending(m => m.Rating).Take(5).ToList();
                }

                Log.Information("Found {MovieCount} active movies to schedule", activeMovies.Count);

                // Group theater rooms by cinema
                var roomsByCinema = allTheaterRooms
                    .Where(r => !string.IsNullOrEmpty(r.CinemaId))
                    .GroupBy(r => r.CinemaId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Only 2 showtimes for today (afternoon and evening)
                var showtimes = new[]
                {
                    new { hour = 17, minute = 30 },  // 5:30 PM
                    new { hour = 21, minute = 0 }    // 9:00 PM
                };

                int screeningCounter = 1;
                var movieIndex = 0;

                // Create exactly 2 screenings per cinema
                foreach (var cinema in allCinemas)
                {
                    if (!roomsByCinema.ContainsKey(cinema.Id) || !roomsByCinema[cinema.Id].Any())
                    {
                        Log.Warning("Cinema {CinemaId} has no rooms, skipping", cinema.Id);
                        continue;
                    }

                    var cinemaRooms = roomsByCinema[cinema.Id];

                    // Create 2 screenings (one per showtime)
                    foreach (var showtime in showtimes)
                    {
                        // Rotate through movies to distribute them evenly
                        var movie = activeMovies[movieIndex % activeMovies.Count];
                        movieIndex++;

                        var startTime = today.AddHours(showtime.hour).AddMinutes(showtime.minute);
                        var endTime = startTime.AddMinutes(120); // 2 hours duration

                        // Select a random room for this cinema
                        var room = cinemaRooms[Random.Shared.Next(cinemaRooms.Count)];

                        // Standard pricing in Costa Rican colones
                        var price = 4500.0; // Fixed price for simplicity

                        var screening = new Screening
                        {
                            Id = $"SCR-MIN-{screeningCounter:D4}",
                            MovieId = movie.Id,
                            CinemaId = cinema.Id,
                            TheaterRoomId = room.Id,
                            StartTime = startTime,
                            EndTime = endTime,
                            Price = price
                        };

                        screenings.Add(screening);
                        await _screeningService.AddScreeningAsync(screening);
                        screeningCounter++;

                        Log.Information("Created screening {Id} for {MovieTitle} at {CinemaName} ({Time})",
                            screening.Id, movie.Title, cinema.Name, startTime.ToString("HH:mm"));
                    }
                }

                Log.Information("✅ Minimal seed completed: {Count} screenings created", screenings.Count);

                return Ok(new
                {
                    success = true,
                    message = $"Created {screenings.Count} minimal screenings for today",
                    statistics = new
                    {
                        totalScreenings = screenings.Count,
                        cinemasCount = allCinemas.Count,
                        moviesUsed = activeMovies.Count,
                        screeningsPerCinema = 2,
                        date = today.ToString("yyyy-MM-dd")
                    },
                    screenings = screenings.Select(s => new
                    {
                        s.Id,
                        s.CinemaId,
                        CinemaName = allCinemas.FirstOrDefault(c => c.Id == s.CinemaId)?.Name,
                        s.MovieId,
                        MovieTitle = activeMovies.FirstOrDefault(m => m.Id == s.MovieId)?.Title,
                        s.TheaterRoomId,
                        StartTime = s.StartTime.ToString("yyyy-MM-dd HH:mm"),
                        EndTime = s.EndTime.ToString("yyyy-MM-dd HH:mm"),
                        s.Price
                    }).ToList(),
                    note = "🎯 MINIMAL SEED: Only 2 screenings per cinema for TODAY. Use this for daily operations and testing."
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating minimal screenings");
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error creating screenings: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Creates screenings for demo day with realistic data:
        /// - Past screenings (completed)
        /// - Current screenings (in progress)
        /// - Future screenings (scheduled)
        /// POST /api/minimal-seed/create-demo-screenings
        /// </summary>
        [HttpPost("create-demo-screenings")]
        public async Task<IActionResult> CreateDemoScreenings()
        {
            try
            {
                Log.Information("🎭 Starting DEMO seed with past/current/future screenings...");

                var screenings = new List<Screening>();
                var now = DateTime.UtcNow;
                var today = now.Date;

                // Get all required data
                var allMovies = await _movieService.GetAllMoviesAsync();
                var allCinemas = await _cinemaLocationService.GetAllCinemaLocationsAsync();
                var allTheaterRooms = await _theaterRoomService.GetAllTheaterRoomsAsync();

                if (!allMovies.Any() || !allCinemas.Any() || !allTheaterRooms.Any())
                {
                    return BadRequest(new { success = false, message = "Missing required data (movies/cinemas/rooms)" });
                }

                var activeMovies = allMovies
                    .Where(m => m.IsNew == true || m.Rating >= 7.5)
                    .OrderByDescending(m => m.Rating)
                    .ToList();

                var roomsByCinema = allTheaterRooms
                    .Where(r => !string.IsNullOrEmpty(r.CinemaId))
                    .GroupBy(r => r.CinemaId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                int screeningCounter = 1;

                // DEMO DISTRIBUTION:
                // - 2 past screenings per cinema (yesterday)
                // - 1 current screening per cinema (happening now)
                // - 3 future screenings per cinema (today evening + tomorrow)

                foreach (var cinema in allCinemas)
                {
                    if (!roomsByCinema.ContainsKey(cinema.Id)) continue;
                    var cinemaRooms = roomsByCinema[cinema.Id];

                    // PAST SCREENINGS (Yesterday - Completed)
                    for (int i = 0; i < 2; i++)
                    {
                        var movie = activeMovies[screeningCounter % activeMovies.Count];
                        var room = cinemaRooms[Random.Shared.Next(cinemaRooms.Count)];
                        var startTime = today.AddDays(-1).AddHours(14 + (i * 4)); // 2 PM and 6 PM yesterday

                        screenings.Add(new Screening
                        {
                            Id = $"SCR-DEMO-{screeningCounter:D4}",
                            MovieId = movie.Id,
                            CinemaId = cinema.Id,
                            TheaterRoomId = room.Id,
                            StartTime = startTime,
                            EndTime = startTime.AddMinutes(120),
                            Price = 4500.0
                        });
                        await _screeningService.AddScreeningAsync(screenings.Last());
                        screeningCounter++;
                    }

                    // CURRENT SCREENING (Happening NOW)
                    {
                        var movie = activeMovies[screeningCounter % activeMovies.Count];
                        var room = cinemaRooms[Random.Shared.Next(cinemaRooms.Count)];
                        var startTime = now.AddMinutes(-30); // Started 30 minutes ago

                        screenings.Add(new Screening
                        {
                            Id = $"SCR-DEMO-{screeningCounter:D4}",
                            MovieId = movie.Id,
                            CinemaId = cinema.Id,
                            TheaterRoomId = room.Id,
                            StartTime = startTime,
                            EndTime = startTime.AddMinutes(120),
                            Price = 4500.0
                        });
                        await _screeningService.AddScreeningAsync(screenings.Last());
                        screeningCounter++;
                    }

                    // FUTURE SCREENINGS (Today evening + Tomorrow)
                    var futureShowtimes = new[]
                    {
                        today.AddHours(21),           // Today 9 PM
                        today.AddDays(1).AddHours(14), // Tomorrow 2 PM
                        today.AddDays(1).AddHours(19)  // Tomorrow 7 PM
                    };

                    foreach (var startTime in futureShowtimes)
                    {
                        var movie = activeMovies[screeningCounter % activeMovies.Count];
                        var room = cinemaRooms[Random.Shared.Next(cinemaRooms.Count)];

                        screenings.Add(new Screening
                        {
                            Id = $"SCR-DEMO-{screeningCounter:D4}",
                            MovieId = movie.Id,
                            CinemaId = cinema.Id,
                            TheaterRoomId = room.Id,
                            StartTime = startTime,
                            EndTime = startTime.AddMinutes(120),
                            Price = 4500.0
                        });
                        await _screeningService.AddScreeningAsync(screenings.Last());
                        screeningCounter++;
                    }
                }

                var pastCount = screenings.Count(s => s.EndTime < now);
                var currentCount = screenings.Count(s => s.StartTime <= now && s.EndTime >= now);
                var futureCount = screenings.Count(s => s.StartTime > now);

                Log.Information("✅ DEMO seed completed: {Total} screenings ({Past} past, {Current} current, {Future} future)",
                    screenings.Count, pastCount, currentCount, futureCount);

                return Ok(new
                {
                    success = true,
                    message = $"Created {screenings.Count} demo screenings with realistic distribution",
                    statistics = new
                    {
                        totalScreenings = screenings.Count,
                        pastScreenings = pastCount,
                        currentScreenings = currentCount,
                        futureScreenings = futureCount,
                        cinemasCount = allCinemas.Count,
                        screeningsPerCinema = 6
                    },
                    note = "🎭 DEMO MODE: Includes past (completed), current (in progress), and future (scheduled) screenings for realistic demo"
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating demo screenings");
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Creates 2 screenings PER MOVIE PER CINEMA for optimal distribution.
        /// Limits to top N movies to avoid excessive screenings.
        /// POST /api/minimal-seed/create-optimized-screenings?maxMovies=5
        /// </summary>
        [HttpPost("create-optimized-screenings")]
        public async Task<IActionResult> CreateOptimizedScreenings([FromQuery] int maxMovies = 5)
        {
            try
            {
                Log.Information("🎯 Starting OPTIMIZED seed: 2 screenings per movie per cinema (max {MaxMovies} movies)", maxMovies);

                var screenings = new List<Screening>();
                var today = DateTime.UtcNow.Date;

                // Get all required data
                var allMovies = await _movieService.GetAllMoviesAsync();
                var allCinemas = await _cinemaLocationService.GetAllCinemaLocationsAsync();
                var allTheaterRooms = await _theaterRoomService.GetAllTheaterRoomsAsync();

                if (!allMovies.Any() || !allCinemas.Any() || !allTheaterRooms.Any())
                {
                    return BadRequest(new { success = false, message = "Missing required data (movies/cinemas/rooms)" });
                }

                // Filter and limit movies: Top N movies (isNew=true or high rating)
                var selectedMovies = allMovies
                    .Where(m => m.IsNew == true || m.Rating >= 7.5)
                    .OrderByDescending(m => m.IsNew)  // Prioritize "En Cartelera"
                    .ThenByDescending(m => m.Rating)
                    .Take(maxMovies)
                    .ToList();

                if (!selectedMovies.Any())
                {
                    selectedMovies = allMovies.OrderByDescending(m => m.Rating).Take(maxMovies).ToList();
                }

                Log.Information("Selected {Count} top movies for scheduling", selectedMovies.Count);

                // Group theater rooms by cinema
                var roomsByCinema = allTheaterRooms
                    .Where(r => !string.IsNullOrEmpty(r.CinemaId))
                    .GroupBy(r => r.CinemaId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // 2 showtimes per movie per cinema
                var showtimes = new[]
                {
                    new { hour = 17, minute = 30 },  // 5:30 PM
                    new { hour = 21, minute = 0 }    // 9:00 PM
                };

                int screeningCounter = 1;

                // For each cinema, create 2 screenings per movie
                foreach (var cinema in allCinemas)
                {
                    if (!roomsByCinema.ContainsKey(cinema.Id) || !roomsByCinema[cinema.Id].Any())
                    {
                        Log.Warning("Cinema {CinemaId} has no rooms, skipping", cinema.Id);
                        continue;
                    }

                    var cinemaRooms = roomsByCinema[cinema.Id];

                    // For each movie
                    foreach (var movie in selectedMovies)
                    {
                        // Create 2 screenings (5:30 PM and 9:00 PM)
                        foreach (var showtime in showtimes)
                        {
                            var startTime = today.AddHours(showtime.hour).AddMinutes(showtime.minute);
                            var endTime = startTime.AddMinutes(120); // 2 hours duration

                            // Select a random room for this cinema
                            var room = cinemaRooms[Random.Shared.Next(cinemaRooms.Count)];

                            var screening = new Screening
                            {
                                Id = $"SCR-OPT-{screeningCounter:D4}",
                                MovieId = movie.Id,
                                CinemaId = cinema.Id,
                                TheaterRoomId = room.Id,
                                StartTime = startTime,
                                EndTime = endTime,
                                Price = 4500.0
                            };

                            screenings.Add(screening);
                            await _screeningService.AddScreeningAsync(screening);
                            screeningCounter++;

                            Log.Information("Created screening {Id} for {MovieTitle} at {CinemaName} ({Time})",
                                screening.Id, movie.Title, cinema.Name, startTime.ToString("HH:mm"));
                        }
                    }
                }

                var totalScreeningsExpected = allCinemas.Count * selectedMovies.Count * 2;

                Log.Information("✅ OPTIMIZED seed completed: {Count} screenings created ({Movies} movies × {Cinemas} cinemas × 2 showtimes)",
                    screenings.Count, selectedMovies.Count, allCinemas.Count);

                return Ok(new
                {
                    success = true,
                    message = $"Created {screenings.Count} optimized screenings",
                    statistics = new
                    {
                        totalScreenings = screenings.Count,
                        expectedScreenings = totalScreeningsExpected,
                        cinemasCount = allCinemas.Count,
                        moviesCount = selectedMovies.Count,
                        screeningsPerMovie = screenings.GroupBy(s => s.MovieId).Select(g => new { movieId = g.Key, count = g.Count() }).ToList(),
                        screeningsPerCinema = screenings.GroupBy(s => s.CinemaId).Select(g => new { cinemaId = g.Key, count = g.Count() }).ToList(),
                        date = today.ToString("yyyy-MM-dd")
                    },
                    movies = selectedMovies.Select(m => new
                    {
                        m.Id,
                        m.Title,
                        m.Rating,
                        m.IsNew,
                        screeningsCreated = screenings.Count(s => s.MovieId == m.Id)
                    }).ToList(),
                    note = $"🎯 OPTIMIZED: 2 screenings per movie per cinema. Limited to top {maxMovies} movies to minimize Firestore reads."
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating optimized screenings");
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error: {ex.Message}"
                });
            }
        }
    }
}
