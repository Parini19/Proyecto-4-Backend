using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScreeningsController : ControllerBase
    {
        [HttpPost("add-screening")]
        public IActionResult AddScreening()
        {
            // TODO: Implement add screening logic
            return Ok();
        }

        [HttpGet("get-screening/{id}")]
        public IActionResult GetScreening(string id)
        {
            // TODO: Implement get screening logic
            return Ok();
        }

        [HttpPut("edit-screening/{id}")]
        public IActionResult EditScreening(string id)
        {
            // TODO: Implement edit screening logic
            return Ok();
        }

        [HttpDelete("delete-screening/{id}")]
        public IActionResult DeleteScreening(string id)
        {
            // TODO: Implement delete screening logic
            return Ok();
        }

        [HttpGet("get-all-screenings")]
        public IActionResult GetAllScreenings()
        {
            // TODO: Implement get all screenings logic
            return Ok();
        }
    }
}