using projektdotnet.Models;
using projektdotnet.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace projektdotnet.Services
{
    public class TicketCommentService
    {
        private readonly TicketCommentRepository _ticketCommentRepository;

        public TicketCommentService(TicketCommentRepository ticketCommentRepository)
        {
            _ticketCommentRepository = ticketCommentRepository;
        }

        public async Task<List<TicketComment>> GetAllComments()
        {
            return await _ticketCommentRepository.GetAllComments();
        }

        public async Task<TicketComment> GetCommentById(int id)
        {
            return await _ticketCommentRepository.GetCommentById(id);
        }

        public async Task AddComment(TicketComment comment)
        {
            await _ticketCommentRepository.AddComment(comment);
        }

        public async Task UpdateComment(TicketComment comment)
        {
            await _ticketCommentRepository.UpdateComment(comment);
        }

        public async Task RemoveComment(int id)
        {
            await _ticketCommentRepository.RemoveComment(id);
        }

        public async Task<bool> CommentExists(int id)
        {
            return await _ticketCommentRepository.CommentExists(id);
        }
    }
}
