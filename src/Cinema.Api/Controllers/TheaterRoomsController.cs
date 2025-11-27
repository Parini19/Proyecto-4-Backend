using System;
using System.Linq;
using System.Threading.Tasks;
using Cinema.Api.Services;
using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace Cinema.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TheaterRoomsController : ControllerBase
    {
        private readonly FirestoreTheaterRoomService _theaterRoomService;
        private readonly FirestoreCinemaLocationService _cinemaLocationService;

        public TheaterRoomsController(FirestoreTheaterRoomService theaterRoomService, FirestoreCinemaLocationService cinemaLocationService)
        {
            _theaterRoomService = theaterRoomService;
            _cinemaLocationService = cinemaLocationService;
        }

        [HttpPost("add-theater-room")]
        public async Task<IActionResult> AddTheaterRoom([FromBody] TheaterRoom room)
        {
            try
            {
                await _theaterRoomService.AddTheaterRoomAsync(room);
                return Ok(new { success = true, id = room.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to add theater room.", error = ex.Message });
            }
        }

        [HttpGet("get-theater-room/{id}")]
        public async Task<IActionResult> GetTheaterRoom(string id)
        {
            try
            {
                var room = await _theaterRoomService.GetTheaterRoomAsync(id);
                if (room == null)
                    return NotFound(new { success = false, message = "Theater room not found." });

                return Ok(new { success = true, room });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to get theater room.", error = ex.Message });
            }
        }

        [HttpPut("edit-theater-room/{id}")]
        public async Task<IActionResult> EditTheaterRoom(string id, [FromBody] TheaterRoom room)
        {
            try
            {
                room.Id = id;
                await _theaterRoomService.UpdateTheaterRoomAsync(room);
                return Ok(new { success = true, room });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to edit theater room.", error = ex.Message });
            }
        }

        [HttpDelete("delete-theater-room/{id}")]
        public async Task<IActionResult> DeleteTheaterRoom(string id)
        {
            try
            {
                await _theaterRoomService.DeleteTheaterRoomAsync(id);
                return Ok(new { success = true, message = $"Theater room {id} deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to delete theater room.", error = ex.Message });
            }
        }

        [HttpGet("get-all-theater-rooms")]
        public async Task<IActionResult> GetAllTheaterRooms()
        {
            try
            {
                var rooms = await _theaterRoomService.GetAllTheaterRoomsAsync();
                return Ok(new
                {
                    success = true,
                    rooms
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to get theater rooms.", error = ex.Message });
            }
        }

        /// <summary>
        /// Clear all existing theater rooms (for testing).
        /// DELETE /api/theaterrooms/clear-all
        /// </summary>
        [HttpDelete("clear-all")]
        public async Task<IActionResult> ClearAllTheaterRooms()
        {
            try
            {
                var existingRooms = await _theaterRoomService.GetAllTheaterRoomsAsync();
                int deletedCount = 0;

                foreach (var room in existingRooms)
                {
                    await _theaterRoomService.DeleteTheaterRoomAsync(room.Id);
                    deletedCount++;
                }

                return Ok(new
                {
                    success = true,
                    message = $"Deleted {deletedCount} theater rooms",
                    count = deletedCount
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Error deleting theater rooms: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Seed theater rooms for testing.
        /// Creates 15 normal rooms (Sala 1-15) and 5 VIP rooms (Sala VIP 1-5).
        /// POST /api/theaterrooms/seed?clearExisting=false
        /// </summary>
        [HttpPost("seed")]
        public async Task<IActionResult> SeedTheaterRooms([FromQuery] bool clearExisting = false)
        {
            try
            {
                // Clear existing rooms if requested
                if (clearExisting)
                {
                    var existingRooms = await _theaterRoomService.GetAllTheaterRoomsAsync();
                    foreach (var room in existingRooms)
                    {
                        await _theaterRoomService.DeleteTheaterRoomAsync(room.Id);
                    }
                }

                var rooms = new System.Collections.Generic.List<TheaterRoom>();

                // Create 15 normal rooms: Sala 1 - Sala 15 (96 seats each)
                for (int i = 1; i <= 15; i++)
                {
                    var room = new TheaterRoom
                    {
                        Id = $"SALA-{i:D2}",
                        Name = $"Sala {i}",
                        Capacity = 96  // 8 rows x 12 seats
                    };
                    await _theaterRoomService.AddTheaterRoomAsync(room);
                    rooms.Add(room);
                }

                // Create 5 VIP rooms: Sala VIP 1 - Sala VIP 5 (60 seats each)
                for (int i = 1; i <= 5; i++)
                {
                    var room = new TheaterRoom
                    {
                        Id = $"SALA-VIP-{i:D2}",
                        Name = $"Sala VIP {i}",
                        Capacity = 60  // Smaller, more luxurious seating
                    };
                    await _theaterRoomService.AddTheaterRoomAsync(room);
                    rooms.Add(room);
                }

                return Ok(new
                {
                    success = true,
                    message = $"Created {rooms.Count} theater rooms (15 normal + 5 VIP)",
                    count = rooms.Count,
                    normalRooms = 15,
                    vipRooms = 5,
                    rooms = rooms.Select(r => new
                    {
                        r.Id,
                        r.Name,
                        r.Capacity
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Error seeding theater rooms: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Assign theater rooms to cinemas evenly
        /// POST /api/theaterrooms/assign-to-cinemas
        /// </summary>
        [HttpPost("assign-to-cinemas")]
        public async Task<IActionResult> AssignRoomsToCinemas()
        {
            try
            {
                var cinemas = await _cinemaLocationService.GetAllCinemaLocationsAsync();
                var rooms = await _theaterRoomService.GetAllTheaterRoomsAsync();

                if (cinemas.Count == 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No cinemas found. Please seed cinemas first."
                    });
                }

                if (rooms.Count == 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No theater rooms found. Please seed rooms first."
                    });
                }

                // Distribute rooms evenly among cinemas
                var roomsPerCinema = (int)Math.Ceiling((double)rooms.Count / cinemas.Count);
                var assignments = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>();
                var updatedRooms = new System.Collections.Generic.List<TheaterRoom>();

                int currentCinemaIndex = 0;
                foreach (var room in rooms)
                {
                    var cinema = cinemas[currentCinemaIndex % cinemas.Count];
                    room.CinemaId = cinema.Id;

                    await _theaterRoomService.UpdateTheaterRoomAsync(room);
                    updatedRooms.Add(room);

                    if (!assignments.ContainsKey(cinema.Name))
                    {
                        assignments[cinema.Name] = new System.Collections.Generic.List<string>();
                    }
                    assignments[cinema.Name].Add(room.Name);

                    currentCinemaIndex++;
                }

                return Ok(new
                {
                    success = true,
                    message = $"Assigned {rooms.Count} rooms to {cinemas.Count} cinemas",
                    totalRooms = rooms.Count,
                    totalCinemas = cinemas.Count,
                    roomsPerCinema = roomsPerCinema,
                    assignments = assignments.Select(a => new {
                        cinema = a.Key,
                        rooms = a.Value
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Error assigning rooms to cinemas: {ex.Message}"
                });
            }
        }
    }
}