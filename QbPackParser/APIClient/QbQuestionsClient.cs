using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace QbPackParser.APIClient
{
    public class QbQuestionsClient : HttpClient
    {
        private readonly string baseUrl = "http://qbquestionsapi.azurewebsites.net/";

        /// <summary>Creates a new instance of QbQuestionsClient</summary>
        public QbQuestionsClient() : base()
        {
            BaseAddress = new System.Uri(baseUrl);
        }

        /// <summary>Adds questions to the database via a REST API call</summary>
        /// <param name="content">The questions to add in the form of a JSON array</param>
        public async Task<HttpResponseMessage> AddQuestionsToDbAsync(string content)
        {
            HttpResponseMessage authorizationResponse = await GetAuthorization();
            string token = await authorizationResponse.Content.ReadAsStringAsync();
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            StringContent stringContent = new StringContent(content, Encoding.UTF8, "application/json");
            return await PostAsync("api/qbquestions", stringContent);
        }

        // TODO: Refactor to use refresh token
        /// <summary>Gets the authorization information.</summary>
        private async Task<HttpResponseMessage> GetAuthorization()
        {
            string username = await GetKeyVaultSecret("https://extriviaganza-vault.vault.azure.net/secrets/QbQuestionsUsername");
            string password = await GetKeyVaultSecret("https://extriviaganza-vault.vault.azure.net/secrets/QbQuestionsPassword");
            string content = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\"}";
            StringContent stringContent = new StringContent(content, Encoding.UTF8, "application/json");
            return await PostAsync("api/authenticate", stringContent);
        }

        /// <summary>Retrieves a secret from Azure Key Vault</summary>
        /// <param name="secretId">The secret's identifier in Azure Key Vault</param>
        private async Task<string> GetKeyVaultSecret(string secretId)
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            SecretBundle secret = await keyVaultClient.GetSecretAsync(secretId).ConfigureAwait(false);
            return secret.Value;
        }
    }
}
