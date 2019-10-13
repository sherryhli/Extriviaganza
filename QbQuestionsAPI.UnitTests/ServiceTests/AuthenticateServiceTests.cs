using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;

using QbQuestionsAPI.Domain.Models;
using QbQuestionsAPI.Domain.Services;
using QbQuestionsAPI.Services;

namespace QbQuestionsAPI.UnitTests.ServiceTests
{
    public class AuthenticateServiceTests
    {
        [Fact]
        public void IsAuthenticatedSuccessTest()
        {
            // Arrange
            IUserService userService = Substitute.For<IUserService>();
            userService.IsValidUser(Arg.Any<User>()).Returns(true);
            ISecretManagementService secretManagementService = Substitute.For<ISecretManagementService>();
            secretManagementService.GetKeyVaultSecret(Arg.Any<string>()).Returns(Task.FromResult("very_long_token_secret"));
            IOptions<TokenPayload> options = Options.Create(new TokenPayload{
                Secret = string.Empty,
                Issuer = string.Empty,
                Audience = string.Empty,
                AccessExpiration = 1,
                RefreshExpiration = 1
            });
            IAuthenticateService authenticateService = new AuthenticateService(userService, secretManagementService, options);
            User user = new User{
                Username = "username"
            };
            string token;

            // Act
            bool success = authenticateService.IsAuthenticated(user, out token);

            // Assert
            success.Should().Be(true);
        }

        [Fact]
        public void IsAuthenticatedFailureTest()
        {
            // Arrange
            IUserService userService = Substitute.For<IUserService>();
            userService.IsValidUser(Arg.Any<User>()).Returns(false);
            ISecretManagementService secretManagementService = Substitute.For<ISecretManagementService>();
            IOptions<TokenPayload> options = Options.Create(new TokenPayload{
                Secret = string.Empty,
                Issuer = string.Empty,
                Audience = string.Empty,
                AccessExpiration = 1,
                RefreshExpiration = 1
            });
            IAuthenticateService authenticateService = new AuthenticateService(userService, secretManagementService, options);
            User user = new User();
            string token;

            // Act
            bool success = authenticateService.IsAuthenticated(user, out token);

            // Assert
            success.Should().Be(false);
        }
    }
}