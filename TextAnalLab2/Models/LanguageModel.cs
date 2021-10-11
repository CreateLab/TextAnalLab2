using System;
using System.Collections.Generic;
using System.Linq;

namespace TextAnalLab2.Models
{
    public class LanguageModel
    {
        private readonly TextCleaner _textCleaner;
        private readonly NGramBuilder _nGramBuilder;

        private Dictionary<Ngram, int> _ngramCounts;
        private Dictionary<Ngram, decimal> _ngramProb;

        private readonly decimal _alpha;
        private readonly int _corpusVocabularySize;
        private readonly int _corpusSize;

        private readonly Dictionary<int, decimal> _backoffCoefficientsPerNGramSize;
        private int _nGramLength;

        public LanguageModel(NGramBuilder nGramBuilder, TextCleaner textCleaner,
            Dictionary<Ngram, int> ngramCounts, decimal alpha, int corpusVocabularySize, int corpusSize,
            Dictionary<int, decimal> backoffCoefficientsPerNGramSize, int nGramLength)
        {
            _nGramBuilder = nGramBuilder;
            _textCleaner = textCleaner;
            _ngramCounts = ngramCounts;
            _alpha = alpha;
            _corpusVocabularySize = corpusVocabularySize;
            _corpusSize = corpusSize;
            _backoffCoefficientsPerNGramSize = backoffCoefficientsPerNGramSize;
            _nGramLength = nGramLength;

            CountNgramProb();
        }

        public double Assess(string text)
        {
            var sentence = _textCleaner.SplitToSentence(text);
            var wordsSentenceCollection = sentence.Select(x =>
            {
                var collection = x.Split().Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                collection.AddRange(Enumerable.Repeat("$", _nGramLength - 1));
                var result = Enumerable.Repeat("$", _nGramLength - 1).ToList();
                result.AddRange(collection);
                return result;
            });

            var ngrams =
                wordsSentenceCollection.SelectMany(x => _nGramBuilder.BuildNGrams(x, _nGramLength));

            var wordCount = wordsSentenceCollection.SelectMany(x => x).Count();

            var prediction = ngrams.Select(x => Math.Log10((double)BackOffLaplaceSmoothedProbability(x) + 2)).Sum();

            return Math.Pow((double)(prediction), (-1.0 / wordCount));
        }

        private decimal LaplaceSmoothedProbability(IEnumerable<string> ngram)
        {
            var enumerable = ngram as string[] ?? ngram.ToArray();

            var shortNgram = new Ngram
            {
                NgramElems = enumerable.Take(enumerable.Length - 1)
            };

            var longNgram = new Ngram
            {
                NgramElems = enumerable
            };

            var shortCount = enumerable.Length == 1 ? _corpusSize :
                _ngramCounts.TryGetValue(shortNgram, out var count) ? count : 0;

            var longCount = _ngramCounts.TryGetValue(longNgram, out var newCount) ? newCount : 0;

            return (longCount + _alpha) / (shortCount + _corpusVocabularySize * _alpha);
        }

        private decimal BackOffLaplaceSmoothedProbability(IEnumerable<string> ngram)
        {
            var ngramArray = ngram as string[] ?? ngram.ToArray();

            var res = ngramArray.Select((t, i) => (IEnumerable<string>)ngramArray.Take(ngramArray.Length - i))
                .Select((shortNgram, i) => _backoffCoefficientsPerNGramSize[ngramArray.Length - i] *
                                           LaplaceSmoothedProbability(shortNgram)).Sum();
            return res;
        }

        private void CountNgramProb()
        {
            _ngramProb = _ngramCounts.Select(x =>
            {
                var prob = BackOffLaplaceSmoothedProbability(x.Key.NgramElems);
                return (x.Key, prob);
            }).ToDictionary(x => x.Key, x => x.prob);
        }

        public string PredictNext(string text)
        {
            var sentence = _textCleaner.SplitToSentence(text);

            var cleanDictionary = _ngramProb.Where(x => !x.Key.NgramElems.Contains("$"))
                .ToDictionary(x => x.Key, x => x.Value);
            var lastWords = sentence.SelectMany(x =>
                x.Split().Where(s => !string.IsNullOrWhiteSpace(s)).ToList()
            ).TakeLast(_nGramLength - 1);

            var a = cleanDictionary.Where(x => x.Key.StartWith(lastWords)).ToArray();
            if (a.Length > 1)
            {
                return string.Join(" ", a.Take(1).Select(x => x.Key.NgramElems.Last()));
            }
            var keyValuePair = a.FirstOrDefault(x => x.Value == a.Max(x => x.Value));

            if (keyValuePair.Key != null)
                return keyValuePair.Key.NgramElems.Last();


            var lastWord = sentence.SelectMany(x =>
                x.Split().Where(s => !string.IsNullOrWhiteSpace(s)).ToList()
            ).TakeLast(_nGramLength - 2);

            var a1 = cleanDictionary.Where(x => x.Key.StartWith(lastWords)).ToArray();
            var keyValuePair2 = a1.FirstOrDefault(x => x.Value == a.Max(x => x.Value));
            if (a1.Length > 1)
            {
                return string.Join(" ", a1.Take(1).Select(x => x.Key.NgramElems.Last()));
            }
            return keyValuePair2.Key?.NgramElems.Last() ?? cleanDictionary.Where(x => x.Key.NgramElems.Count() == 1 && x.Key.NgramElems.First() != "$")
                .OrderByDescending(x => x.Value).First().Key.NgramElems.First();
        }
    }
}