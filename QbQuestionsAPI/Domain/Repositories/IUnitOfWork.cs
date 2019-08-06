using System.Threading.Tasks;

namespace QbQuestionsAPI.Domain.Repositories
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}
