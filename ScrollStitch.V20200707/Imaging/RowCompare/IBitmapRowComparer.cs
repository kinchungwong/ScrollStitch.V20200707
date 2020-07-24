using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.RowCompare
{
    /// <summary>
    /// <see cref="IBitmapRowComparer{TPixel, TSum}"/> is generic interface for a pixel comparison 
    /// function that is logically elementwise, but operates on a whole row of pixels in order to 
    /// reduce runtime execution overhead.
    /// 
    /// <para>
    /// The exact behavior of the comparison function is dependent on the concrete implementation.
    /// </para>
    /// </summary>
    /// 
    /// <typeparam name="TPixel">
    /// The type of pixel values to be compared.
    /// </typeparam>
    /// 
    /// <typeparam name="TSum">
    /// The type to be used for reporting the summary pixel differences.
    /// </typeparam>
    /// 
    public interface IBitmapRowComparer<TPixel, TSum>
    {
        /// <summary>
        /// Compares two arrays elementwise, and writes comparison results to the two summary arrays.
        /// 
        /// <para>
        /// Important implementation remark.
        /// <br/>
        /// The concrete implementation of this method must accumulate to this array without 
        /// clearing. This is so that, when this method is called successively on each pair of rows 
        /// from the two bitmaps, the accumulated elementwise statistic will become the column 
        /// statistic of the whole bitmap comparison operation.
        /// </para>
        /// </summary>
        /// 
        /// <param name="input1">
        /// The first of the two arrays to be compared.
        /// </param>
        /// 
        /// <param name="input2">
        /// The second of the two arrays to be compared. The lengths of the two arrays must be equal.
        /// </param>
        /// 
        /// <param name="outItemDiff">
        /// An <see cref="ArraySegment{TSum}"/> where the elementwise difference statistic will be 
        /// accumulated to.
        /// <br/>
        /// If that output is not needed, pass in a default-initialized <c>ArraySegment</c>.
        /// <br/>
        /// If a non-empty instance of <c>ArraySegment</c> is specified, its <c>Count</c>
        /// must be equal to the length of the arrays.
        /// </param>
        /// 
        /// <returns>
        /// The sum of accumulated differences.
        /// </returns>
        /// 
        TSum Compare(
            ArraySegment<TPixel> input1, 
            ArraySegment<TPixel> input2, 
            ArraySegment<TSum> outItemDiff);
    }
}
