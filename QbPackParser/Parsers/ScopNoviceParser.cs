using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using QbPackParser.Models;

namespace QbPackParser.Parsers
{
    public class ScopNoviceParser : BaseParser
    {
        /// <summary>Creates a new instance of ScopNoviceParser</summary>
        /// <param name="text">The raw text of the question pack</param>
        /// <param name="level">The level of the question pack</param>
        /// <param name="tournament">The tournament the pack is from</param>
        /// <param name="year">The year of the tournament</param>
        public ScopNoviceParser(string text, string level, string tournament, int year) : base(text, level, tournament, year)
        {

        }

        /// <summary>Cleans text so that it is ready to be split into individual questions</summary>
        protected override void CleanText()
        {
            const string zeroWidthSpace = "\u200B";
            const string header = @"[0-9]{4} SCOP Novice [0-9]+\r\n\r\n((Round [0-9]+)|Replacements)\r\n\r\n[A-Za-z\s\r\n•]+Tossups";
            const string pageSeparator = "\fSCOP Novice 9 · ((Round [0-9]+)|Replacements)\r\nPage [0-9]+ of [0-9]+";
            const string bonusPageSeparator = "Bonuses|BONUSES";
            const string pageBreak = @"\f";

            base.text = base.text.Replace(zeroWidthSpace, String.Empty);
            base.text = Regex.Replace(base.text, header, String.Empty);
            base.text = Regex.Replace(base.text, pageSeparator, String.Empty);
            base.text = Regex.Split(base.text, bonusPageSeparator)[0];
            base.text = base.text.Replace(pageBreak, String.Empty);
        }

        // TODO: Abstract more functionalities into the base class
        /// <summary>Parses the raw text of the questions to obtain relevant information</summary>
        /// <returns>JSON representation of the questions in an array</returns>
        public override string Parse()
        {
            CleanText();

            const string questionSeparator = @"\([0-9]+\) ";
            const string powerSeparator = @"\(\*\)";
            const string answerSeparator = "ANSWER: ";
            const string notesPattern = @"\((.|\n|\r)*\)";

            List<string> tossups = Regex.Split(base.text, questionSeparator).ToList();
            if (tossups[0] == "\r\n\r\n")
            {
                tossups.RemoveAt(0);
            }

            List<QbQuestion> questions = new List<QbQuestion>();

            foreach (string tossup in tossups)
            {
                string[] tossupParts = Regex.Split(tossup, powerSeparator);
                string power = tossupParts[0].Trim();
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
                    Power = power,
                    Body = body,
                    Answer = answer,
                    Notes = notes
                };

                questions.Add(question);
            }
            return SerializeQuestionsToJson(questions);
        }
    }
}
