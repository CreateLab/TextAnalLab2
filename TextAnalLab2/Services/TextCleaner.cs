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
            return data.Split().Where(x => _regex.IsMatch(x)).Select(x => ClearString(x.ToLowerInvariant()));
        }

        private string ClearString(string s)
        {
            return Regex.Replace(s, @"[\W]", x => Regex.IsMatch(x.Value, @"\s") ? " " : string.Empty);
        }

        public IEnumerable<string> SplitToSentence(string data)
        {
            return data.Split(".", StringSplitOptions.RemoveEmptyEntries).Select(ClearString);
        }
    }
}