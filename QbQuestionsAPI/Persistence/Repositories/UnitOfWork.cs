using System.Threading.Tasks;

using QbQuestionsAPI.Domain.Repositories;
using QbQuestionsAPI.Persistence.Contexts;

namespace QbQuestionsAPI.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}