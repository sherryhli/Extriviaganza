using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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

        }

        /// <summary>Parses the raw text of the questions to obtain relevant information</summary>
        /// <returns>JSON representation of the questions in an array</returns>
        public override string Parse() {
            return String.Empty;
        }
    }
}
