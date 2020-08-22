using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Collections
{
    public struct FilteredEnumerable<T>
        : IEnumerable<T>
    {
        private IEnumerable<T> _baseEnumerable;

        private Func<T, bool> _predicate;

        public FilteredEnumerable(IEnumerable<T> baseEnumerable, Func<T, bool> predicate)
        {
            _baseEnumerable = baseEnumerable;
            _predicate = predicate;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new FilteredEnumerator<T>(_baseEnumerable, _predicate);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
