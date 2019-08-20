using System.Threading.Tasks;

namespace QbQuestionsAPI.Domain.Services
{
    public interface ISecretManagementService
    {
        Task<string> GetKeyVaultSecret(string secretId);
    }
}