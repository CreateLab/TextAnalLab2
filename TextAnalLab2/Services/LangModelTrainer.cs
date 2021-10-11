using System;
using System.Collections.Generic;
using System.Linq;
using TextAnalLab2.Models;

namespace TextAnalLab2.Services
{
    public class LangModelTrainer
    {
        private readonly TextCleaner _textCleaner;
        private readonly NGramBuilder _nGramBuilder;
        private readonly NGramProbBuilder _ngramProb;


        public LangModelTrainer(TextCleaner textCleaner, NGramBuilder nGramBuilder, NGramProbBuilder ngramProb)
        {
            _textCleaner = textCleaner;
            _nGramBuilder = nGramBuilder;
            _ngramProb = ngramProb;
        }


        public LanguageModel Train(string corpus, decimal alpha, int ngramLength,
            Dictionary<int, decimal> backoffCoefficientsPerNGramSize)
        {
            if (backoffCoefficientsPerNGramSize.Keys.Count() != ngramLength) throw new Exception("ты пбес");
            var sentence = _textCleaner.SplitToSentence(corpus);
            var wordsSentenceCollection = sentence.Select(x =>
            {
                var collection = x.Split().Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                collection.AddRange(Enumerable.Repeat("$", ngramLength - 1));
                var result = Enumerable.Repeat("$", ngramLength - 1).ToList();
                result.AddRange(collection);
                return result;
            }).ToArray();

            var listNgrams = new List<IEnumerable<string>>();
            for (var i = 1; i <= ngramLength; i++)
            {
                listNgrams.AddRange(wordsSentenceCollection.SelectMany(x => _nGramBuilder.BuildNGrams(x, i)));
            }


            var ngramCounts = _ngramProb.Count(listNgrams).ToDictionary(x => new Ngram
            {
                NgramElems = x.Ngram
            }, x => (int)x.Count);

            var corpusVocabularySize = wordsSentenceCollection.SelectMany(x => x).Distinct().Count();
            var corpSize = wordsSentenceCollection.SelectMany(x => x).Count();


            return new LanguageModel(_nGramBuilder, _textCleaner, ngramCounts, alpha, corpusVocabularySize, corpSize,
                backoffCoefficientsPerNGramSize, ngramLength);
        }
    }
}