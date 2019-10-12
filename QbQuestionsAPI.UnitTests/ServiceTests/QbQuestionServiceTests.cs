using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Threading.Tasks;
using Xunit;

using QbQuestionsAPI.Domain.Repositories;
using QbQuestionsAPI.Domain.Models;
using QbQuestionsAPI.Domain.Services.Communication;
using QbQuestionsAPI.Services;

namespace QbQuestionsAPI.UnitTests.ServiceTests
{
    public class QbQuestionServiceTests
    {
        [Fact]
        public void SaveAsyncSuccessTest()
        {
            // Arrange
            IQbQuestionRepository repository = Substitute.For<IQbQuestionRepository>();
            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            repository.AddAsync(Arg.Any<QbQuestion>()).Returns(Task.CompletedTask);
            unitOfWork.CompleteAsync().Returns(Task.CompletedTask);
            QbQuestionService service = new QbQuestionService(repository, unitOfWork);
            QbQuestion question = new QbQuestion();

            // Act
            Task<QbQuestionResponse> result = service.SaveAsync(question);

            // Assert
            QbQuestionResponse response = new QbQuestionResponse(question);
            result.Result.Success.Should().Be(true);
        }

        [Fact]
        public void SaveAsyncFailureOnAddTest()
        {
            // Arrange
            IQbQuestionRepository repository = Substitute.For<IQbQuestionRepository>();
            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            repository.AddAsync(Arg.Any<QbQuestion>()).Throws(new Exception("Exception occurred on AddAsync"));
            QbQuestionService service = new QbQuestionService(repository, unitOfWork);
            QbQuestion question = new QbQuestion();

            // Act
            Task<QbQuestionResponse> result = service.SaveAsync(question);

            // Assert
            string errorMessage = "An error occurred when saving the question: Exception occurred on AddAsync";
            result.Result.Success.Should().Be(false);
            result.Result.Message.Should().Be(errorMessage);
        }

        [Fact]
        public void SaveAsyncFailureOnSaveTest()
        {
            // Arrange
            IQbQuestionRepository repository = Substitute.For<IQbQuestionRepository>();
            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            repository.AddAsync(Arg.Any<QbQuestion>()).Returns(Task.CompletedTask);
            unitOfWork.CompleteAsync().Throws(new Exception("Exception occurred on CompleteAsync"));
            QbQuestionService service = new QbQuestionService(repository, unitOfWork);
            QbQuestion question = new QbQuestion();

            // Act
            Task<QbQuestionResponse> result = service.SaveAsync(question);

            // Assert
            string errorMessage = "An error occurred when saving the question: Exception occurred on CompleteAsync";
            result.Result.Success.Should().Be(false);
            result.Result.Message.Should().Be(errorMessage);
        }

        [Fact]
        public void UpdateAsyncSuccessTest()
        {
            // Arrange
            IQbQuestionRepository repository = Substitute.For<IQbQuestionRepository>();
            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            QbQuestion question = new QbQuestion();
            repository.FindByIdAsync(Arg.Any<int>()).Returns(question);
            repository.Update(Arg.Any<QbQuestion>()).Returns(true);
            unitOfWork.CompleteAsync().Returns(Task.CompletedTask);
            QbQuestionService service = new QbQuestionService(repository, unitOfWork);

            // Act
            Task<QbQuestionResponse> result = service.UpdateAsync(1, question);

            // Assert
            result.Result.Success.Should().Be(true);
        }

        [Fact]
        public void UpdateAsyncFailureOnFindTest()
        {
            // Arrange
            IQbQuestionRepository repository = Substitute.For<IQbQuestionRepository>();
            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            QbQuestion question = null;
            repository.FindByIdAsync(Arg.Any<int>()).Returns(question);
            QbQuestionService service = new QbQuestionService(repository, unitOfWork);

            // Act
            Task<QbQuestionResponse> result = service.UpdateAsync(1, question);

            // Assert
            string errorMessage = "Question not found.";
            result.Result.Success.Should().Be(false);
            result.Result.Message.Should().Be(errorMessage);
        }

        [Fact]
        public void UpdateAsyncFailureOnUpdateTest()
        {
            // Arrange
            IQbQuestionRepository repository = Substitute.For<IQbQuestionRepository>();
            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            QbQuestion question = new QbQuestion();
            repository.FindByIdAsync(Arg.Any<int>()).Returns(question);
            repository.Update(Arg.Any<QbQuestion>()).Throws(new Exception("Exception occurred on Update"));
            QbQuestionService service = new QbQuestionService(repository, unitOfWork);

            // Act
            Task<QbQuestionResponse> result = service.UpdateAsync(1, question);

            // Assert
            string errorMessage = "An error occurred when updating the question: Exception occurred on Update";
            result.Result.Success.Should().Be(false);
            result.Result.Message.Should().Be(errorMessage);
        }

        [Fact]
        public void UpdateAsyncFailureOnSaveTest()
        {
            // Arrange
            IQbQuestionRepository repository = Substitute.For<IQbQuestionRepository>();
            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            QbQuestion question = new QbQuestion();
            repository.FindByIdAsync(Arg.Any<int>()).Returns(question);
            repository.Update(Arg.Any<QbQuestion>()).Returns(true);
            unitOfWork.CompleteAsync().Throws(new Exception("Exception occurred on CompleteAsync"));
            QbQuestionService service = new QbQuestionService(repository, unitOfWork);

            // Act
            Task<QbQuestionResponse> result = service.UpdateAsync(1, question);

            // Assert
            string errorMessage = "An error occurred when updating the question: Exception occurred on CompleteAsync";
            result.Result.Success.Should().Be(false);
            result.Result.Message.Should().Be(errorMessage);
        }
    }
}