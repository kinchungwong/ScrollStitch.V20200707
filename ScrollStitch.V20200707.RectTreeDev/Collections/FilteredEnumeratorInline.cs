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

    public struct FilteredEnumeratorInline<PredicateType, T>
        : IEnumerator<T>
        where PredicateType : struct, IFuncInline<PredicateType, T, bool>
    {
        private IEnumerator<T> _baseEnumerator;

        private PredicateType _predicate;

        public T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _baseEnumerator.Current;
        }

        object IEnumerator.Current => Current;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FilteredEnumeratorInline(IEnumerator<T> baseEnumerator, PredicateType predicate)
        {
            _baseEnumerator = baseEnumerator;
            _predicate = predicate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            while (_baseEnumerator.MoveNext())
            {
                if (_predicate.Invoke(Current))
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
