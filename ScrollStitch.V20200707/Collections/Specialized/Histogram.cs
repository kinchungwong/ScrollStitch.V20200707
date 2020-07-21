using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Collections.Specialized
{
    using ScrollStitch.V20200707.Collections.Internals;

    public class Histogram<TKey, TBin> 
        : IHistogram<TKey, TBin>
    {
        private IHistArith<TBin> _histArith;
        private UniqueList<TKey> _mapping;
        private List<TBin> _bins;

        public int Count => _mapping.Count;
        public TBin DefaultIncrement { get; set; }

        public TBin this[TKey key] => _bins[_mapping.IndexOf(key)];

        public Histogram(IHistArith<TBin> histArith)
        {
            _histArith = histArith;
            _mapping = new UniqueList<TKey>();
            _bins = new List<TBin>();
            DefaultIncrement = histArith.One;
        }

        public TBin Add(TKey key) 
        {
            return Add(key, DefaultIncrement);
        }

        public void AddRange(IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
            {
                Add(key);
            }
        }

        public TBin Add(TKey key, TBin amount)
        {
            int index = _mapping.Add(key);
            while (index >= _bins.Count)
            {
                _bins.Add(_histArith.Zero);
            }
            TBin oldValue = _bins[index];
            TBin newValue = _histArith.AddFunc(oldValue, amount);
            _bins[index] = newValue;
            return newValue;
        }

        public void AddRange(IEnumerable<ValueTuple<TKey, TBin>> keysAndIncrements)
        {
            foreach ((TKey key, TBin increment) in keysAndIncrements)
            {
                Add(key, increment);
            }
        }

        public void AddRange(IEnumerable<KeyValuePair<TKey, TBin>> keysAndIncrements)
        {
            foreach (var kvp in keysAndIncrements)
            {
                TKey key = kvp.Key;
                TBin increment = kvp.Value;
                Add(key, increment);
            }
        }

        public void AddRange(IHistogram<TKey, TBin> other)
        {
            void func(TKey key, TBin value) => Add(key, value);
            other.ForEach(func);
        }

        public void ForEach(Action<TKey, TBin> keyAndBinFunc)
        {
            int count = _mapping.Count;
            for (int k = 0; k < count; ++k)
            {
                TKey key = _mapping.ItemAt(k);
                TBin value = _bins[k];
                keyAndBinFunc?.Invoke(key, value);
            }
        }

        public IHistogram<TKey, TBin> GetFiltered(Func<TKey, TBin, bool> filterFunc)
        {
            var filtered = new Histogram<TKey, TBin>(_histArith);
            void func(TKey key, TBin value)
            { 
                if (filterFunc(key, value))
                {
                    filtered.Add(key, value);
                }
            }
            ForEach(func);
            return filtered;
        }

        public Dictionary<TKey, TBin> ToDictionary()
        {
            int count = _mapping.Count;
            var dict = new Dictionary<TKey, TBin>(capacity: count);
            ForEach((key, value) => dict.Add(key, value));
            return dict;
        }

        public List<KeyValuePair<TKey, TBin>> ToList()
        {
            int count = _mapping.Count;
            var list = new List<KeyValuePair<TKey, TBin>>(capacity: count);
            ForEach((key, value) => list.Add(new KeyValuePair<TKey, TBin>(key, value)));
            return list;
        }

        public IEnumerator<KeyValuePair<TKey, TBin>> GetEnumerator()
        {
            return ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerator<KeyValuePair<TKey, TBin>> e = GetEnumerator();
            return e;
        }
    }
}
