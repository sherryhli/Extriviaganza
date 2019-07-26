using System.Collections.Generic;

namespace QbPackParser.Parsers
{
    public abstract class BaseParser
    {
        protected string text { get; set; }
        protected string level { get; set; }
        protected string tournament { get; set; }
        protected int year { get; set; }

        private Dictionary<string, string> levels = new Dictionary<string, string>()
        {
            {"HS", "High School"},
            {"MS", "Middle School"},
            {"C", "Collegiate"},
            {"T", "Trash"}
        };

        /// <summary>Initalizes fields of the BaseParser</summary>
        /// <param name="text">The raw text of the question pack</param>
        /// <param name="level">The level of the question pack</param>
        /// <param name="tournament">The tournament the pack is from</param>
        /// <param name="year">The year of the tournament</param>
        public BaseParser(string text, string level, string tournament, int year)
        {
            this.text = text;
            this.level = levels[level];
            this.tournament = tournament;
            this.year = year;
        }

        /// <summary>Cleans text so that it is ready to be split into individual questions</summary>
        protected abstract void CleanText();

        /// <summary>Parses the raw text of the questions to obtain relevant information</summary>
        /// <returns>JSON representation of the questions in an array</returns>
        public abstract string Parse();
    }
}
