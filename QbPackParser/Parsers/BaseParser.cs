using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

using QbPackParser.Models;

namespace QbPackParser.Parsers
{
    // TODO: Further abstract parser properties into this class
    public abstract class BaseParser
    {
        protected string text { get; set; }
        protected int level { get; set; }
        protected string tournament { get; set; }
        protected int year { get; set; }

        // TODO: Consider changing to Enum
        private Dictionary<string, int> levels = new Dictionary<string, int>()
        {
            {"MS", 1},
            {"HS", 2},
            {"C", 3},
            {"T", 4}
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

        /// <summary>Serializes a list of QbQuestions into JSON</summary>
        /// <returns>JSON representation of the questions in an array</returns>
        protected string SerializeQuestionsToJson(List<QbQuestion> questions)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    OverrideSpecifiedNames = false
                }
            };

            return JsonConvert.SerializeObject(questions, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });
        }
    }
}
