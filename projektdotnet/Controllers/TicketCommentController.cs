using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using projektdotnet.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using projektdotnet.Data;
namespace projektdotnet.Controllers
{
    public class TicketCommentController : Controller
    {
        private readonly NewDbContext _context;
        public TicketCommentController(NewDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> Create([Bind("TicketId,EmployeeId,Description")] TicketComment comment)
        {
            if (ModelState.IsValid)
            {
                var ticket = _context.Tickets.FirstOrDefault(e => e.TicketId == comment.TicketId);
                if (ticket != null)
                {
                    comment.CreationDate = DateTime.Now;
                    _context.TicketComments.Add(comment);
                    await _context.SaveChangesAsync();
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
