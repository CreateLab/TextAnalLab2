using System.Collections.Generic;
using System.Reflection.Metadata;

namespace TextAnalLab2.Models
{
    public class NgramProb
    {
        public IEnumerable<string> Ngram  { get; set; }
        public decimal Count { get; set; }
    }
}