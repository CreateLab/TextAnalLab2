using System.Collections.Generic;
using System.Linq;
using TextAnalLab2.Models;

namespace TextAnalLab2
{
    public class NGramProbBuilder
    {
        public IEnumerable<NgramProb> Count(IEnumerable<IEnumerable<string>> ngrams)
        {
            var enumerable1 = ngrams as IEnumerable<string>[] ?? ngrams.ToArray();
            var probNGrams = new Dictionary<string, NgramProb>(enumerable1.Length);
            foreach (var ngram in enumerable1)
            {
                var enumerable = ngram as string[] ?? ngram.ToArray();
                var key = string.Join(" ", enumerable);
                if (probNGrams.ContainsKey(key))
                {
                    probNGrams[key].Count++;
                }
                else
                {
                    probNGrams.Add(key, new NgramProb
                    {
                        Ngram = enumerable,
                        Count = 1
                    });
                }
            }

            return probNGrams.Values.OrderByDescending(x => x.Count);
        }

       
    }
}