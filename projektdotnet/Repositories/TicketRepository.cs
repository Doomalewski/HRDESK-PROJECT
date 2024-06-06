using Microsoft.EntityFrameworkCore;
using projektdotnet.Data;
using projektdotnet.Models;
using System.Security.Policy;
using System.Web.Mvc.Async;

namespace projektdotnet.Repositories
{
    public class TicketRepository
    {
        private readonly NewDbContext _context;
        public TicketRepository(NewDbContext context)
        {
            _context = context;
        }
        public async Task<List<Ticket>> GetTicketsWithSenderById(int id)
        {
            return await _context.Tickets
                .Include(s => s.Sender)
                .Include(r => r.Receiver)
                .Include(r=>r.Comments)
                .Where(t => t.SenderId == id)
                .ToListAsync();
        }
        public async Task<List<Ticket>> GetTicketsWithReceiverById(int id)
        {
            return await _context.Tickets
                .Include(s => s.Sender)
                .Include(r => r.Receiver)
                .Include(r => r.Comments)
                .Where(t => t.ReceiverId == id)
                .ToListAsync();
        }
        public async Task AddTicket(Ticket ticket)
        {
            _context.Add(ticket);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateTicket(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }
        public async Task<Ticket> GetTicketById(int? id)
        {
            if (id == null)
            {
                return null;
            }
            return await _context.Tickets
                .Include(s => s.Sender)
                .Include (r => r.Receiver)
                .Include(r => r.Comments)
                .FirstOrDefaultAsync(m => m.TicketId == id);
        }
        public async Task<bool> TicketExists(int id)
        {
            return await _context.Tickets.AnyAsync(e => e.TicketId == id);
        }
        public async Task RemoveTicket(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Ticket>> GetAllTickets()
        {
            return await _context.Tickets
                .Include(t => t.Sender)
                .Include(t => t.Receiver)
                .Include(r => r.Comments)
                .ToListAsync();
        }
        public async Task<List<Ticket>> GetAllResolvedTickets()
        {
            return await _context.Tickets
                .Include(t => t.Sender)
                .Include(t => t.Receiver)
                .Include(r => r.Comments)
                .Where(t=>t.Status == TicketStatus.Resolved)
                .ToListAsync();
        }
        public async Task SaveDbChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}
