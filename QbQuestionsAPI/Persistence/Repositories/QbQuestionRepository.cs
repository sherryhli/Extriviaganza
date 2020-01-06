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
            // TODO: Consider using SQL parameter for table
            string table = level == null ? "QbQuestions" : $"(SELECT * FROM QbQuestions WHERE Level = {level})";
            string query = "SELECT TOP 1 * FROM " + table + 
                           $" AS q WHERE q.Id >= (ABS(CHECKSUM(NEWID())) % (SELECT MAX(p.Id) FROM {table} AS p));";
            var result = await _context.QbQuestions
                .FromSql<QbQuestion>(query)
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
