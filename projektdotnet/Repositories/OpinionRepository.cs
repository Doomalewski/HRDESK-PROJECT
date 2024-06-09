using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using projektdotnet.Data;
using projektdotnet.Models;

namespace projektdotnet.Repositories
{
    public class OpinionRepository
    {
        private readonly NewDbContext _context;
        public OpinionRepository(NewDbContext context)
        {
            _context = context;
        }
        public async Task<List<Opinion>> GetAllOpinions()
        {
            return await _context.Opinions.Include(e => e.Employee).ToListAsync();
        }
        public async Task<Opinion> GetOpinionById(int? id)
        {
            return await _context.Opinions.Include(e => e.Employee).FirstOrDefaultAsync(e => e.OpinionId == id);
        }
        public async Task AddOpinion(Opinion opinion)
        {
            _context.Add(opinion);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateOpinion(Opinion opinion)
        {
            _context.Update(opinion);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> OpinionExists(int id)
        {
            return await _context.Opinions.AnyAsync(e => e.OpinionId == id);
        }
        public async Task RemoveOpinion(int id)
        {
            var opinion = await _context.Opinions.FirstOrDefaultAsync(e=>e.OpinionId == id);
            _context.Opinions.Remove(opinion);
            await _context.SaveChangesAsync();

        }
    }
}
