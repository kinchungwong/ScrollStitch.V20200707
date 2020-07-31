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

        /// <summary>
        /// Returns the histogram key(s) which receive the highest votes.
        /// 
        /// Return a list of <see cref="KeyValuePair"/> containing the highest voted histogram keys.
        /// </summary>
        /// 
        /// <returns>
        /// A list of <see cref="KeyValuePair{TKey, TValue}"/>. 
        /// <br/>
        /// If the histogram is empty, an empty list is returned. 
        /// <br/>
        /// If there is one histogram key with the highest vote, a list containing that key and vote 
        /// is returned. 
        /// <br/>
        /// If there are multiple histogram keys that tie for the highest vote, a list containing 
        /// such keys and their votes is returned.
        /// </returns>
        ///
        List<KeyValuePair<TKey, TBin>> GetPeaks();
    }
}
