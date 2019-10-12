using System.Threading.Tasks;

using QbQuestionsAPI.Domain.Models;

namespace QbQuestionsAPI.Domain.Repositories
{
    public interface IQbQuestionRepository
    {
        Task<QbQuestion> FindByIdAsync(int id);
        Task<QbQuestion> GetRandomAsync(int? level);
        Task AddAsync(QbQuestion qbQuestion);
        bool Update(QbQuestion qbQuestion);
        void Remove(QbQuestion qbQuestion);
    }
}