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
    public class QbQuestionsControllerTest
    {
        private readonly IMapper mapper = Substitute.For<IMapper>();

        [Fact]
        public async Task GetAsyncSuccessTest()
        {
            QbQuestion question = new QbQuestion
            {
                Level = ETournamentLevel.Collegiate,
                Tournament = "",
                Year = 2019,
                Power = "",
                Body = "",
                Answer = "",
                Notes = ""
            };

            IQbQuestionService qbQuestionService = Substitute.For<IQbQuestionService>();
            QbQuestionsController controller = new QbQuestionsController(qbQuestionService, mapper);
            qbQuestionService.GetAsync(Arg.Any<int>()).Returns(question);
            var result = await controller.GetAsync(1);
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetAsyncFailureTest()
        {
            QbQuestion question = null;
            IQbQuestionService qbQuestionService = Substitute.For<IQbQuestionService>();
            QbQuestionsController controller = new QbQuestionsController(qbQuestionService, mapper);
            qbQuestionService.GetAsync(Arg.Any<int>()).Returns(question);
            var result = await controller.GetAsync(1);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetRandomAsyncSuccessTest()
        {
            QbQuestion question = new QbQuestion
            {
                Level = ETournamentLevel.Collegiate,
                Tournament = "",
                Year = 2019,
                Power = "",
                Body = "",
                Answer = "",
                Notes = ""
            };

            IQbQuestionService qbQuestionService = Substitute.For<IQbQuestionService>();
            QbQuestionsController controller = new QbQuestionsController(qbQuestionService, mapper);
            qbQuestionService.GetRandomAsync(Arg.Any<int?>()).Returns(question);
            var result = await controller.GetRandomAsync(null);
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetRandomAsyncFailureTest()
        {
            QbQuestion question = null;
            IQbQuestionService qbQuestionService = Substitute.For<IQbQuestionService>();
            QbQuestionsController controller = new QbQuestionsController(qbQuestionService, mapper);
            qbQuestionService.GetRandomAsync(Arg.Any<int?>()).Returns(question);
            var result = await controller.GetRandomAsync(null);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task PostAsyncSuccessTest()
        {
            SaveQbQuestionResource resource = new SaveQbQuestionResource
            {
                Level = 1,
                Tournament = "",
                Year = 2019,
                Power = "",
                Body = "",
                Answer = "",
                Notes = ""
            };
            SaveQbQuestionResource[] resources = { resource };
            QbQuestion question = null;
            List<QbQuestion> questions = new List<QbQuestion>
            {
                question
            };
            QbQuestionResponse response = new QbQuestionResponse(question);

            mapper.Map<List<SaveQbQuestionResource>, List<QbQuestion>>(Arg.Any<List<SaveQbQuestionResource>>()).Returns(questions);
            IQbQuestionService qbQuestionService = Substitute.For<IQbQuestionService>();
            qbQuestionService.SaveAsync(Arg.Any<QbQuestion>()).Returns(response);
            QbQuestionsController controller = new QbQuestionsController(qbQuestionService, mapper);
            var result = await controller.PostAsync(resources);
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
