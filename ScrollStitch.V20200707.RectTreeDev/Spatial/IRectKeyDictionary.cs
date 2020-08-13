using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial
{
    using ScrollStitch.V20200707.Data;

    /// <inheritdoc/>
    public interface IRectKeyDictionary<TValue>
        : IDictionary<Rect, TValue>
    {
#if false
        TValue this[Rect key] { get; set; }
        ICollection<Rect> Keys { get; }
        ICollection<TValue> Values { get; }
        void Add(Rect key, TValue value);
        bool ContainsKey(Rect key);
        bool Remove(Rect key);
        bool TryGetValue(Rect key, out TValue value);
#endif
    }
}
