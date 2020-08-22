using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Collections
{
    using ScrollStitch.V20200707.Functional;

    public struct FilteredEnumerableInline<PredicateType, T>
        : IEnumerable<T>
        where PredicateType : struct, IFuncInline<PredicateType, T, bool>
    {
        private IEnumerable<T> _baseEnumerable;

        private PredicateType _predicate;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FilteredEnumerableInline(IEnumerable<T> baseEnumerable, PredicateType predicate)
        {
            _baseEnumerable = baseEnumerable;
            _predicate = predicate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FilteredEnumeratorInline<PredicateType, T> GetEnumerator()
        {
            return new FilteredEnumeratorInline<PredicateType, T>(_baseEnumerable.GetEnumerator(), 
                _predicate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
