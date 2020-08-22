using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.NewEnumeratorDesign
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Functional;
    using ScrollStitch.V20200707.Spatial;
    using ScrollStitch.V20200707.Spatial.Internals;

    public class FilteredRectDataEnumerator<T>
        : IEnumerator<(Rect, T)>
    {
        public IEnumerator<(Rect, T)> BaseEnumerator { get; }

        public Func<(Rect, T), bool> FilterPredicate { get; }

        public (Rect, T) Current => BaseEnumerator.Current;

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            while (BaseEnumerator.MoveNext())
            {
                if (FilterPredicate(BaseEnumerator.Current))
                {
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            BaseEnumerator.Reset();
        }

        public void Dispose()
        {
        }
    }
}
