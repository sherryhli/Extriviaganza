using QbQuestionsAPI.Domain.Models;

namespace QbQuestionsAPI.Domain.Services.Communication
{
    public class QbQuestionResponse
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }
        public QbQuestion QbQuestion { get; private set; }
        
        private QbQuestionResponse(bool success, string message, QbQuestion qbQuestion)
        {
            Success = success;
            Message = message;
            QbQuestion = qbQuestion;
        }

        public QbQuestionResponse(QbQuestion qbQuestion) : this(true, string.Empty, qbQuestion)
        {

        }

        public QbQuestionResponse(string message) : this(false, message, null)
        {
            
        }
    }
}