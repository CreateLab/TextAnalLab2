using System.Collections.Generic;
using System.Linq;
using TextAnalLab2.Models;

namespace TextAnalLab2
{
    public class NGramProbBuilder
    {
        public IEnumerable<NgramProb> CreateTextProbability(IEnumerable<IEnumerable<string>> ngrams)
        {
            var enumerable1 = ngrams as IEnumerable<string>[] ?? ngrams.ToArray();
            var probNGrams = new Dictionary<string, NgramProb>(enumerable1.Length);
            foreach (var ngram in enumerable1)
            {
                var enumerable = ngram as string[] ?? ngram.ToArray();
                var key = string.Join(" ", enumerable);
                if (probNGrams.ContainsKey(key))
                {
                    probNGrams[key].Probability++;
                }
                else
                {
                    probNGrams.Add(key, new NgramProb
                    {
                        Ngrams = enumerable,
                        Probability = 1
                    });
                }
            }

            return probNGrams.Values.OrderByDescending(x => x.Probability);
        }

        public IEnumerable<NgramProb> CreateLaplasProbability(IEnumerable<NgramProb> enumerable, int totalCount,
            int tocalVoc)
        {
            return enumerable.Select(x => new NgramProb
            {
                Ngrams = x.Ngrams,
                Probability = (x.Probability + 1) / (tocalVoc + totalCount)
            });
        }
    }
}