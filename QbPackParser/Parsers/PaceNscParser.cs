using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using QbPackParser.Models;

namespace QbPackParser.Parsers
{
    public class PaceNscParser : IParser
    {
        private string _text;
        private string _level;
        private string _tournament;
        private int _year;

        private Dictionary<string, string> _levels = new Dictionary<string, string>()
        {
            {"HS", "High School"},
            {"MS", "Middle School"},
            {"C", "Collegiate"},
            {"T", "Trash"}
        };

        /// <summary>Creates a new instance of PaceNscParser</summary>
        /// <param name="text">The raw text of the question pack</param>
        /// <param name="level">The level of the question pack</param>
        /// <param name="tournament">The tournament the pack is from</param>
        /// <param name="year">The year of the tournament</param>
        public PaceNscParser(string text, string level, string tournament, int year)
        {
            _text = text;
            _level = _levels[level];
            _tournament = tournament;
            _year = year;
        }

        /// <summary>Cleans _text so that it is ready to be split into individual questions</summary>
        private void CleanText()
        {
            const string zeroWidthSpace = "\u200B";
            const string pageBreak = @"\f";
            const string header = @"NSC [0-9]{4} - Round [0-9]{2} - Tossups\s+(This round is sponsored by ([A-Za-z]+\s*)+)?";
            const string credits = "<.*>";
            const string bonusPageSeparator = "NSC [0-9]{4} - Round [0-9]{2} - Bonuses";
            const string pageSeparator = "NSC [0-9]{4} - Round [0-9]{2} - Page [0-9]+ of [0-9]+";

            _text = _text.Trim();
            _text = _text.Replace(zeroWidthSpace, String.Empty);
            _text = Regex.Replace(_text, pageBreak, String.Empty);
            _text = Regex.Replace(_text, header, String.Empty);
            _text = Regex.Replace(_text, credits, String.Empty);
            _text = Regex.Split(_text, bonusPageSeparator)[0];
            _text = Regex.Replace(_text, pageSeparator, String.Empty);
        }

        /// <summary>Parses the raw text of the questions to obtain relevant information</summary>
        /// <returns>JSON representation of the questions in an array</returns>
        public string Parse()
        {
            CleanText();

            const string questionSeparator = @"\s{2,}[0-9]+\. ";
            const string bonusSeparator = @"\(\*\)";
            const string firstQuestionSeparator = @"1\. ";
            const string answerSeparator = "ANSWER: ";
            const string notesPattern = @"\[(.|\n|\r)*\]";

            List<string> tossups = Regex.Split(_text, questionSeparator).ToList();
            tossups[0] = Regex.Replace(tossups[0], firstQuestionSeparator, String.Empty);

            // foreach loop does not allow assignment of individual elements
            for (int i = 0; i < tossups.Count; i++)
            {
                tossups[i] = tossups[i].Replace("\n", "");
                tossups[i] = tossups[i].Replace("\r", "");
                tossups[i] = tossups[i].Replace("\r\n", "");
            }

            List<QbQuestion> jsonQuestions = new List<QbQuestion>();

            foreach (string tossup in tossups)
            {
                string[] tossupParts = Regex.Split(tossup, bonusSeparator);
                string bonus = tossupParts[0].Trim();
                // Console.WriteLine("BONUS: " + bonus);
                string bodyAnswerNotes = tossupParts[1];
                string body = Regex.Split(bodyAnswerNotes, answerSeparator)[0].Trim();
                // Console.WriteLine("BODY: " + body);
                string answerNotes = Regex.Split(bodyAnswerNotes, answerSeparator)[1];
                string answer = Regex.Split(answerNotes, notesPattern)[0].Trim();
                // Console.WriteLine("ANSWER: " + answer);
                string notes = Regex.Match(answerNotes, notesPattern).ToString().Trim();
                // Console.WriteLine("NOTES: " + notes);
                // Console.WriteLine("\n");

                QbQuestion question = new QbQuestion
                {
                    Level = _level,
                    Tournament = _tournament,
                    Year = _year,
                    Bonus = bonus,
                    Body = body,
                    Answer = answer,
                    Notes = notes
                };

                jsonQuestions.Add(question);
            }

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    OverrideSpecifiedNames = false
                }
            };

            return JsonConvert.SerializeObject(jsonQuestions, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });
        }
    }
}
