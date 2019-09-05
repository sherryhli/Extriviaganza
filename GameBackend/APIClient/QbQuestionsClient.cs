using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GameBackend.APIClient
{
    public class QbQuestionsClient : HttpClient
    {
        private readonly string baseUrl = "http://qbquestionsapi.azurewebsites.net/";

        public QbQuestionsClient() : base()
        {
            BaseAddress = new System.Uri(baseUrl);
        }

        public async Task<HttpResponseMessage> GetRandomQuestionAsync()
        {
            HttpResponseMessage authorizationResponse = await GetAuthorization();
            string token = await authorizationResponse.Content.ReadAsStringAsync();
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await GetAsync("api/qbquestions/random");
        }

        // TODO: Refactor to use refresh token
        private async Task<HttpResponseMessage> GetAuthorization()
        {
            string username = await GetKeyVaultSecret("https://extriviaganza-vault.vault.azure.net/secrets/QbQuestionsUsername");
            string password = await GetKeyVaultSecret("https://extriviaganza-vault.vault.azure.net/secrets/QbQuestionsPassword");
            string content = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\"}";
            StringContent stringContent = new StringContent(content, Encoding.UTF8, "application/json");
            return await PostAsync("api/authenticate", stringContent);
        }

        private async Task<string> GetKeyVaultSecret(string secretId)
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            SecretBundle secret = await keyVaultClient.GetSecretAsync(secretId).ConfigureAwait(false);
            return secret.Value;
        }
    }
}