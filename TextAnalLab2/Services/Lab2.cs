using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TextAnalLab2.Models;

namespace TextAnalLab2
{
    public class Lab2
    {
        private readonly TextCleaner _textCleaner;
        private readonly NGramBuilder _nGramBuilder;
        private readonly NGramProbBuilder _nGramProbBuilder;
        private readonly Predicter _predicter;

        public Lab2(TextCleaner textCleaner, NGramProbBuilder nGramProbBuilder, NGramBuilder nGramBuilder, Predicter predicter)
        {
            _textCleaner = textCleaner;
            _nGramProbBuilder = nGramProbBuilder;
            _nGramBuilder = nGramBuilder;
            _predicter = predicter;
        }

        public string Main(string inputText, string path, int wordscount)
        {
            var ngramCollection = GenerateNGramCollection(File.ReadAllText(path), 2);
            var ngramTextPropbCollection = ngramCollection.AsParallel().Select(_nGramProbBuilder.CreateTextProbability)
                .OrderBy(x => x.First().Ngrams.Count()).ToArray();
            var ngramProbCollection = ngramTextPropbCollection.AsParallel().Select(x =>
                _nGramProbBuilder.CreateLaplasProbability(x, GetTotalTextCount(x, ngramCollection), x.Count())).ToArray();

            for (var i = 0; i < wordscount; i++)
            {
                var inputNgramCollection = GenerateNGramCollection(inputText,1);
                inputText += _predicter.PredictNextWord(inputNgramCollection, ngramProbCollection);
            }

            return inputText;
        }

        private int GetTotalTextCount(IEnumerable<NgramProb> ngramProbs, IEnumerable<IEnumerable<string>>[] ngramCollection)
        {
            var ngramCount = ngramProbs.First().Ngrams.Count();
            var enumb = ngramCollection.Where(x => x.First().Count() == ngramCount).First();
            return enumb.Count();
        }

        private IEnumerable<IEnumerable<string>>[] GenerateNGramCollection(string text, int start)
        {
            var clearTextCollection = _textCleaner.Clear(text);
            var ngramCollection = Enumerable.Range(start, 3).AsParallel().Select(
                x => _nGramBuilder.BuildNGram(clearTextCollection, x)).OrderBy(x => x.First().Count()).ToArray();
            return ngramCollection;
        }
    }
}