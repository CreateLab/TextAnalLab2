using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TextAnalLab2
{
    public class TextCleaner
    {
        private readonly Regex _regex = new Regex(@"\w+");

        public IEnumerable<string> Clear(string data)
        {
            return data.Split().Where(x => _regex.IsMatch(x)).Select(x => ClearWord(x.ToLowerInvariant()));
        }

        private string ClearWord(string toLowerInvariant)
        {
            var spes = new[]
            {
                "“", "”", "-", "’", "‘", "\"", "<", ">", "—", "[", "]", ";", "*", ":", ".", "\n", ".", "\"",
                "–", "*", "«", "»", "=", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
                ",", "(", ")", "!", "?" , "…"
            };

            return spes.Aggregate(toLowerInvariant, (current, s) => current.Replace(s, string.Empty));
        }
    }
}