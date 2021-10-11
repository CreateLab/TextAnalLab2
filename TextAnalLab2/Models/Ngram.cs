using System;
using System.Collections.Generic;
using System.Linq;

namespace TextAnalLab2.Models
{
    public class Ngram
    {
        public IEnumerable<string> NgramElems { get; set; }

        /// <inheritdoc />
        public override int GetHashCode()
        {
           return string.Join(" ", NgramElems).GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj as Ngram == null)
            {
                return false;
            }
            return string.Join(" ", NgramElems) == string.Join(" ", (obj as Ngram).NgramElems);
        }

        public bool StartWith(IEnumerable<string> enumerable)
        {
            if (enumerable.Count() != NgramElems.Count() - 1) return false;

            if (NgramElems.First() == "никакого")
            {
                
            }
            var value = string.Join(" ",enumerable);
            var s =string.Join(" ", NgramElems);
            return s.StartsWith(value);
        }
    }
}