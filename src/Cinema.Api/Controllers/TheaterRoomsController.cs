using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TheaterRoomsController : ControllerBase
    {
        [HttpPost("add-theater-room")]
        public IActionResult AddTheaterRoom()
        {
            // TODO: Implement add theater room logic
            return Ok();
        }

        [HttpGet("get-theater-room/{id}")]
        public IActionResult GetTheaterRoom(string id)
        {
            // TODO: Implement get theater room logic
            return Ok();
        }

        [HttpPut("edit-theater-room/{id}")]
        public IActionResult EditTheaterRoom(string id)
        {
            // TODO: Implement edit theater room logic
            return Ok();
        }

        [HttpDelete("delete-theater-room/{id}")]
        public IActionResult DeleteTheaterRoom(string id)
        {
            // TODO: Implement delete theater room logic
            return Ok();
        }

        [HttpGet("get-all-theater-rooms")]
        public IActionResult GetAllTheaterRooms()
        {
            // TODO: Implement get all theater rooms logic
            return Ok();
        }
    }
}