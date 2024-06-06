using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using projektdotnet.Models;
using projektdotnet.Services;


namespace projektdotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsApiController : ControllerBase
    {
        private readonly TicketService _ticketService;

        public TicketsApiController(TicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // Endpoint do pobierania wszystkich ticketów
        [HttpGet("tickets")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetAllTickets()
        {
            var tickets = await _ticketService.GetAllTickets();
            return Ok(tickets);
        }
    }
}
