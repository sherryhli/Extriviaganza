using QbQuestionsAPI.Domain.Models;

namespace QbQuestionsAPI.Domain.Services
{
    public interface IAuthenticateService
    {
        bool IsAuthenticated(User user, out string token);
    }
}