using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial
{
    using ScrollStitch.V20200707.Data;

    public interface IRectQuery<T>
    {
        /// <summary>
        /// Enumerates all items that have a positive-area overlap with the query rectangle.
        /// 
        /// <para>
        /// Normal functioning is not guaranteed if the collection has been modified after the instantiation 
        /// of the <see cref="IEnumerable{T}"/>. This is not an implementation requirement.
        /// </para>
        /// </summary>
        /// <param name="queryRect">
        /// The query rectangle.
        /// </param>
        /// <returns>
        /// An enumeration of items having a positive-area overlap with the query rectangle.
        /// </returns>
        IEnumerable<T> Enumerate(Rect queryRect);
    }
}
