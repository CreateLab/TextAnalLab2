using System;
using System.Collections.Generic;
using System.Linq;
using TextAnalLab2.Models;

namespace TextAnalLab2
{
    public class Predicter
    {
        public string PredictNextWord(IEnumerable<IEnumerable<string>>[] inputNgramCollection,
            IEnumerable<NgramProb>[] ngramProbCollection)
        {
            var repCollection = PredictNextWords(inputNgramCollection, ngramProbCollection);
            return repCollection.OrderByDescending(x => x.prop).Select(x => x.name).Distinct()
                .Take(new Random().Next(1, 4))
                .Aggregate(string.Empty, (x, y) => x.Trim() + " " + y);
        }

        private IEnumerable<(string name, decimal prop)> PredictNextWords(
            IEnumerable<IEnumerable<string>>[] inputNgramCollection,
            IEnumerable<NgramProb>[] ngramProbCollection)
        {
            return inputNgramCollection.SelectMany(ngrams => PredictNextWordForNgram(ngrams, ngramProbCollection));
        }

        private IEnumerable<(string name, decimal prop)> PredictNextWordForNgram(
            IEnumerable<IEnumerable<string>> ngrams,
            IEnumerable<NgramProb>[] ngramProbCollection)
        {
            var lastNgram = ngrams.Last();
            var selectedProb =
                ngramProbCollection.First(x => x.First<NgramProb>().Ngrams.Count() == lastNgram.Count() + 1);
            return PredictNextWordForConcreteNgram(lastNgram, selectedProb);
        }

        private IEnumerable<(string name, decimal prop)> PredictNextWordForConcreteNgram(IEnumerable<string> lastNgram,
            IEnumerable<NgramProb> selectedProb)
        {
            var enumerable = lastNgram as string[] ?? lastNgram.ToArray();
            var rate = enumerable.Count();
            var supportedWords = new Dictionary<string, decimal>();
            foreach (var ngramProb in selectedProb)
            {
                if (CompareNgram(ngramProb, enumerable))
                {
                    if (supportedWords.ContainsKey(ngramProb.Ngrams.Last()))
                    {
                        if (ngramProb.Ngrams.Last().Length > 2)
                        {
                            supportedWords[ngramProb.Ngrams.Last()] += rate * rate * rate * ngramProb.Probability;
                        }
                        else
                        {
                            supportedWords[ngramProb.Ngrams.Last()] += ngramProb.Probability;
                        }
                    }
                    else
                    {
                        if (ngramProb.Ngrams.Last().Length > 2)
                        {
                            supportedWords.Add(ngramProb.Ngrams.Last(), rate * rate * rate * ngramProb.Probability);
                        }
                        else
                        {
                            supportedWords.Add(ngramProb.Ngrams.Last(), ngramProb.Probability);
                        }
                    }
                }
            }

            if (!supportedWords.Any()) return (new[] { (name: string.Empty, default(decimal)) }).AsEnumerable();
            var result = supportedWords.OrderByDescending(x => x.Value).Take(3);
            return result.Select(r => (r.Key, r.Value));
        }

        private bool CompareNgram(NgramProb ngramProb, IEnumerable<string> lastNgram)
        {
            var res = true;
            var ngrams = lastNgram as string[] ?? lastNgram.ToArray();
            var ngramProbNgrams = ngramProb.Ngrams as string[] ?? ngramProb.Ngrams.ToArray();
            for (var i = 0; i < ngrams.Length; i++)
            {
                res = res && ngrams[i] == ngramProbNgrams[i];
            }

            return res;
        }
    }
}