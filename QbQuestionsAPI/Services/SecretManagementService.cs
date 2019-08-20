using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using System.Threading.Tasks;

using QbQuestionsAPI.Domain.Services;

namespace QbQuestionsAPI.Services
{
    public class SecretManagementService : ISecretManagementService
    {
        public async Task<string> GetKeyVaultSecret(string secretId)
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            SecretBundle secret = await keyVaultClient.GetSecretAsync(secretId).ConfigureAwait(false);
            return secret.Value;
        }
    }
}