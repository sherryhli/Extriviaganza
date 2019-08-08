using System.Threading.Tasks;

using QbQuestionsAPI.Domain.Models;
using QbQuestionsAPI.Domain.Repositories;
using QbQuestionsAPI.Persistence.Contexts;

namespace QbQuestionsAPI.Persistence.Repositories
{
    public class QbQuestionRepository : IQbQuestionRepository
    {
        private readonly AppDbContext _context;

        public QbQuestionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<QbQuestion> FindByIdAsync(int id)
        {
            return await _context.QbQuestions.FindAsync(id);
        }

         public async Task AddAsync(QbQuestion qbQuestion)
        {
            await _context.QbQuestions.AddAsync(qbQuestion);
        }

        public void Update(QbQuestion qbQuestion)
        {
            _context.QbQuestions.Update(qbQuestion);
        }

        public void Remove(QbQuestion qbQuestion)
        {
            _context.QbQuestions.Remove(qbQuestion);
        }
    }
}