using QbQuestionsAPI.Domain.Models;

namespace QbQuestionsAPI.Domain.Repositories
{
    public interface IUserRepository
    {
        bool IsValidUser(User user);
    }
}