using System.Collections.Generic;
using System.Reflection.Metadata;

namespace TextAnalLab2.Models
{
    public class NgramProb
    {
        public IEnumerable<string> Ngrams  { get; set; }
        public decimal Probability { get; set; }
    }
}