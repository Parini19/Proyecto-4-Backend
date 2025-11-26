using Microsoft.AspNetCore.Mvc;
using Cinema.Api.Services;

namespace Cinema.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly OpenAIChatService _chatService;
        private readonly FirestoreMovieService _movieService;

        public ChatController(OpenAIChatService chatService, FirestoreMovieService movieService)
        {
            _chatService = chatService;
            _movieService = movieService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChatRequest request)
        {
            var movies = await _movieService.GetAllMoviesAsync(); // Adjust method name as needed
            var movieContext = string.Join("\n", movies.Select(m => $"{m.Title} ({m.Genre}): {m.Description}"));
            var response = await _chatService.GetChatResponseAsync(request.Message, movieContext);
            return Ok(new { reply = response });
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; }
    }
}
