using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using QbQuestionsAPI.Domain.Models;
using QbQuestionsAPI.Domain.Services;

namespace QbQuestionsAPI.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly IUserService _userService;
        private readonly TokenPayload _tokenPayload;

        public AuthenticateService(IUserService userService, IOptions<TokenPayload> tokenPayload)
        {
            _userService = userService;
            _tokenPayload = tokenPayload.Value;
        }

        public bool IsAuthenticated(User user, out string token)
        {
            token = string.Empty;

            if (!_userService.IsValidUser(user))
            {
                return false;
            }

            Claim[] claim = new[]
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            const string issuerSigningKeyId = "https://extriviaganza-vault.vault.azure.net/secrets/QbQuestionsIssuerSigningKey";
            _tokenPayload.Secret = GetKeyVaultSecret(issuerSigningKeyId).Result;

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_tokenPayload.Secret));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtToken = new JwtSecurityToken(
                _tokenPayload.Issuer,
                _tokenPayload.Audience,
                claim,
                expires: DateTime.Now.AddSeconds(_tokenPayload.AccessExpiration),
                signingCredentials: credentials
            );

            token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return true;
        }

        // TODO: Use shared secret management service
        private async Task<string> GetKeyVaultSecret(string secretId)
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            SecretBundle secret = await keyVaultClient.GetSecretAsync(secretId).ConfigureAwait(false);
            return secret.Value;
        }
    }
}