using projektdotnet.Models;
using projektdotnet.Repositories;

namespace projektdotnet.Services
{
    public class OpinionService
    {
        private readonly OpinionRepository _opinionRepository;
        public OpinionService(OpinionRepository opinionRepository)
        {
            _opinionRepository = opinionRepository;
        }
        public async Task<List<Opinion>> GetAllOpinions()
        {
            return await _opinionRepository.GetAllOpinions();
        }
        public async Task<Opinion> GetOpinionById(int? id)
        {
            return await _opinionRepository.GetOpinionById(id);
        }
        public async Task AddOpinion(Opinion opinion)
        {
            await _opinionRepository.AddOpinion(opinion);
        }
        public async Task UpdateOpinion(Opinion opinion)
        {
            await _opinionRepository.UpdateOpinion(opinion);
        }
        public async Task<bool> OpinionExists(int id)
        {
            return await _opinionRepository.OpinionExists(id);
        }
        public async Task RemoveOpinion(int id)
        {
            await _opinionRepository.RemoveOpinion(id);
        }
    }
}
