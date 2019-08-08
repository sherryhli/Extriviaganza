using System.Threading.Tasks;

using QbQuestionsAPI.Domain.Models;
using QbQuestionsAPI.Domain.Services.Communication;

namespace QbQuestionsAPI.Domain.Services
{
    public interface IQbQuestionService
    {
        Task<QbQuestion> GetAsync(int id);
        Task<QbQuestionResponse> SaveAsync(QbQuestion qbQuestion);
        Task<QbQuestionResponse> UpdateAsync(int id, QbQuestion qbQuestion);
        Task<QbQuestionResponse> DeleteAsync(int id);
    }
}