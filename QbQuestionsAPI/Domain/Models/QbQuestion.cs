namespace QbQuestionsAPI.Domain.Models
{
    public class QbQuestion
    {
        public int Id { get; set; }
        public ETournamentLevel Level { get; set; }
        public string Tournament { get; set; }
        public int Year { get; set; }
        public string Power { get; set; }
        public string Body { get; set; }
        public string Answer { get; set; }
        public string Notes { get; set; }
    }
}