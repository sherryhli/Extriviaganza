using System.Threading.Tasks;

using QbQuestionsAPI.Domain.Models;

namespace QbQuestionsAPI.Domain.Repositories
{
    public interface IQbQuestionRepository
    {
        Task<QbQuestion> FindByIdAsync(int id);
        Task AddAsync(QbQuestion qbQuestion);
        void Update(QbQuestion qbQuestion);
        void Remove(QbQuestion qbQuestion);
    }
}