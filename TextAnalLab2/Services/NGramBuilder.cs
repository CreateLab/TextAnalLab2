using System.Collections.Generic;
using System.Linq;

namespace TextAnalLab2
{
    public class NGramBuilder
    {
        public IEnumerable<IEnumerable<T>> BuildNGram<T>(IEnumerable<T> enumerable, int count)
        {
            var enumerable1 = enumerable.ToList();
            for (var i = 0; i <= enumerable1.Count - count; i++)
            {
                yield return enumerable1.Skip(i).Take(count);
            }
        }
    }
}