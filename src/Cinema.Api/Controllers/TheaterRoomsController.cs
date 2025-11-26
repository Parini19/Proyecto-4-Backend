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

        public TheaterRoomsController(FirestoreTheaterRoomService theaterRoomService)
        {
            _theaterRoomService = theaterRoomService;
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
    }
}