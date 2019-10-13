using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

using QbQuestionsAPI.Controllers;
using QbQuestionsAPI.Domain.Models;
using QbQuestionsAPI.Domain.Services;
using QbQuestionsAPI.Resources;

namespace QbQuestionsAPI.UnitTests.ControllerTests
{
    public class AuthenticateControllerTests
    {
        private readonly IMapper mapper = Substitute.For<IMapper>();

        [Fact]
        public void GetAuthTokenSuccessTest()
        {
            // Arrange
            AuthenticateUserResource resource = new AuthenticateUserResource();
            User user = new User();
            mapper.Map<AuthenticateUserResource, User>(Arg.Any<AuthenticateUserResource>()).Returns(user);
            IAuthenticateService authenticateService = Substitute.For<IAuthenticateService>();
            string token;
            authenticateService.IsAuthenticated(user, out token)
            .Returns(x => { 
                x[1] = "token";
                return true;
            });
            AuthenticateController controller = new AuthenticateController(authenticateService, mapper);

            // Act
            IActionResult result = controller.GetAuthToken(resource);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            OkObjectResult okResult = result as OkObjectResult;
            okResult.Value.Should().Be("token");
        }

        [Fact]
        public void GetAuthTokenFailureTest()
        {
            // Arrange
            AuthenticateUserResource resource = new AuthenticateUserResource();
            User user = new User();
            mapper.Map<AuthenticateUserResource, User>(Arg.Any<AuthenticateUserResource>()).Returns(user);
            IAuthenticateService authenticateService = Substitute.For<IAuthenticateService>();
            string token;
            authenticateService.IsAuthenticated(user, out token).Returns(false);
            AuthenticateController controller = new AuthenticateController(authenticateService, mapper);

            // Act
            IActionResult result = controller.GetAuthToken(resource);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            BadRequestObjectResult badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Invalid authentication request.");
        }
    }
}
