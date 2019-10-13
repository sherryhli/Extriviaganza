using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
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

        public async Task<QbQuestion> GetRandomAsync(int? level)
        {
            #pragma warning disable EF1000
            string whereClause = level == null ? string.Empty : $"WHERE Level = {level}";
            // TODO: Consider using SQL parameter for whereClause
            var result = await _context.QbQuestions
                .FromSql<QbQuestion>((string)$"SELECT TOP 1 * from QbQuestions {whereClause} ORDER BY NEWID()")
                .ToListAsync();

            return result.FirstOrDefault();
        }

        public async Task AddAsync(QbQuestion qbQuestion)
        {
            await _context.QbQuestions.AddAsync(qbQuestion);
        }

        public bool Update(QbQuestion qbQuestion)
        {
            try 
            {
                _context.QbQuestions.Update(qbQuestion);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Remove(QbQuestion qbQuestion)
        {
            try 
            {
                _context.QbQuestions.Remove(qbQuestion);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
