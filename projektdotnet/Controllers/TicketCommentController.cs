using Microsoft.AspNetCore.Mvc;
using projektdotnet.Models;
using projektdotnet.Services;
using System.Threading.Tasks;

namespace projektdotnet.Controllers
{
    public class TicketCommentController : Controller
    {
        private readonly TicketCommentService _ticketCommentService;
        private readonly TicketService _ticketService;  // Assuming there's a TicketService for fetching ticket details

        public TicketCommentController(TicketCommentService ticketCommentService, TicketService ticketService)
        {
            _ticketCommentService = ticketCommentService;
            _ticketService = ticketService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("TicketId,EmployeeId,Description")] TicketComment comment)
        {
            if (ModelState.IsValid)
            {
                var ticket = await _ticketService.GetTicketById(comment.TicketId);
                if (ticket != null)
                {
                    comment.CreationDate = DateTime.Now;
                    await _ticketCommentService.AddComment(comment);
                    return RedirectToAction("Details", "Tickets", new { id = comment.TicketId });
                }
                else
                {
                    return NotFound("Ticket not found");
                }
            }
            return BadRequest(ModelState);
        }
    }
}
