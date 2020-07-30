using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Collections
{
    /// <summary>
    /// This shim implements <see cref="IReadOnlyCollection{T}"/> by bundling the item count 
    /// with the enumerable.
    /// 
    /// <para>
    /// To instantiate this shim, provide the constructor with an item count and an 
    /// <see cref="IEnumerable{T}"/>.
    /// </para>
    /// 
    /// <para>
    /// For users of this instance, <br/>
    /// The <see cref="Count"/> property is a constant time operation. <br/>
    /// Accessing the items will still take linear time, by iterating through the collection.
    /// </para>
    /// 
    /// <para>
    /// This shim does not provide a <see cref="IReadOnlyList{T}"/> because it does not have a 
    /// constant-time item indexer function.
    /// </para>
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The item type.
    /// </typeparam>
    /// 
    public class ReadOnlyEnumerableCollection<T>
        : IReadOnlyCollection<T>
    {
        public int Count { get; }

        public IEnumerable<T> Enumerable { get; }

        public ReadOnlyEnumerableCollection(int count, IEnumerable<T> enumerable)
        {
            Count = count;
            Enumerable = enumerable;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Enumerable.GetEnumerator();
        }
    }
}
