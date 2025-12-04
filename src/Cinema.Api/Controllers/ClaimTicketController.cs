using Microsoft.AspNetCore.Mvc;
using Cinema.Api.Services;
using Cinema.Domain.Entities;
using System.Threading.Tasks;

namespace Cinema.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClaimTicketController : ControllerBase
    {
        private readonly FirestoreClaimTicketService _claimTicketService;

        public ClaimTicketController(FirestoreClaimTicketService claimTicketService)
        {
            _claimTicketService = claimTicketService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClaimTicket ticket)
        {
            await _claimTicketService.AddClaimTicketAsync(ticket);
            return Ok(ticket);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var ticket = await _claimTicketService.GetClaimTicketAsync(id);
            if (ticket == null)
                return NotFound();
            return Ok(ticket);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tickets = await _claimTicketService.GetAllClaimTicketsAsync();
            return Ok(tickets);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ClaimTicket ticket)
        {
            if (id != ticket.Id)
                return BadRequest();

            await _claimTicketService.UpdateClaimTicketAsync(ticket);
            return Ok(ticket);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _claimTicketService.DeleteClaimTicketAsync(id);
            return NoContent();
        }
    }
}