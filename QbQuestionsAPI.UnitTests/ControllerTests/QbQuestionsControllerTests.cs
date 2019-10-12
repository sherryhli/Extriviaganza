using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

using QbQuestionsAPI.Controllers;
using QbQuestionsAPI.Domain.Models;
using QbQuestionsAPI.Domain.Services;
using QbQuestionsAPI.Domain.Services.Communication;
using QbQuestionsAPI.Resources;

namespace QbQuestionsAPI.UnitTests
{
    public class QbQuestionsControllerTests
    {
        private readonly IMapper mapper = Substitute.For<IMapper>();

        [Fact]
        public async Task GetAsyncSuccessTest()
        {
            // Arrange
            IQbQuestionService qbQuestionService = Substitute.For<IQbQuestionService>();
            QbQuestion question = new QbQuestion();
            qbQuestionService.GetAsync(Arg.Any<int>()).Returns(question);
            QbQuestionsController controller = new QbQuestionsController(qbQuestionService, mapper);

            // Act
            IActionResult result = await controller.GetAsync(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetAsyncFailureTest()
        {
            // Arrange
            IQbQuestionService qbQuestionService = Substitute.For<IQbQuestionService>();
            QbQuestion question = null;
            qbQuestionService.GetAsync(Arg.Any<int>()).Returns(question);
            QbQuestionsController controller = new QbQuestionsController(qbQuestionService, mapper);

            // Act
            IActionResult result = await controller.GetAsync(1);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetRandomAsyncSuccessTest()
        {
            // Arrange
            IQbQuestionService qbQuestionService = Substitute.For<IQbQuestionService>();
            QbQuestion question = new QbQuestion();
            qbQuestionService.GetRandomAsync(Arg.Any<int?>()).Returns(question);
            QbQuestionsController controller = new QbQuestionsController(qbQuestionService, mapper);

            // Act
            IActionResult result = await controller.GetRandomAsync(null);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetRandomAsyncFailureTest()
        {
            // Arrange
            IQbQuestionService qbQuestionService = Substitute.For<IQbQuestionService>();
            QbQuestion question = null;
            qbQuestionService.GetRandomAsync(Arg.Any<int?>()).Returns(question);
            QbQuestionsController controller = new QbQuestionsController(qbQuestionService, mapper);

            // Act
            IActionResult result = await controller.GetRandomAsync(null);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task PostAsyncSuccessTest()
        {
            // Arrange
            SaveQbQuestionResource[] resources = { new SaveQbQuestionResource() };
            List<QbQuestion> questions = new List<QbQuestion> { new QbQuestion() };
            mapper.Map<List<SaveQbQuestionResource>, List<QbQuestion>>(Arg.Any<List<SaveQbQuestionResource>>()).Returns(questions);
            IQbQuestionService qbQuestionService = Substitute.For<IQbQuestionService>();
            QbQuestionResponse response = new QbQuestionResponse(new QbQuestion());
            qbQuestionService.SaveAsync(Arg.Any<QbQuestion>()).Returns(response);
            QbQuestionsController controller = new QbQuestionsController(qbQuestionService, mapper);

            // Act
            IActionResult result = await controller.PostAsync(resources);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task PostAsyncFailureTest()
        {
            // Arrange
            SaveQbQuestionResource[] resources = { new SaveQbQuestionResource() };
            List<QbQuestion> questions = new List<QbQuestion> { new QbQuestion() };
            mapper.Map<List<SaveQbQuestionResource>, List<QbQuestion>>(Arg.Any<List<SaveQbQuestionResource>>()).Returns(questions);
            IQbQuestionService qbQuestionService = Substitute.For<IQbQuestionService>();
            QbQuestionResponse response = new QbQuestionResponse(string.Empty);
            qbQuestionService.SaveAsync(Arg.Any<QbQuestion>()).Returns(response);
            QbQuestionsController controller = new QbQuestionsController(qbQuestionService, mapper);

            // Act
            IActionResult result = await controller.PostAsync(resources);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateAsyncSuccessTest()
        {
            // Arrange
            SaveQbQuestionResource resource = new SaveQbQuestionResource();
            QbQuestion question = new QbQuestion();
            mapper.Map<SaveQbQuestionResource, QbQuestion>(Arg.Any<SaveQbQuestionResource>()).Returns(question);
            IQbQuestionService qbQuestionService = Substitute.For<IQbQuestionService>();
            QbQuestionResponse response = new QbQuestionResponse(question);
            qbQuestionService.UpdateAsync(Arg.Any<int>(), Arg.Any<QbQuestion>()).Returns(response);
            QbQuestionsController controller = new QbQuestionsController(qbQuestionService, mapper);

            // Act
            IActionResult result = await controller.PutAsync(1, resource);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task UpdateAsyncFailureTest()
        {
            // Arrange
            SaveQbQuestionResource resource = new SaveQbQuestionResource();
            QbQuestion question = new QbQuestion();
            mapper.Map<SaveQbQuestionResource, QbQuestion>(Arg.Any<SaveQbQuestionResource>()).Returns(question);
            IQbQuestionService qbQuestionService = Substitute.For<IQbQuestionService>();
            QbQuestionResponse response = new QbQuestionResponse(string.Empty);
            qbQuestionService.UpdateAsync(Arg.Any<int>(), Arg.Any<QbQuestion>()).Returns(response);
            QbQuestionsController controller = new QbQuestionsController(qbQuestionService, mapper);

            // Act
            IActionResult result = await controller.PutAsync(1, resource);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteAsyncSuccessTest()
        {
            // Arrange
            QbQuestion question = new QbQuestion();
            QbQuestionResponse response = new QbQuestionResponse(question);
            IQbQuestionService qbQuestionService = Substitute.For<IQbQuestionService>();
            qbQuestionService.DeleteAsync(Arg.Any<int>()).Returns(response);
            QbQuestionResource resource = new QbQuestionResource();
            mapper.Map<QbQuestion, QbQuestionResource>(Arg.Any<QbQuestion>()).Returns(resource);
            QbQuestionsController controller = new QbQuestionsController(qbQuestionService, mapper);

            // Act
            IActionResult result = await controller.DeleteAsync(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteAsyncFailureTest()
        {
            // Arrange
            QbQuestionResponse response = new QbQuestionResponse(string.Empty);
            IQbQuestionService qbQuestionService = Substitute.For<IQbQuestionService>();
            qbQuestionService.DeleteAsync(Arg.Any<int>()).Returns(response);
            QbQuestionsController controller = new QbQuestionsController(qbQuestionService, mapper);

            // Act
            IActionResult result = await controller.DeleteAsync(1);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
