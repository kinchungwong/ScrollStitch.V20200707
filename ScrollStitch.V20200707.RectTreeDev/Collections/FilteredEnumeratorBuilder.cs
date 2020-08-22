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

    public static class FilteredEnumeratorBuilder
    {
        public static FilteredEnumerator<T> Create<T>(IEnumerable<T> baseEnumerable, Func<T, bool> predicate)
        {
            return new FilteredEnumerator<T>(baseEnumerable, predicate);
        }

        public static FilteredEnumerator<T> Create<T>(IEnumerator<T> baseEnumerator, Func<T, bool> predicate)
        {
            return new FilteredEnumerator<T>(baseEnumerator, predicate);
        }

        public static FilteredEnumerator<T> Create<T>(IEnumerable<T> baseEnumerable, IFunc<T, bool> predicate)
        {
            // ======
            // Pending question: should there be a specialized wrapper for this case?
            // Option 1: Provides a lambda that captures and calls the IFunc
            // Option 2: Wraps the IFunc into an IFuncInline class
            // ======
            return new FilteredEnumerator<T>(baseEnumerable, (T t) => predicate.Invoke(t));
        }

        public static FilteredEnumerator<T> Create<T>(IEnumerator<T> baseEnumerator, IFunc<T, bool> predicate)
        {
            // ======
            // Pending question: should there be a specialized wrapper for this case?
            // Option 1: Provides a lambda that captures and calls the IFunc
            // Option 2: Wraps the IFunc into an IFuncInline class
            // ======
            return new FilteredEnumerator<T>(baseEnumerator, (T t) => predicate.Invoke(t));
        }

        public static FilteredEnumeratorInline<PredicateType, T> Create<PredicateType, T>(
            IEnumerable<T> baseEnumerable, PredicateType predicate)
            where PredicateType : struct, IFuncInline<PredicateType, T, bool>
        {
            return new FilteredEnumeratorInline<PredicateType, T>(baseEnumerable.GetEnumerator(), predicate);
        }

        public static FilteredEnumeratorInline<PredicateType, T> Create<PredicateType, T>(
            IEnumerator<T> baseEnumerator,  PredicateType predicate)
            where PredicateType : struct, IFuncInline<PredicateType, T, bool>
        {
            return new FilteredEnumeratorInline<PredicateType, T>(baseEnumerator, predicate);
        }
    }
}
