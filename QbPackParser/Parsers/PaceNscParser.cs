using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using QbPackParser.Models;

namespace QbPackParser.Parsers
{
    public class PaceNscParser : BaseParser
    {
        /// <summary>Creates a new instance of PaceNscParser</summary>
        /// <param name="text">The raw text of the question pack</param>
        /// <param name="level">The level of the question pack</param>
        /// <param name="tournament">The tournament the pack is from</param>
        /// <param name="year">The year of the tournament</param>
        public PaceNscParser(string text, string level, string tournament, int year) : base(text, level, tournament, year)
        {

        }

        /// <summary>Cleans text so that it is ready to be split into individual questions</summary>
        protected override void CleanText()
        {
            const string zeroWidthSpace = "\u200B";
            const string pageBreak = @"\f";
            const string tossupHeader = @"NSC [0-9]{4} - Round [0-9]{2} - Tossups";
            const string sponsorHeader = @"This round is sponsored by ([^\s]+\s{0,1})+";
            const string credits = "<.*>";
            const string bonusPageSeparator = "NSC [0-9]{4} - Round [0-9]{2} - Bonuses";
            const string pageSeparator = "NSC [0-9]{4} - Round [0-9]{2} - Page [0-9]+ of [0-9]+";

            base.text = base.text.Trim();
            base.text = base.text.Replace(zeroWidthSpace, String.Empty);
            base.text = Regex.Replace(base.text, pageBreak, String.Empty);
            base.text = Regex.Replace(base.text, tossupHeader, String.Empty);
            base.text = Regex.Replace(base.text, sponsorHeader, String.Empty);
            base.text = Regex.Replace(base.text, credits, String.Empty);
            base.text = Regex.Split(base.text, bonusPageSeparator)[0];
            base.text = Regex.Replace(base.text, pageSeparator, String.Empty);
        }

        /// <summary>Parses the raw text of the questions to obtain relevant information</summary>
        /// <returns>JSON representation of the questions in an array</returns>
        public override string Parse()
        {
            CleanText();

            const string questionSeparator = @"\s{5,}[0-9]+\. ";
            const string bonusSeparator = @"\(\*\)";
            const string firstQuestionSeparator = @"1\. ";
            const string answerSeparator = "ANSWER: ";
            const string notesPattern = @"\[(.|\n|\r)*\]";

            List<string> tossups = Regex.Split(base.text, questionSeparator).ToList();
            if (tossups[0] == "") {
                tossups.RemoveAt(0);
            }
            tossups[0] = Regex.Replace(tossups[0], firstQuestionSeparator, String.Empty);

            // foreach loop does not allow assignment of individual elements
            for (int i = 0; i < tossups.Count; i++)
            {
                tossups[i] = tossups[i].Replace("\n", String.Empty);
                tossups[i] = tossups[i].Replace("\r", String.Empty);
                tossups[i] = tossups[i].Replace("\r\n", String.Empty);
            }

            List<QbQuestion> jsonQuestions = new List<QbQuestion>();

            foreach (string tossup in tossups)
            {
                string[] tossupParts = Regex.Split(tossup, bonusSeparator);
                string bonus = tossupParts[0].Trim();
                string bodyAnswerNotes = tossupParts[1];
                string body = Regex.Split(bodyAnswerNotes, answerSeparator)[0].Trim();
                string answerNotes = Regex.Split(bodyAnswerNotes, answerSeparator)[1];
                string answer = Regex.Split(answerNotes, notesPattern)[0].Trim();
                string notes = Regex.Match(answerNotes, notesPattern).ToString().Trim();

                QbQuestion question = new QbQuestion
                {
                    Level = base.level,
                    Tournament = base.tournament,
                    Year = base.year,
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
