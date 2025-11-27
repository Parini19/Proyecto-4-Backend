using System;
using System.Threading.Tasks;
using Cinema.Api.Services;
using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers
{
    /// <summary>
    /// Controlador para gestionar cines/sedes
    /// Endpoints: CRUD completo + estadísticas
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CinemaLocationsController : ControllerBase
    {
        private readonly FirestoreCinemaLocationService _cinemaService;

        public CinemaLocationsController(FirestoreCinemaLocationService cinemaService)
        {
            _cinemaService = cinemaService;
        }

        /// <summary>
        /// Crea un nuevo cine
        /// POST /api/CinemaLocations/add-cinema
        /// </summary>
        [HttpPost("add-cinema")]
        public async Task<IActionResult> AddCinema([FromBody] CinemaLocation cinema)
        {
            try
            {
                var created = await _cinemaService.AddCinemaLocationAsync(cinema);
                return Ok(new { success = true, cinema = created });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to add cinema.", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un cine por ID
        /// GET /api/CinemaLocations/get-cinema/{id}
        /// </summary>
        [HttpGet("get-cinema/{id}")]
        public async Task<IActionResult> GetCinema(string id)
        {
            try
            {
                var cinema = await _cinemaService.GetCinemaLocationAsync(id);
                if (cinema == null)
                    return NotFound(new { success = false, message = "Cinema not found." });

                return Ok(new { success = true, cinema });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to get cinema.", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todos los cines
        /// GET /api/CinemaLocations/get-all-cinemas
        /// </summary>
        [HttpGet("get-all-cinemas")]
        public async Task<IActionResult> GetAllCinemas()
        {
            try
            {
                var cinemas = await _cinemaService.GetAllCinemaLocationsAsync();
                return Ok(new { success = true, cinemas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to get cinemas.", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene solo cines activos
        /// GET /api/CinemaLocations/get-active-cinemas
        /// </summary>
        [HttpGet("get-active-cinemas")]
        public async Task<IActionResult> GetActiveCinemas()
        {
            try
            {
                var cinemas = await _cinemaService.GetActiveCinemaLocationsAsync();
                return Ok(new { success = true, cinemas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to get active cinemas.", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene cines por ciudad
        /// GET /api/CinemaLocations/get-cinemas-by-city/{city}
        /// </summary>
        [HttpGet("get-cinemas-by-city/{city}")]
        public async Task<IActionResult> GetCinemasByCity(string city)
        {
            try
            {
                var cinemas = await _cinemaService.GetCinemaLocationsByCityAsync(city);
                return Ok(new { success = true, cinemas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to get cinemas by city.", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un cine existente
        /// PUT /api/CinemaLocations/update-cinema/{id}
        /// </summary>
        [HttpPut("update-cinema/{id}")]
        public async Task<IActionResult> UpdateCinema(string id, [FromBody] CinemaLocation cinema)
        {
            try
            {
                if (id != cinema.Id)
                    return BadRequest(new { success = false, message = "ID mismatch." });

                await _cinemaService.UpdateCinemaLocationAsync(cinema);
                return Ok(new { success = true, message = "Cinema updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to update cinema.", error = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un cine
        /// DELETE /api/CinemaLocations/delete-cinema/{id}
        /// IMPORTANTE: Verificar que no tenga salas o funciones antes de eliminar
        /// </summary>
        [HttpDelete("delete-cinema/{id}")]
        public async Task<IActionResult> DeleteCinema(string id)
        {
            try
            {
                await _cinemaService.DeleteCinemaLocationAsync(id);
                return Ok(new { success = true, message = "Cinema deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to delete cinema.", error = ex.Message });
            }
        }

        /// <summary>
        /// Activa o desactiva un cine
        /// PATCH /api/CinemaLocations/toggle-status/{id}
        /// </summary>
        [HttpPatch("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(string id, [FromBody] ToggleStatusRequest request)
        {
            try
            {
                await _cinemaService.ToggleCinemaLocationStatusAsync(id, request.IsActive);
                return Ok(new { success = true, message = "Cinema status updated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to toggle cinema status.", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene estadísticas de un cine (salas, funciones, etc.)
        /// GET /api/CinemaLocations/get-stats/{id}
        /// </summary>
        [HttpGet("get-stats/{id}")]
        public async Task<IActionResult> GetCinemaStats(string id)
        {
            try
            {
                var stats = await _cinemaService.GetCinemaStatsAsync(id);
                return Ok(new { success = true, stats });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to get cinema stats.", error = ex.Message });
            }
        }
    }

    /// <summary>
    /// Request para activar/desactivar un cine
    /// </summary>
    public class ToggleStatusRequest
    {
        public bool IsActive { get; set; }
    }
}
