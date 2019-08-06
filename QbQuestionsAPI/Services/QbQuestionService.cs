using System;
using System.Threading.Tasks;

using QbQuestionsAPI.Domain.Models;
using QbQuestionsAPI.Domain.Repositories;
using QbQuestionsAPI.Domain.Services;
using QbQuestionsAPI.Domain.Services.Communication;

namespace QbQuestionsAPI.Services
{
    public class QbQuestionService : IQbQuestionService
    {
        private readonly IQbQuestionRepository _qbQuestionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public QbQuestionService(IQbQuestionRepository qbQuestionRepository, IUnitOfWork unitOfWork)
        {
            _qbQuestionRepository = qbQuestionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<QbQuestion> GetAsync(int id)
        {
            return await _qbQuestionRepository.FindByIdAsync(id);
        }

        public async Task<QbQuestionResponse> SaveAsync(QbQuestion qbQuestion)
        {
            try
            {
                await _qbQuestionRepository.AddAsync(qbQuestion);
                await _unitOfWork.CompleteAsync();

                return new QbQuestionResponse(qbQuestion);
            }
            catch (Exception ex)
            {
                return new QbQuestionResponse($"An error occurred when saving the question: {ex.Message}");
            }
        }
    }
}