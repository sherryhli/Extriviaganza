namespace QbPackParser.Models
{
    public class QbQuestion
    {
        public string Level { get; set; }
        public string Tournament { get; set; }
        public int Year { get; set; }
        public int RoundNumber { get; set; }
        public string Bonus { get; set; }
        public string Body { get; set; }
        public string Answer { get; set; }
        public string Notes { get; set; }
    }
}
