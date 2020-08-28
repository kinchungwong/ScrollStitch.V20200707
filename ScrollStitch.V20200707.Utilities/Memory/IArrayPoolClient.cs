using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Memory
{
    /// <summary>
    /// Defines the minimum functionality required on the client side of an array pool.
    /// </summary>
    public interface IArrayPoolClient<T>
    {
        /// <summary>
        /// Request an array of exact length.
        /// </summary>
        /// <param name="length">
        /// The exact length of the requested array.
        /// </param>
        /// <returns>
        /// An array having the exactly requested length. If the array pool does not have 
        /// an array of this length, a new one is created and returned.
        /// </returns>
        T[] Rent(int length);

        /// <summary>
        /// Requests an array of at least this minimum length, and possibly up the 
        /// the specified maximum.
        /// </summary>
        /// <param name="minLength">
        /// The minimum array length. Inclusive.
        /// </param>
        /// <param name="maxLength">
        /// The maximum array length. Inclusive. If there is no upper bound on array length, use <see cref="int.MaxValue"/>.
        /// </param>
        /// <returns></returns>
        T[] Rent(int minLength, int maxLength);

        /// <summary>
        /// Returns the array to the array pool.
        /// </summary>
        /// <param name="array"></param>
        void Return(T[] array);
    }
}
