using FluentAssertions;
using NSubstitute;
using Xunit;

using QbQuestionsAPI.Domain.Models;
using QbQuestionsAPI.Domain.Repositories;
using QbQuestionsAPI.Domain.Services;
using QbQuestionsAPI.Services;

namespace QbQuestionsAPI.UnitTests.ServiceTests
{
    public class UserServiceTests
    {
        [Fact]
        public void IsValidUserSuccessTest()
        {
            // Arrange
            IUserRepository userRepository = Substitute.For<IUserRepository>();
            userRepository.IsValidUser(Arg.Any<User>()).Returns(true);
            IUserService userService = new UserService(userRepository);
            User user = new User();

            // Act
            bool result = userService.IsValidUser(user);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public void IsValidUserFailureTest()
        {
            // Arrange
            IUserRepository userRepository = Substitute.For<IUserRepository>();
            userRepository.IsValidUser(Arg.Any<User>()).Returns(false);
            IUserService userService = new UserService(userRepository);
            User user = new User();

            // Act
            bool result = userService.IsValidUser(user);

            // Assert
            result.Should().Be(false);
        }
    }
}