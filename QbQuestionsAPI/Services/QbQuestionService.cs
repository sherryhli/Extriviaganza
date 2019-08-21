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

        public async Task<QbQuestion> GetRandomAsync(int? level)
        {
            return await _qbQuestionRepository.GetRandomAsync(level);
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

        public async Task<QbQuestionResponse> UpdateAsync(int id, QbQuestion qbQuestion)
        {
            QbQuestion oldQuestion = await _qbQuestionRepository.FindByIdAsync(id);

            if (oldQuestion == null)
            {
                return new QbQuestionResponse("Question not found.");
            }

            oldQuestion.Level = qbQuestion.Level;
            oldQuestion.Tournament = qbQuestion.Tournament;
            oldQuestion.Year = qbQuestion.Year;
            oldQuestion.Power = qbQuestion.Power;
            oldQuestion.Body = qbQuestion.Body;
            oldQuestion.Answer = qbQuestion.Answer;
            oldQuestion.Notes = qbQuestion.Notes;

            try
            {
                _qbQuestionRepository.Update(oldQuestion);
                await _unitOfWork.CompleteAsync();

                return new QbQuestionResponse(oldQuestion);
            }
            catch (Exception ex)
            {
                return new QbQuestionResponse($"An error occurred when updating the question: {ex.Message}");
            }
        }

        public async Task<QbQuestionResponse> DeleteAsync(int id)
        {
            QbQuestion oldQuestion = await _qbQuestionRepository.FindByIdAsync(id);

            if (oldQuestion == null)
            {
                return new QbQuestionResponse("Question not found.");
            }

            try
            {
                _qbQuestionRepository.Remove(oldQuestion);
                await _unitOfWork.CompleteAsync();

                return new QbQuestionResponse(oldQuestion);
            }
            catch (Exception ex)
            {
                return new QbQuestionResponse($"An error occurred when deleting the question: {ex.Message}");
            }
        }
    }
}