using Microsoft.EntityFrameworkCore;
using projektdotnet.Data;
using projektdotnet.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace projektdotnet.Repositories
{
    public class TicketCommentRepository
    {
        private readonly NewDbContext _context;

        public TicketCommentRepository(NewDbContext context)
        {
            _context = context;
        }

        public async Task<List<TicketComment>> GetAllComments()
        {
            return await _context.TicketComments.ToListAsync();
        }

        public async Task<TicketComment> GetCommentById(int id)
        {
            return await _context.TicketComments.FirstOrDefaultAsync(tc => tc.TicketCommentId == id);
        }

        public async Task AddComment(TicketComment comment)
        {
            _context.TicketComments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateComment(TicketComment comment)
        {
            _context.TicketComments.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveComment(int id)
        {
            var comment = await _context.TicketComments.FirstOrDefaultAsync(tc => tc.TicketCommentId == id);
            if (comment != null)
            {
                _context.TicketComments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> CommentExists(int id)
        {
            return await _context.TicketComments.AnyAsync(tc => tc.TicketCommentId == id);
        }
    }
}
