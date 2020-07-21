using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Collections.Specialized
{
    public interface IHistogram<TKey, TBin>
        : IReadOnlyCollection<KeyValuePair<TKey, TBin>>
    {
        TBin this[TKey key] { get; }

        TBin DefaultIncrement { get; set; }

        TBin Add(TKey key);

        TBin Add(TKey key, TBin amount);

        void AddRange(IEnumerable<TKey> keys);

        void AddRange(IEnumerable<ValueTuple<TKey, TBin>> keysAndIncrements);

        void AddRange(IEnumerable<KeyValuePair<TKey, TBin>> keysAndIncrements);

        void AddRange(IHistogram<TKey, TBin> other);

        void ForEach(Action<TKey, TBin> keyAndBinFunc);

        IHistogram<TKey, TBin> GetFiltered(Func<TKey, TBin, bool> filterFunc);

        Dictionary<TKey, TBin> ToDictionary();

        List<KeyValuePair<TKey, TBin>> ToList();
    }
}
