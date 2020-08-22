using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Collections
{
    public struct FilteredEnumerator<T>
        : IEnumerator<T>
    {
        private IEnumerator<T> _baseEnumerator;

        private Func<T, bool> _predicate;

        public T Current => _baseEnumerator.Current;

        object IEnumerator.Current => Current;

        public FilteredEnumerator(IEnumerable<T> baseEnumerable, Func<T, bool> predicate)
        {
            _baseEnumerator = baseEnumerable.GetEnumerator();
            _predicate = predicate;
        }

        public FilteredEnumerator(IEnumerator<T> baseEnumerator, Func<T, bool> predicate)
        {
            _baseEnumerator = baseEnumerator;
            _predicate = predicate;
        }

        public bool MoveNext()
        {
            while (_baseEnumerator.MoveNext())
            {
                if (_predicate(Current))
                {
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            _baseEnumerator.Reset();
        }

        public void Dispose()
        {
        }
    }
}
