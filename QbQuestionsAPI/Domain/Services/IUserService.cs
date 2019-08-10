using QbQuestionsAPI.Domain.Models;

namespace QbQuestionsAPI.Domain.Services
{
    public interface IUserService
    {
        bool IsValidUser(User user);
    }
}