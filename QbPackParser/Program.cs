﻿using System;
using System.IO;
using System.Text;

using QbPackParser.Parsers;

namespace QbPackParser
{
    public class Program
    {
        // 1st arg is the name of the parser to use
        // 2nd arg is the full path to the .txt quizbowl pack
        // 3rd arg is the level, e.g. Collegiate (C), High School (HS), Middle School (MS), Trash (T)
        // 4th arg is tournament name
        // 5th arg is year of tournament
        public static void Main(string[] args)
        {
            if (args.Length != 5)
            {
                throw new Exception("Wrong number of arguments, expected 5, received " + args.Length.ToString());
            }

            string text = File.ReadAllText(args[1], Encoding.UTF8);
            string level = args[2];
            string tournament = args[3];
            int year = Int32.Parse(args[4]);

            if (args[0].ToLower() == "pace")
            {
                IParser parser = new PaceNscParser(text, level, tournament, year);
                string result = parser.Parse();
                Console.WriteLine(result);
            }
            // TODO: other parsers
        }
    }
}
