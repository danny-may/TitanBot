using Discord.Commands;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TitanBot2.TypeReaders
{
    public class BetterTimespanTypeReader : Discord.Commands.TypeReader
    {   
        public override Task<TypeReaderResult> Read(ICommandContext context, string input)
        {
            var days = 0;
            var hours = 0;
            var minutes = 0;
            var seconds = 0;
            var match = false;

            var colon = Regex.Match(input, @"(\d{1,2}([:\.]\d{1,2}){1,4}|^\d+$)");
            if (colon.Success)
            {
                var matchedString = colon.Value.Split(new char[] { ':', '.' });
                match = true;
                switch (matchedString.Length)
                {
                    case 1:
                        minutes = int.Parse(matchedString[0]);
                        break;
                    case 2:
                        hours = int.Parse(matchedString[0]);
                        minutes = int.Parse(matchedString[1]);
                        break;
                    case 3:
                        hours = int.Parse(matchedString[0]);
                        minutes = int.Parse(matchedString[1]);
                        seconds = int.Parse(matchedString[2]);
                        break;
                    case 4:
                        days = int.Parse(matchedString[0]);
                        hours = int.Parse(matchedString[1]);
                        minutes = int.Parse(matchedString[2]);
                        seconds = int.Parse(matchedString[3]);
                        break;
                    case 5:
                        days = int.Parse(matchedString[0]);
                        hours = int.Parse(matchedString[1]);
                        minutes = int.Parse(matchedString[2]);
                        seconds = int.Parse(matchedString[3]);
                        break;
                }
            }
            else
            {
                var capture = Regex.Match(input, @"\d{1,2} ?(?=\s*(days|day|d))", RegexOptions.IgnoreCase);
                if (capture.Success)
                {
                    match = true;
                    days = int.Parse(capture.Value);
                }
                capture = Regex.Match(input, @"\d{1,2} ?(?=\s*(hours|hour|hrs|hr|h))", RegexOptions.IgnoreCase);
                if (capture.Success)
                {
                    match = true;
                    hours = int.Parse(capture.Value);
                }
                capture = Regex.Match(input, @"\d{1,2} ?(?=\s*(minutes|minute|mins|min|m))", RegexOptions.IgnoreCase);
                if (capture.Success)
                {
                    match = true;
                    minutes = int.Parse(capture.Value);
                }
                capture = Regex.Match(input, @"\d{1,2} ?(?=\s*(seconds|second|secs|sec|s))", RegexOptions.IgnoreCase);
                if (capture.Success)
                {
                    match = true;
                    seconds = int.Parse(capture.Value);
                }
            }

            if (match)
            {
                return Task.FromResult(TypeReaderResult.FromSuccess(new TimeSpan(days, hours, minutes, seconds)));
            }
            else
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input could not be parsed as a TimeSpan"));
            }
        }
    }
}
