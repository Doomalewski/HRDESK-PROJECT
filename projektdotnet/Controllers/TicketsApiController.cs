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

        // Endpoint to get all tickets
        [HttpGet("tickets")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetAllTickets()
        {
            var tickets = await _ticketService.GetAllTickets();
            return Ok(tickets);
        }

        // Endpoint to get a ticket by id
        [HttpGet("tickets/{id}")]
        public async Task<ActionResult<Ticket>> GetTicketById(int id)
        {
            var ticket = await _ticketService.GetTicketById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }

        // Endpoint to create a new ticket
        [HttpPost("tickets")]
        public async Task<ActionResult<Ticket>> CreateTicket(Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdTicket = await _ticketService.AddTicket(ticket);

            return CreatedAtAction(nameof(GetTicketById), new { id = createdTicket.TicketId }, createdTicket);
        }

        // Endpoint to update an existing ticket
        [HttpPut("tickets/{id}")]
        public async Task<IActionResult> UpdateTicket(int id, Ticket ticket)
        {
            if (id != ticket.TicketId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _ticketService.UpdateTicket(ticket);

            if (result == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // Endpoint to delete a ticket
        [HttpDelete("tickets/{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var ticket = await _ticketService.GetTicketById(id);
            var result = await _ticketService.RemoveTicket(ticket);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
