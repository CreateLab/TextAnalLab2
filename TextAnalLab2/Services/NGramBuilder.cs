using System.Collections.Generic;
using System.Linq;

namespace TextAnalLab2
{
    public class NGramBuilder
    {
        public IEnumerable<IEnumerable<T>> BuildNGrams<T>(IEnumerable<T> enumerable, int ngramSize)
        {
            var list = enumerable.ToList();
            for (var i = 0; i <= list.Count - ngramSize; i++)
            {
                yield return list.Skip(i).Take(ngramSize);
            }
        }

       
    }
}