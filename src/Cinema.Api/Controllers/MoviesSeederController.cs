using Cinema.Api.Services;
using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers
{
    /// <summary>
    /// Controlador temporal para sembrar datos de películas en Firestore.
    /// ELIMINAR DESPUÉS DE USAR EN PRODUCCIÓN.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesSeederController : ControllerBase
    {
        private readonly FirestoreMovieService _movieService;
        private readonly ILogger<MoviesSeederController> _logger;

        public MoviesSeederController(
            FirestoreMovieService movieService,
            ILogger<MoviesSeederController> logger)
        {
            _movieService = movieService;
            _logger = logger;
        }

        /// <summary>
        /// Inserta todas las películas de la cartelera en Firestore.
        /// POST /api/moviesseeder/seed
        /// </summary>
        [HttpPost("seed")]
        public async Task<IActionResult> SeedMovies()
        {
            try
            {
                _logger.LogInformation("Iniciando seed de películas...");

                var movies = GetMoviesData();

                int count = 0;
                foreach (var movie in movies)
                {
                    await _movieService.AddMovieAsync(movie);
                    count++;
                    _logger.LogInformation($"Película agregada: {movie.Title}");
                }

                return Ok(new
                {
                    success = true,
                    message = $"Se insertaron {count} películas exitosamente",
                    count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al sembrar películas");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al sembrar películas",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Elimina todas las películas y vuelve a sembrar.
        /// POST /api/moviesseeder/reseed
        /// </summary>
        [HttpPost("reseed")]
        public async Task<IActionResult> ReseedMovies()
        {
            try
            {
                _logger.LogInformation("Eliminando películas existentes...");

                // Obtener todas las películas
                var existingMovies = await _movieService.GetAllMoviesAsync();

                // Eliminar cada una
                foreach (var movie in existingMovies)
                {
                    await _movieService.DeleteMovieAsync(movie.Id);
                }

                _logger.LogInformation($"Eliminadas {existingMovies.Count} películas");

                // Sembrar de nuevo
                return await SeedMovies();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al resembrar películas");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al resembrar películas",
                    error = ex.Message
                });
            }
        }

        private List<Movie> GetMoviesData()
        {
            return new List<Movie>
            {
                // EN CARTELERA (nowPlaying) - IDs 1-8
                new Movie
                {
                    Id = "1",
                    Title = "Dune: Part Two",
                    Description = "Paul Atreides se une a Chani y los Fremen mientras busca venganza contra los conspiradores que destruyeron a su familia.",
                    Year = 2024,
                    DurationMinutes = 166,
                    Genre = "Ciencia Ficción, Aventura",
                    Director = "Denis Villeneuve",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133756/Dune_part_2_x4ymwt.jpg",
                    Rating = 8.8,
                    Classification = "PG-13",
                    IsNew = true,
                    Showtimes = new List<string> { "14:00", "17:30", "21:00" }
                },
                new Movie
                {
                    Id = "2",
                    Title = "Kung Fu Panda 4",
                    Description = "Po debe entrenar a un nuevo guerrero cuando se le pide que se convierta en el Líder Espiritual del Valle de la Paz.",
                    Year = 2024,
                    DurationMinutes = 94,
                    Genre = "Animación, Comedia, Acción",
                    Director = "Mike Mitchell",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133759/Kung_fu_panda_4_jgum2c.jpg",
                    Rating = 7.2,
                    Classification = "PG",
                    IsNew = true,
                    Showtimes = new List<string> { "13:00", "15:30", "18:00", "20:30" }
                },
                new Movie
                {
                    Id = "3",
                    Title = "Godzilla x Kong: The New Empire",
                    Description = "Los dos titanes más poderosos chocan en una batalla épica mientras la humanidad lucha por su supervivencia.",
                    Year = 2024,
                    DurationMinutes = 115,
                    Genre = "Acción, Ciencia Ficción",
                    Director = "Adam Wingard",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133758/godzilla_x_kong_the_new_empire_b9b8og.jpg",
                    Rating = 7.5,
                    Classification = "PG-13",
                    IsNew = true,
                    Showtimes = new List<string> { "15:00", "18:30", "21:30" }
                },
                new Movie
                {
                    Id = "4",
                    Title = "Ghostbusters: Frozen Empire",
                    Description = "La familia Spengler regresa a Nueva York para salvar al mundo de una nueva era de hielo.",
                    Year = 2024,
                    DurationMinutes = 115,
                    Genre = "Comedia, Fantasía, Acción",
                    Director = "Gil Kenan",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133757/Ghostbusters_Frozen_Empire_icvjpc.jpg",
                    Rating = 6.8,
                    Classification = "PG-13",
                    IsNew = true,
                    Showtimes = new List<string> { "14:30", "17:00", "19:30", "22:00" }
                },
                new Movie
                {
                    Id = "5",
                    Title = "Civil War",
                    Description = "En un futuro cercano, un equipo de periodistas viaja a través de Estados Unidos durante una guerra civil.",
                    Year = 2024,
                    DurationMinutes = 109,
                    Genre = "Acción, Guerra, Drama",
                    Director = "Alex Garland",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133755/Civil_war_whtcmf.jpg",
                    Rating = 7.9,
                    Classification = "R",
                    IsNew = true,
                    Showtimes = new List<string> { "16:00", "19:00", "22:00" }
                },
                new Movie
                {
                    Id = "6",
                    Title = "The Fall Guy",
                    Description = "Un doble de acción herido debe encontrar a una estrella de cine desaparecida y resolver una conspiración.",
                    Year = 2024,
                    DurationMinutes = 126,
                    Genre = "Acción, Comedia",
                    Director = "David Leitch",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133762/The_Fall_guy_xw9frf.jpg",
                    Rating = 7.4,
                    Classification = "PG-13",
                    IsNew = true,
                    Showtimes = new List<string> { "13:30", "16:30", "19:30", "22:30" }
                },
                new Movie
                {
                    Id = "7",
                    Title = "Bad Boys: Ride or Die",
                    Description = "Los detectives de Miami Mike Lowrey y Marcus Burnett se unen una vez más para una última misión.",
                    Year = 2024,
                    DurationMinutes = 115,
                    Genre = "Acción, Comedia, Crimen",
                    Director = "Adil El Arbi, Bilall Fallah",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133754/Bad_Boys_Ride_or_die_zqdtve.jpg",
                    Rating = 7.6,
                    Classification = "R",
                    IsNew = true,
                    Showtimes = new List<string> { "15:30", "18:00", "20:30", "23:00" }
                },
                new Movie
                {
                    Id = "8",
                    Title = "A Quiet Place: Day One",
                    Description = "Precuela que explora el primer día de la invasión alienígena que cambió al mundo para siempre.",
                    Year = 2024,
                    DurationMinutes = 99,
                    Genre = "Terror, Ciencia Ficción",
                    Director = "Michael Sarnoski",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133753/A_Quiet_Place_Day_One_vzyxwu.jpg",
                    Rating = 7.3,
                    Classification = "PG-13",
                    IsNew = true,
                    Showtimes = new List<string> { "17:00", "19:30", "22:00" }
                },

                // PRÓXIMOS ESTRENOS (upcoming) - IDs 9-16
                new Movie
                {
                    Id = "9",
                    Title = "Deadpool & Wolverine",
                    Description = "Wade Wilson y Logan unen fuerzas en una aventura que cambiará el MCU para siempre.",
                    Year = 2024,
                    DurationMinutes = 128,
                    Genre = "Acción, Comedia, Superhéroes",
                    Director = "Shawn Levy",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133755/Deadpool_and_wolverine_dzqukz.jpg",
                    Rating = 8.2,
                    Classification = "R",
                    IsNew = false,
                    Showtimes = new List<string> { "14:00", "17:00", "20:00", "23:00" }
                },
                new Movie
                {
                    Id = "10",
                    Title = "Inside Out 2",
                    Description = "Riley es ahora una adolescente y nuevas emociones llegan a la sala de control.",
                    Year = 2024,
                    DurationMinutes = 96,
                    Genre = "Animación, Familia, Comedia",
                    Director = "Kelsey Mann",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133759/Inside_out_2_xmhyya.jpg",
                    Rating = 8.5,
                    Classification = "PG",
                    IsNew = false,
                    Showtimes = new List<string> { "12:30", "15:00", "17:30", "20:00" }
                },
                new Movie
                {
                    Id = "11",
                    Title = "Twisters",
                    Description = "Una cazadora de tornados se enfrenta a fenómenos climáticos nunca antes vistos.",
                    Year = 2024,
                    DurationMinutes = 117,
                    Genre = "Acción, Aventura, Suspenso",
                    Director = "Lee Isaac Chung",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133764/Twisters_jdm7tl.jpg",
                    Rating = 7.1,
                    Classification = "PG-13",
                    IsNew = false,
                    Showtimes = new List<string> { "13:00", "16:00", "19:00", "22:00" }
                },
                new Movie
                {
                    Id = "12",
                    Title = "Despicable Me 4",
                    Description = "Gru y su familia enfrentan un nuevo enemigo mientras Gru Jr. causa caos en casa.",
                    Year = 2024,
                    DurationMinutes = 95,
                    Genre = "Animación, Comedia, Familia",
                    Director = "Chris Renaud",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133755/Despicable_me_4_gu5yul.jpg",
                    Rating = 7.8,
                    Classification = "PG",
                    IsNew = false,
                    Showtimes = new List<string> { "12:00", "14:30", "17:00", "19:30" }
                },
                new Movie
                {
                    Id = "13",
                    Title = "Beetlejuice Beetlejuice",
                    Description = "El travieso fantasma regresa para causar más problemas después de décadas.",
                    Year = 2024,
                    DurationMinutes = 104,
                    Genre = "Comedia, Fantasía, Terror",
                    Director = "Tim Burton",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133754/Beetlejuice_Beetlejuice_ljzfu6.jpg",
                    Rating = 7.5,
                    Classification = "PG-13",
                    IsNew = false,
                    Showtimes = new List<string> { "15:30", "18:00", "20:30", "23:00" }
                },
                new Movie
                {
                    Id = "14",
                    Title = "Alien: Romulus",
                    Description = "Un grupo de jóvenes colonizadores enfrenta la forma de vida más aterradora del universo.",
                    Year = 2024,
                    DurationMinutes = 119,
                    Genre = "Terror, Ciencia Ficción",
                    Director = "Fede Álvarez",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133754/Alien_Romulus_prirf2.jpg",
                    Rating = 7.9,
                    Classification = "R",
                    IsNew = false,
                    Showtimes = new List<string> { "16:30", "19:30", "22:30" }
                },
                new Movie
                {
                    Id = "15",
                    Title = "Joker: Folie à Deux",
                    Description = "Arthur Fleck encuentra el amor mientras continúa su descenso a la locura.",
                    Year = 2024,
                    DurationMinutes = 138,
                    Genre = "Drama, Crimen, Musical",
                    Director = "Todd Phillips",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133759/Joker_Folie_%C3%A0_Deux_cf4txd.jpg",
                    Rating = 8.3,
                    Classification = "R",
                    IsNew = false,
                    Showtimes = new List<string> { "14:30", "18:00", "21:30" }
                },
                new Movie
                {
                    Id = "16",
                    Title = "Gladiator II",
                    Description = "El hijo de Maximus busca venganza y gloria en la arena del Coliseo.",
                    Year = 2024,
                    DurationMinutes = 155,
                    Genre = "Acción, Drama, Histórica",
                    Director = "Ridley Scott",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133757/Gladiator_II_wgxwcr.jpg",
                    Rating = 8.1,
                    Classification = "R",
                    IsNew = false,
                    Showtimes = new List<string> { "15:00", "18:30", "22:00" }
                },

                // PELÍCULAS POPULARES (popular) - IDs 17-24
                new Movie
                {
                    Id = "17",
                    Title = "Oppenheimer",
                    Description = "La historia del físico J. Robert Oppenheimer y su papel en el desarrollo de la bomba atómica.",
                    Year = 2023,
                    DurationMinutes = 180,
                    Genre = "Drama, Biografía, Historia",
                    Director = "Christopher Nolan",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133761/Oppenheimer_hrvtkx.jpg",
                    Rating = 8.5,
                    Classification = "R",
                    IsNew = false,
                    Showtimes = new List<string> { "14:00", "18:00", "21:30" }
                },
                new Movie
                {
                    Id = "18",
                    Title = "Barbie",
                    Description = "Barbie y Ken se embarcan en una aventura de autodescubrimiento en el mundo real.",
                    Year = 2023,
                    DurationMinutes = 114,
                    Genre = "Comedia, Aventura, Fantasía",
                    Director = "Greta Gerwig",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133754/Barbie_ihyjsf.jpg",
                    Rating = 7.8,
                    Classification = "PG-13",
                    IsNew = false,
                    Showtimes = new List<string> { "13:00", "16:00", "19:00", "22:00" }
                },
                new Movie
                {
                    Id = "19",
                    Title = "The Super Mario Bros. Movie",
                    Description = "Mario y Luigi se embarcan en una aventura para salvar el Reino Champiñón.",
                    Year = 2023,
                    DurationMinutes = 92,
                    Genre = "Animación, Aventura, Comedia",
                    Director = "Aaron Horvath, Michael Jelenic",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133763/The_Super_Mario_Bros._Movie_yjeord.jpg",
                    Rating = 7.3,
                    Classification = "PG",
                    IsNew = false,
                    Showtimes = new List<string> { "12:30", "15:00", "17:30", "20:00" }
                },
                new Movie
                {
                    Id = "20",
                    Title = "Spider-Man: Across the Spider-Verse",
                    Description = "Miles Morales viaja a través del multiverso enfrentando nuevos Spider-People.",
                    Year = 2023,
                    DurationMinutes = 140,
                    Genre = "Animación, Acción, Aventura",
                    Director = "Joaquim Dos Santos",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133761/Spider-Man_Across_the_Spider-Verse_fvrwnp.jpg",
                    Rating = 8.7,
                    Classification = "PG",
                    IsNew = false,
                    Showtimes = new List<string> { "14:30", "17:30", "20:30" }
                },
                new Movie
                {
                    Id = "21",
                    Title = "Guardians of the Galaxy Vol. 3",
                    Description = "Los Guardianes emprenden una misión peligrosa para salvar a uno de los suyos.",
                    Year = 2023,
                    DurationMinutes = 150,
                    Genre = "Acción, Ciencia Ficción, Comedia",
                    Director = "James Gunn",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133758/Guardians_of_the_Galaxy_Vol._3_mkdrl6.jpg",
                    Rating = 8.1,
                    Classification = "PG-13",
                    IsNew = false,
                    Showtimes = new List<string> { "15:00", "18:30", "22:00" }
                },
                new Movie
                {
                    Id = "22",
                    Title = "The Batman",
                    Description = "Bruce Wayne investiga la corrupción mientras enfrenta a un asesino enigmático.",
                    Year = 2022,
                    DurationMinutes = 176,
                    Genre = "Acción, Crimen, Drama",
                    Director = "Matt Reeves",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133762/The_Batman_p8j4kz.jpg",
                    Rating = 7.8,
                    Classification = "PG-13",
                    IsNew = false,
                    Showtimes = new List<string> { "16:00", "19:30", "23:00" }
                },
                new Movie
                {
                    Id = "23",
                    Title = "Top Gun: Maverick",
                    Description = "Después de 30 años, Maverick enfrenta su pasado mientras entrena a una nueva generación.",
                    Year = 2022,
                    DurationMinutes = 130,
                    Genre = "Acción, Drama",
                    Director = "Joseph Kosinski",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133764/Top_Gun_Maverick_dyxkzd.jpg",
                    Rating = 8.3,
                    Classification = "PG-13",
                    IsNew = false,
                    Showtimes = new List<string> { "13:30", "16:30", "19:30", "22:30" }
                },
                new Movie
                {
                    Id = "24",
                    Title = "Avatar: The Way of Water",
                    Description = "Jake y Neytiri protegen a su familia mientras exploran nuevas regiones de Pandora.",
                    Year = 2022,
                    DurationMinutes = 192,
                    Genre = "Ciencia Ficción, Aventura",
                    Director = "James Cameron",
                    PosterUrl = "https://res.cloudinary.com/dtt8qst09/image/upload/v1764133753/Avatar_The_Way_of_Water_cg4cak.jpg",
                    Rating = 7.9,
                    Classification = "PG-13",
                    IsNew = false,
                    Showtimes = new List<string> { "14:00", "18:00", "21:30" }
                }
            };
        }
    }
}
